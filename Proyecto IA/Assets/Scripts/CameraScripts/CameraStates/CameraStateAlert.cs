using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStateAlert : State<StateEnum>
{
    IAlert _alert;
    public CameraStateAlert(IAlert alert)
    {
        _alert = alert;
    }
    public override void Enter()
    {
        base.Enter();
        _alert.IsAlert = true;
        Debug.Log("ALERT ENTER");
    }
    public override void Exit()
    {
        base.Exit();
        _alert.IsAlert = false;
    }
}
