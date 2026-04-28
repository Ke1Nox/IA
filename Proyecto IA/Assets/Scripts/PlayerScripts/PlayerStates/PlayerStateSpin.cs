using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateSpin : State<StateEnum>
{
    ISpin _spin;
    FSM<StateEnum> _fsm;
    public PlayerStateSpin(FSM<StateEnum> fsm, ISpin spin)
    {
        _spin = spin;
        _fsm = fsm;
    }
    public override void Enter()
    {
        base.Enter();
        _spin.Spin();
    }
    public override void Execute()
    {
        base.Execute();
        if (Input.GetKeyDown(KeyCode.Space) || _spin.IsDetectable)
        {
            _fsm.Transition(StateEnum.Idle);
        }
    }
    public override void Exit()
    {
        base.Exit();
        _spin.Spin();
    }
}
