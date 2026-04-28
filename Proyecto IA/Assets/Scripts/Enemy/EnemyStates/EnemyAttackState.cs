using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : State<StateEnum>
{
    IAttack _attack;

    public EnemyAttackState(IAttack attack)
    {
        _attack = attack;
    }
    public override void Enter()
    {
        base.Enter();
    }
    public override void Execute()
    {
        if (_attack.Cooldown == null || !_attack.Cooldown.IsCooldown())
        {
            _attack.Attack();
        }
        Debug.Log("ATTACK STATE EXECUTING");
        base.Execute();
    }
}
