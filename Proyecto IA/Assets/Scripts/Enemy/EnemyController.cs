using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// EnemyController - Controlador principal de la IA del enemigo.
/// 
/// La Roulette Wheel se dispara UNA SOLA VEZ cuando el enemigo detecta al jugador
/// por primera vez. El flag _hasReacted evita que se re-dispare en cada frame.
/// Una vez elegido el estado (Chase/RunAway/Patrol), el enemigo lo mantiene
/// hasta que pierde la vision del jugador, momento en que se resetea.
/// </summary>
public class EnemyController : MonoBehaviour
{
    [Header("Referencias")]
    public Transform target;
    public LineOfSight los;
    public List<Transform> waypoints;

    [Header("Configuracion")]
    public EnemyGroup enemyGroup = EnemyGroup.Aggressive;
    [Tooltip("Cuantos waypoints recorre antes de hacer Idle")]
    public int iterationsToIdle = 4;
    [Tooltip("Segundos en estado Idle antes de volver a Patrol")]
    public float idleDuration = 3f;
    [Tooltip("Segundos huyendo antes de reevaluar")]
    public float runAwayDuration = 4f;
    public LayerMask obstacleMask;

    private FSM<StateEnum> _fsm;
    private IAttack _entityAttack;
    private ITreeNode _root;

    private EnemyIdleState _idleState;
    private EnemyPatrolState _patrolState;
    private EnemyRunAwayState _runAwayState;
    private EnemyChaseState _chaseState;
    private EnemyAttackState _attackState;

    private Dictionary<StateEnum, float> _rouletteWeights;

    /// <summary>
    /// Evita que la ruleta se dispare multiples veces mientras el enemigo
    /// ya tomo una decision. Se resetea cuando pierde la vision del jugador
    /// o cuando termina un estado temporal (Idle, RunAway).
    /// </summary>
    private bool _hasReacted = false;

    private void Start()
    {
        InitRoulette();
        InitializedFSM();
        InitializedTree();
    }

    /// <summary>
    /// Configura los pesos de la ruleta segun el grupo del enemigo.
    /// GRUPO AGRESIVO: 85% Chase, 10% RunAway, 5% Patrol
    /// GRUPO COBARDE:  15% Chase, 70% RunAway, 15% Patrol
    /// Cumple el requisito: 3 resultados con diferentes probabilidades.
    /// </summary>
    void InitRoulette()
    {
        _rouletteWeights = new Dictionary<StateEnum, float>();

        if (enemyGroup == EnemyGroup.Aggressive)
        {
            _rouletteWeights[StateEnum.Chase] = 85f;
            _rouletteWeights[StateEnum.RunAway] = 10f;
            _rouletteWeights[StateEnum.Patrol] = 5f;
        }
        else
        {
            _rouletteWeights[StateEnum.Chase] = 15f;
            _rouletteWeights[StateEnum.RunAway] = 70f;
            _rouletteWeights[StateEnum.Patrol] = 15f;
        }

        Debug.Log("[EnemyController] Grupo: " + enemyGroup + " | Ruleta configurada.");
    }

    void InitializedFSM()
    {
        IMove entityMove = GetComponent<IMove>();
        _entityAttack = GetComponent<IAttack>();
        float speed = GetComponent<Entity>().speed;

        _idleState = new EnemyIdleState(idleDuration);
        _patrolState = new EnemyPatrolState(entityMove, transform, waypoints, iterationsToIdle);
        _runAwayState = new EnemyRunAwayState(entityMove, transform, target, speed, obstacleMask, runAwayDuration);
        _chaseState = new EnemyChaseState(entityMove, transform, target, speed, obstacleMask);
        _attackState = new EnemyAttackState(_entityAttack);

        // Patrol
        _patrolState.AddTransition(StateEnum.Idle, _idleState);
        _patrolState.AddTransition(StateEnum.Chase, _chaseState);
        _patrolState.AddTransition(StateEnum.RunAway, _runAwayState);
        _patrolState.AddTransition(StateEnum.Attack, _attackState);

        // Idle
        _idleState.AddTransition(StateEnum.Patrol, _patrolState);
        _idleState.AddTransition(StateEnum.Chase, _chaseState);
        _idleState.AddTransition(StateEnum.RunAway, _runAwayState);

        // Chase
        _chaseState.AddTransition(StateEnum.Attack, _attackState);
        _chaseState.AddTransition(StateEnum.Idle, _idleState);
        _chaseState.AddTransition(StateEnum.Patrol, _patrolState);
        _chaseState.AddTransition(StateEnum.RunAway, _runAwayState);

        // RunAway
        _runAwayState.AddTransition(StateEnum.Patrol, _patrolState);
        _runAwayState.AddTransition(StateEnum.Chase, _chaseState);
        _runAwayState.AddTransition(StateEnum.Idle, _idleState);

        // Attack
        _attackState.AddTransition(StateEnum.Chase, _chaseState);
        _attackState.AddTransition(StateEnum.Idle, _idleState);
        _attackState.AddTransition(StateEnum.Patrol, _patrolState);

        _fsm = new FSM<StateEnum>(_patrolState);
    }

    void InitializedTree()
    {
        // Idle termino: volver a Patrol y resetear reaccion
        var checkIdleFinished = new ActionTree(() =>
        {
            if (_idleState.IsIdleFinished)
            {
                _hasReacted = false;
                _fsm.Transition(StateEnum.Patrol);
            }
        });

        // RunAway termino: volver a Patrol y resetear reaccion
        var checkRunAwayFinished = new ActionTree(() =>
        {
            if (_runAwayState.RunAwayFinished)
            {
                _hasReacted = false;
                _fsm.Transition(StateEnum.Patrol);
            }
        });

        // Ataque directo: el enemigo ya llego a rango, sin pasar por ruleta
        var doAttack = new ActionTree(() =>
        {
            _fsm.Transition(StateEnum.Attack);
        });

        // Ruleta: se dispara UNA SOLA VEZ gracias a _hasReacted
        var doRoulette = new ActionTree(() =>
        {
            if (_hasReacted) return; // Ya reacciono: mantener la decision actual

            _hasReacted = true;
            StateEnum chosen = MyRandom.RouletteWheelSelection(_rouletteWeights);
            Debug.Log("[Roulette] " + enemyGroup + " eligio: " + chosen);
            _fsm.Transition(chosen);
        });

        // Perdio vision: resetear para que pueda reaccionar la proxima vez
        var doLostSight = new ActionTree(() =>
        {
            if (_hasReacted)
            {
                _hasReacted = false;
                _fsm.Transition(StateEnum.Patrol);
            }
            else if (_patrolState.PatrolIterationComplete)
            {
                _fsm.Transition(StateEnum.Idle);
            }
        });

        // --- Arbol de decision ---
        //
        // Frame a frame:
        // 1. Si esta en Idle    -> esperar timer, luego Patrol
        // 2. Si esta en RunAway -> esperar timer, luego Patrol
        // 3. Ve al jugador:
        //      a. En rango de ataque -> Attack (siempre)
        //      b. Fuera de rango     -> Ruleta UNA VEZ
        // 4. No ve al jugador   -> resetear y volver a Patrol

        var qInAttackRange = new QuestionTree(InAttackRange, doAttack, doRoulette);
        var qInView = new QuestionTree(InView, qInAttackRange, doLostSight);
        var qIsRunning = new QuestionTree(IsRunningAway, checkRunAwayFinished, qInView);
        var qIsIdle = new QuestionTree(IsInIdle, checkIdleFinished, qIsRunning);

        _root = qIsIdle;
    }

    // ---- Condiciones ----

    bool InView()
    {
        if (target == null || los == null) return false;
        return los.CheckRange(target) && los.CheckAngle(target) && los.CheckView(target);
    }

    bool InAttackRange()
    {
        if (_entityAttack == null || target == null) return false;
        float dist = Vector3.Distance(target.position, transform.position);
        return dist <= _entityAttack.GetAttackRange;
    }

    bool IsInIdle()
    {
        // ReferenceEquals es mas seguro que casting para comparar instancias de estado
        return ReferenceEquals(_fsm.GetCurrent, _idleState);
    }

    bool IsRunningAway()
    {
        return ReferenceEquals(_fsm.GetCurrent, _runAwayState);
    }

    // ---- Unity Loop ----

    private void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
    }

    private void FixedUpdate() => _fsm.OnFixedUpdate();
    private void LateUpdate() => _fsm.OnLateUpdate();
}


