using UnityEngine;

/// <summary>
/// Estado Chase del enemigo - persigue al jugador con Pursuit.
/// A diferencia de Seek (que va directo a la posicion actual),
/// Pursuit predice la posicion FUTURA del jugador y se dirige hacia ahi,
/// resultando en una persecucion mas inteligente que corta el paso al objetivo.
/// Combina Pursuit + ObstacleAvoidance para esquivar paredes mientras persigue.
/// </summary>
public class EnemyChaseState : State<StateEnum>
{
    private IMove _move;
    private Transform _entity;
    private Transform _target;

    // Steering behaviours: Pursuit para perseguir + ObstacleAvoidance para no chocar
    private ISteering _pursuit;
    private ISteering _obstacleAvoidance;
    private Vector3 _currentSpeed;

    private float _maxSpeed;
    private LayerMask _obsMask;

    public EnemyChaseState(IMove move, Transform entity, Transform target,
                           float maxSpeed, LayerMask obsMask)
    {
        _move = move;
        _entity = entity;
        _target = target;
        _maxSpeed = maxSpeed;
        _obsMask = obsMask;
    }

    public override void Enter()
    {
        base.Enter();

        // Inicializamos los steerings al entrar (target puede haber cambiado)
        // Persuit.cs debe estar en Assets/Scripts/Enemy/Steering/
        _pursuit = new Persuit(_target, _entity, _maxSpeed);

        // ObstacleAvoidance: radio 3, angulo 40 grados, area personal 1.5
        // ObstacleAvoidance.cs debe estar en Assets/Scripts/Enemy/Steering/
        _obstacleAvoidance = new ObstacleAvoidance(_entity, 3f, 40f, 1.5f, _obsMask, 10);

        _currentSpeed = Vector3.zero;
        Debug.Log("[EnemyChaseState] Persiguiendo al jugador con Pursuit.");
    }

    public override void Execute()
    {
        base.Execute();

        if (_target == null) return;

        // 1. Pursuit calcula la direccion prediciendo la posicion futura del jugador
        Vector3 pursuitDir = _pursuit.GetDir(_currentSpeed);

        // 2. ObstacleAvoidance devia la direccion si hay un obstaculo en el camino
        _currentSpeed = _obstacleAvoidance.GetDir(pursuitDir);

        // 3. Movemos el agente con la velocidad resultante
        _move.Move(_currentSpeed.normalized);

        // Miramos hacia donde nos movemos (ignorando eje Y para no inclinar el modelo)
        Vector3 lookDir = _currentSpeed;
        lookDir.y = 0;
        if (lookDir != Vector3.zero)
            _move.Look(lookDir);
    }
}
