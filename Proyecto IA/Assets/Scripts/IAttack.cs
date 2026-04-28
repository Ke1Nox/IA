using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    void Attack();
    float GetAttackRange { get; }
    Action OnAttack { get; set; }
    Cooldown Cooldown { get;}
}
