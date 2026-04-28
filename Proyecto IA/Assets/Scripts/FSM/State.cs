using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State<T> : IState<T>
{
    Dictionary<T, IState<T>> _transitions = new Dictionary<T, IState<T>>();
    public virtual void Enter()
    {

    }
    public virtual void Execute()
    {

    }
    public virtual void FixedExecute()
    {

    }
    public virtual void LateExecute()
    {

    }
    public virtual void Exit()
    {

    }
    public void AddTransition(T input, IState<T> state)
    {
        _transitions[input] = state;
    }
    public void RemoveTransition(T input)
    {
        if (_transitions.ContainsKey(input))
            _transitions.Remove(input);
    }
    public void RemoveTransition(IState<T> state)
    {
        foreach (var item in _transitions)
        {
            if (state == item.Value)
            {
                _transitions.Remove(item.Key);
                break;
            }
        }
    }
    public IState<T> GetTransition(T input)
    {
        if (!_transitions.ContainsKey(input)) return null;
        return _transitions[input];
    }
}
