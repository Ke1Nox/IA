using UnityEngine;

/// <summary>
/// Enumeracion de todos los estados posibles para la FSM.
/// Usada tanto por enemigos como por el jugador y la camara de seguridad.
/// </summary>
public enum StateEnum
{
    Idle,
    Patrol,
    Move,
    Spin,
    Alert,
    Attack,
    Chase,
    RunAway
}

/// <summary>
/// Define el comportamiento del grupo de enemigos.
/// Aggressive: prefiere perseguir al jugador (70% Chase).
/// Coward: prefiere huir del jugador (70% RunAway).
/// </summary>
public enum EnemyGroup
{
    Aggressive,
    Coward
}
