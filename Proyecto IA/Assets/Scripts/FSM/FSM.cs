using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FSM<T>
{
    IState<T> _current;
    public Action<T, IState<T>, IState<T>> onTransition = delegate { };
    public FSM() { }
    public FSM(IState<T> init)
    {
        SetInitial(init);
    }
    public void SetInitial(IState<T> init)
    {
        _current = init;
        _current.Enter();
    }
    public void OnUpdate()
    {
        if (_current != null)
            _current.Execute();
    }
    public void OnFixedUpdate()
    {
        if (_current != null)
            _current.FixedExecute();
    }
    public void OnLateUpdate()
    {
        if (_current != null)
            _current.LateExecute();
    }
    public void Transition(T input)
    {
        IState<T> newState = _current.GetTransition(input);
        if (newState == null) return;
        var previousState = _current;
        _current.Exit();
        _current = newState;
        _current.Enter();
        onTransition(input, _current, previousState);
    }
    public IState<T> GetCurrent => _current;
}
