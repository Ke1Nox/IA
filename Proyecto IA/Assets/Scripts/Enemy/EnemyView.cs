using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyView : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    IAttack _attack;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _attack = GetComponent<IAttack>();
    }
    private void Start()
    {
        _attack.OnAttack += OnAttackAnim;
    }
    private void Update()
    {
        _anim.SetFloat("Vel", _rb.velocity.magnitude);
    }
    void OnAttackAnim()
    {
        _anim.SetTrigger("Attack");
    }
}
