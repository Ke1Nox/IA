using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISpin
{
    Action OnSpin { get; set; }
    void Spin();
    bool IsDetectable { get; }
}
