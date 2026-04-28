using UnityEngine;

/// <summary>
/// Evade: Steering behaviour complejo de huida.
/// A diferencia de Flee (que huye de la posicion actual del target),
/// Evade predice la posicion FUTURA del target y huye de ahi.
/// Esto hace que el agente anticipe el movimiento del perseguidor,
/// evitando ser atrapado aunque el perseguidor intente cortarle el paso.
/// </summary>
public class Evade : ISteering
{
    private Transform _target;
    private Transform _entity;
    private float _maxSpeed;
    private Rigidbody _targetRB;

    public Evade(Transform target, Transform entity, float maxSpeed)
    {
        _target = target;
        _entity = entity;
        _maxSpeed = maxSpeed;
        // Necesitamos el Rigidbody del target para predecir su velocidad
        _targetRB = _target.GetComponent<Rigidbody>();
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        Vector3 toTarget = _target.position - _entity.position;
        float distance = toTarget.magnitude;

        // Cuanto mas lejos este el target, mas tiempo predecimos hacia adelante
        float predictionTime = distance / _maxSpeed;

        // Posicion futura estimada del target
        Vector3 futurePosition = _target.position;
        if (_targetRB != null)
            futurePosition += _targetRB.velocity * predictionTime;

        // Huimos de la posicion futura (no de la actual)
        Vector3 dir = _entity.position - futurePosition;
        Vector3 desired = dir.normalized * _maxSpeed;

        Vector3 steer = desired - currentSpeed;
        currentSpeed += steer * Time.deltaTime;

        return currentSpeed;
    }
}
