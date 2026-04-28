using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class CameraStateIdle : State<StateEnum>
{
    public override void Enter()
    {
        base.Enter();
        Debug.Log("IDLE ENTER");
    }
}
