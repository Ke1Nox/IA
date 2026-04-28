using System;
using UnityEngine;

/// <summary>
/// Interfaz de ataque para entidades que pueden atacar.
/// Expone el rango, cooldown y el evento de ataque.
/// </summary>
public interface IAttack
{
    float GetAttackRange { get; }
    Action OnAttack { get; set; }
    Cooldown Cooldown { get; }
    void Attack();
}
