using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Persuit : ISteering
{
    private Transform _target;
    private Transform _entity;
    private Rigidbody playerRB;
    private float maxSpeed;

    public Persuit(Transform target, Transform entity, float maxSpeed)
    {
        _target = target;
        _entity = entity;
        this.maxSpeed = maxSpeed;
        playerRB = _entity.GetComponent<Rigidbody>();
    }

    public Vector3 GetDir(Vector3 currentSpeed)
    {
        var toQuarry = _target.position - _entity.position;
        var distance = toQuarry.magnitude;
        var c = 2f;
        float t = distance * c;
        var pForward = _entity.forward;
        var qForward = _target.forward;
        var relativeHeading = Vector3.Dot(pForward, qForward);
        var toPursuer = (_entity.position - _target.position).normalized;
        var forwardDot = Vector3.Dot(qForward, toPursuer);
        //
        if (forwardDot > 0 && relativeHeading < -0.95f)
        {
            t = 0;
        }
        else
        {
            if (relativeHeading < 0)
            {
                t = 1.5f;
            }
            if (forwardDot < 0)
            {
                t = 1.2f;
            }
        }
        var futurePosition = _target.position + playerRB.velocity * t;
        var dir = futurePosition - _entity.position;
        var desired = dir.normalized * maxSpeed;
        var steer = desired - currentSpeed;
        currentSpeed += steer * Time.deltaTime;

        return currentSpeed;
    }
}
