using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolState : State<StateEnum>
{
    IMove _move;
    Transform _entity;
    List<Transform> _waypoints;

    int _currentIndex = 0;
    int _direction = 1; // ida y vuelta

    float _arriveDistance = 0.5f;

    public EnemyPatrolState(IMove move, Transform entity, List<Transform> waypoints)
    {
        _move = move;
        _entity = entity;
        _waypoints = waypoints;
    }

    public override void Execute()
    {
        base.Execute();

        if (_waypoints == null || _waypoints.Count == 0) return;

        Transform wp = _waypoints[_currentIndex];

        Vector3 dir = wp.position - _entity.position;
        dir.y = 0;

        // llegó al waypoint
        if (dir.magnitude <= _arriveDistance)
        {
            _currentIndex += _direction;

           
            if (_currentIndex >= _waypoints.Count)
            {
                _currentIndex = _waypoints.Count - 2;
                _direction = -1;
            }
            else if (_currentIndex < 0)
            {
                _currentIndex = 1;
                _direction = 1;
            }
        }

        _move.Move(dir.normalized);
        _move.Look(dir);
    }
}