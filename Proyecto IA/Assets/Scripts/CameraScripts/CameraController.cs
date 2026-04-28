using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CameraController : MonoBehaviour
{
    public Transform target;
    LineOfSight _los;
    IAlert _alert;
    ISpin _spin;
    FSM<StateEnum> _fsm;
    ITreeNode _root;
    private void Awake()
    {
        _los = GetComponent<LineOfSight>();
        _alert = GetComponent<IAlert>();
        _spin = target.GetComponent<ISpin>();
    }
    private void Start()
    {
        InitializedFSM();
        InitializedTree();
    }
    void InitializedFSM()
    {
        _fsm = new FSM<StateEnum>();
        var idle = new CameraStateIdle();
        var alert = new CameraStateAlert(_alert);

        idle.AddTransition(StateEnum.Alert, alert);
        alert.AddTransition(StateEnum.Idle, idle);

        _fsm.SetInitial(idle);
    }
    void InitializedTree()
    {
        var alert = new ActionTree(() => _fsm.Transition(StateEnum.Alert));
        var idle = new ActionTree(() => _fsm.Transition(StateEnum.Idle));

        var qInView = new QuestionTree(InView, alert, idle);
        var qIsExist = new QuestionTree(() => target != null, qInView, idle);

        _root = qIsExist;
    }
    public bool InView()
    {
        return (_spin == null || _spin.IsDetectable) &&
            _los.CheckRange(target) &&
            _los.CheckAngle(target) &&
            _los.CheckView(target);
    }
    void Update()
    {
        _fsm.OnUpdate();
        _root.Execute();
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
