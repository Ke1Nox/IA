using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionTree : ITreeNode
{
    Action _action;
    public ActionTree(Action action)
    {
        _action = action;
    }
    public void Execute()
    {
        _action();
    }
}
