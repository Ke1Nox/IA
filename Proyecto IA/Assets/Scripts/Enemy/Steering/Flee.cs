using UnityEngine;

/// <summary>
/// Flee: Steering behaviour que aleja al agente del objetivo.
/// Es el inverso de Seek: calcula la direccion opuesta al target.
/// Se usa en el estado RunAway cuando el enemigo detecta al jugador muy cerca.
/// </summary>
public class Flee : ISteering
{
    private Transform _target;
    private Transform _entity;
    private float _maxSpeed;

    public Flee(Transform target, Transform entity, float maxSpeed)
    {
        _target = target;
        _entity = entity;
        _maxSpeed = maxSpeed;
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        // Direccion OPUESTA al target (huimos de el)
        Vector3 dir = _entity.position - _target.position;

        // Velocidad deseada: alejarse del target a maxima velocidad
        Vector3 desired = dir.normalized * _maxSpeed;

        // Steering = desired - velocidad actual (suaviza el giro)
        Vector3 steer = desired - currentSpeed;
        currentSpeed += steer * Time.deltaTime;

        return currentSpeed;
    }
}
