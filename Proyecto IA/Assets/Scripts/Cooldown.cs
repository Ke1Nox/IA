using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cooldown
{
    float _timer;
    float _initTimer;
    Action _onFinishCooldown;
    public Cooldown(float timer = 1, bool isInCooldown = false, Action onFinishCooldown = null)
    {
        SetTimer(timer, isInCooldown, onFinishCooldown);
    }
    public void ResetCooldown()
    {
        _timer = _initTimer;
    }
    public void SetTimer(float timer = 1, bool isInCooldown = false, Action onFinishCooldown = null)
    {
        _timer = isInCooldown ? timer : 0;
        _initTimer = timer;
        _onFinishCooldown = onFinishCooldown;
    }
    public bool IsCooldown()
    {
        RunCooldown();
        return _timer > 0;
    }
    public void RunCooldown()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0 && _onFinishCooldown != null)
        {
            _onFinishCooldown();
        }
    }
    public Action OnFinishCooldown
    {
        get
        {
            return _onFinishCooldown;
        }
        set
        {
            _onFinishCooldown = value;
        }
    }
}
