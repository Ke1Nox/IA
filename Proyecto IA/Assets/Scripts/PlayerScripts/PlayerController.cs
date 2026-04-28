using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    IMove _move;
    ISpin _spin;
    FSM<StateEnum> _fsm;
    void Start()
    {
        _move = GetComponent<IMove>();
        _spin = GetComponent<ISpin>();
        InitializedFSM();
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var idle = new PlayerStateIdle<StateEnum>(_fsm, StateEnum.Move, StateEnum.Spin, _move);
        var move = new PlayerStateMove(_fsm, _move);
        var spin = new PlayerStateSpin(_fsm, _spin);

        idle.AddTransition(StateEnum.Move, move);
        idle.AddTransition(StateEnum.Spin, spin);

        move.AddTransition(StateEnum.Idle, idle);
        move.AddTransition(StateEnum.Spin, spin);

        spin.AddTransition(StateEnum.Idle, idle);


        _fsm.SetInitial(idle);
    }
    void Update()
    {
        _fsm.OnUpdate();
        //if (_spin.IsDetectable)
        //{
        //    var h = Input.GetAxis("Horizontal");
        //    var v = Input.GetAxis("Vertical");

        //    Vector3 dir = new Vector3(h, 0, v);
        //    _move.Move(dir.normalized);
        //    if (h != 0 || v != 0) _move.Look(dir);
        //}
        //if (Input.GetKeyDown(KeyCode.Space)) _spin.Spin();
    }
    private void FixedUpdate()
    {
        _fsm.OnFixedUpdate();
    }
    private void LateUpdate()
    {
        _fsm.OnLateUpdate();
    }
}
