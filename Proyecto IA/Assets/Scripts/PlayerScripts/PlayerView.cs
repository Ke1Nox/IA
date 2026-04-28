using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField]
    Animator _anim;
    Rigidbody _rb;
    ISpin _spin;
    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _spin = GetComponent<ISpin>();
        _spin.OnSpin += OnSpinAnimation;
        OnSpinAnimation();
    }
    private void Update()
    {
        if (_spin.IsDetectable)
        {
            _anim.SetFloat("Vel", _rb.velocity.magnitude);
        }
        else
        {
            _anim.SetFloat("Vel", 0);

        }
    }
    void OnSpinAnimation()
    {
        _anim.SetBool("IsDetectable", _spin.IsDetectable);
    }
}
