using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionTest : QuestionTree
{
    public QuestionTest(ITreeNode tNode, ITreeNode fNode) : base(Test, tNode, fNode)
    {
    }
    static bool Test()
    {
        return true;
    }
}
