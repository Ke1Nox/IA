using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Estado Patrol del enemigo.
/// El NPC recorre waypoints en orden ida/vuelta.
/// Cuenta cuantos waypoints completos ha recorrido: tras N iteraciones,
/// activa el flag PatrolIterationComplete para que el Controller lo pase a Idle.
/// </summary>
public class EnemyPatrolState : State<StateEnum>
{
    private IMove _move;
    private Transform _entity;
    private List<Transform> _waypoints;

    private int _currentIndex = 0;
    private int _direction = 1;          // 1: avanza, -1: retrocede (ida y vuelta)
    private float _arriveDistance = 0.5f;

    private int _patrolIterations = 0;   // Cuantos waypoints completos recorrio
    private int _iterationsToIdle;       // Cuantos necesita para ir a Idle

    /// <summary>
    /// True cuando acumulo suficientes iteraciones: el Controller debe pasar a Idle.
    /// Se resetea al entrar al estado nuevamente.
    /// </summary>
    public bool PatrolIterationComplete { get; private set; }

    /// <param name="iterationsToIdle">Cantidad de waypoints alcanzados antes de ir a Idle</param>
    public EnemyPatrolState(IMove move, Transform entity, List<Transform> waypoints, int iterationsToIdle = 4)
    {
        _move = move;
        _entity = entity;
        _waypoints = waypoints;
        _iterationsToIdle = iterationsToIdle;
    }

    public override void Enter()
    {
        base.Enter();
        // Reseteamos el flag y el contador al re-entrar en Patrol
        PatrolIterationComplete = false;
        _patrolIterations = 0;
        Debug.Log("[EnemyPatrolState] Iniciando patrulla.");
    }

    public override void Execute()
    {
        base.Execute();

        if (_waypoints == null || _waypoints.Count == 0) return;

        Transform wp = _waypoints[_currentIndex];
        Vector3 dir = wp.position - _entity.position;
        dir.y = 0;

        // Llegamos al waypoint
        if (dir.magnitude <= _arriveDistance)
        {
            _patrolIterations++;
            Debug.Log("[EnemyPatrolState] Waypoint alcanzado. Iteraciones: " + _patrolIterations);

            // Chequeamos si acumulamos suficientes iteraciones para descansar
            if (_patrolIterations >= _iterationsToIdle)
            {
                PatrolIterationComplete = true;
                return; // El Controller detectara este flag y hara la transicion
            }

            // Avanzamos al siguiente waypoint (ida y vuelta)
            _currentIndex += _direction;

            if (_currentIndex >= _waypoints.Count)
            {
                // Llegamos al final: invertimos direccion
                _currentIndex = _waypoints.Count - 2;
                _direction = -1;
            }
            else if (_currentIndex < 0)
            {
                // Llegamos al inicio: volvemos a avanzar
                _currentIndex = 1;
                _direction = 1;
            }
        }

        _move.Move(dir.normalized);
        _move.Look(dir);
    }
}
