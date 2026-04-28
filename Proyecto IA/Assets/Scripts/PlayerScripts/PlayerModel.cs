using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel : Entity, ISpin
{

    bool _isDetectable = true;

    ////()=>()
    //public delegate void MyDelegate();
    //public delegate void MyDelegate2();
    //public MyDelegate OnSpin;
    //public Func<float> OnSpin3;

    Action _onSpin = delegate { };

   
    //M: Model
    //V: View
    //C: Controller

    public void Test()
    {

    }

    public void Spin()
    {
        _isDetectable = !_isDetectable;
        _onSpin();
    }
    public bool IsDetectable => _isDetectable;

    Action ISpin.OnSpin { get => _onSpin; set => _onSpin = value; }
}
