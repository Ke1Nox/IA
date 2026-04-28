using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateIdle<T> : State<T>
{
    FSM<T> _fsm;
    T _inputToMove;
    T _inputToSpin;
    IMove _move;

    public PlayerStateIdle(FSM<T> fsm, T inputToMove, T inputToSpin, IMove move)
    {
        _fsm = fsm;
        _inputToMove = inputToMove;
        _inputToSpin = inputToSpin;
        _move = move;
    }

    public override void Enter()
    {
        base.Enter();
        _move.Move(Vector3.zero);
    }
    public override void Execute()
    {
        base.Execute();
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");

        if (h != 0 || v != 0)
        {
            _fsm.Transition(_inputToMove);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _fsm.Transition(_inputToSpin);
        }
    }
}
