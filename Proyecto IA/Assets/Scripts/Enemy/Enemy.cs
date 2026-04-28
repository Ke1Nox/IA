using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Entity, IAttack
{
    public LayerMask attackMask;
    [SerializeField]
    LineOfSight _attackOfSight;
    Action _onAttack;
    public float attackCooldownTime;
    Cooldown _attackCooldown;
    protected override void Awake()
    {
        base.Awake();
        _attackCooldown = new Cooldown(attackCooldownTime);
    }
    public float GetAttackRange => _attackOfSight.range;

    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Cooldown Cooldown { get => _attackCooldown; }

    //Collider[] _enemies = new Collider[5];
    public void Attack()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, _attackOfSight.range, attackMask);
        //int count = Physics.OverlapSphereNonAlloc(transform.position, _attackOfSight.range, _enemies, attackMask);
        foreach (var item in colls)
        {
            var currTarget = item.transform;
            if (!_attackOfSight.CheckAngle(currTarget)) continue;
            if (!_attackOfSight.CheckView(currTarget)) continue;
            //
            Destroy(item.gameObject);
            break;
        }
        _attackCooldown.ResetCooldown();
        _onAttack?.Invoke();
    }
}
