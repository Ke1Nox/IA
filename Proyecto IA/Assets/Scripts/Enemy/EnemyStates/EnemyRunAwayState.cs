using UnityEngine;

/// <summary>
/// Estado RunAway del enemigo.
/// El NPC huye del jugador usando Evade (steering behaviour complejo):
/// predice la posicion futura del jugador y escapa de ahi, evitando
/// ser atrapado aunque el jugador intente cortarle el camino.
/// Tambien aplica ObstacleAvoidance para no chocar mientras huye.
/// Tras un tiempo determinado, vuelve a Patrol si no hay amenaza.
/// </summary>
public class EnemyRunAwayState : State<StateEnum>
{
    private IMove _move;
    private Transform _entity;
    private Transform _target;

    private ISteering _evade;
    private ISteering _obstacleAvoidance;
    private Vector3 _currentSpeed;

    private float _maxSpeed;
    private LayerMask _obsMask;

    private float _runDuration;
    private float _timer;

    /// <summary>True cuando termino el tiempo de huida: el Controller puede transicionar.</summary>
    public bool RunAwayFinished => _timer >= _runDuration;

    public EnemyRunAwayState(IMove move, Transform entity, Transform target,
                             float maxSpeed, LayerMask obsMask, float runDuration = 4f)
    {
        _move = move;
        _entity = entity;
        _target = target;
        _maxSpeed = maxSpeed;
        _obsMask = obsMask;
        _runDuration = runDuration;
    }

    public override void Enter()
    {
        base.Enter();

        // Evade.cs debe estar en Assets/Scripts/Enemy/Steering/
        _evade = new Evade(_target, _entity, _maxSpeed);

        // ObstacleAvoidance.cs debe estar en Assets/Scripts/Enemy/Steering/
        _obstacleAvoidance = new ObstacleAvoidance(_entity, 3f, 40f, 1.5f, _obsMask, 10);

        _currentSpeed = Vector3.zero;
        _timer = 0f;
        Debug.Log("[EnemyRunAwayState] Huyendo con Evade!");
    }

    public override void Execute()
    {
        base.Execute();

        _timer += Time.deltaTime;

        if (_target == null) return;

        // 1. Evade: predice y huye de la posicion futura del jugador
        Vector3 evadeDir = _evade.GetDir(_currentSpeed);

        // 2. ObstacleAvoidance: evita chocar con paredes mientras huye
        _currentSpeed = _obstacleAvoidance.GetDir(evadeDir);

        // 3. Movemos y orientamos al agente
        _move.Move(_currentSpeed.normalized);

        Vector3 lookDir = _currentSpeed;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            _move.Look(lookDir);
    }
}