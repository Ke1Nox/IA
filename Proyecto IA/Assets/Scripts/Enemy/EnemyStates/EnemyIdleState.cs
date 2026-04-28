using UnityEngine;

/// <summary>
/// Estado Idle del enemigo.
/// El NPC se detiene durante un tiempo determinado antes de volver a Patrol.
/// Es activado automaticamente tras cierta cantidad de iteraciones en el estado Patrol.
/// Si durante el Idle detecta al jugador, transiciona a Chase inmediatamente.
/// </summary>
public class EnemyIdleState : State<StateEnum>
{
    private float _idleDuration;  // Cuanto tiempo permanece en Idle
    private float _timer;         // Temporizador interno

    public EnemyIdleState(float idleDuration = 3f)
    {
        _idleDuration = idleDuration;
    }

    public override void Enter()
    {
        base.Enter();
        // Reiniciamos el timer cada vez que entramos al estado
        _timer = 0f;
        Debug.Log("[EnemyIdleState] Entro en Idle. Esperando " + _idleDuration + "s.");
    }

    public override void Execute()
    {
        base.Execute();
        // Contamos el tiempo transcurrido en Idle
        _timer += Time.deltaTime;
    }

    /// <summary>
    /// Indica si ya paso el tiempo suficiente para salir del Idle.
    /// El EnemyController consulta esto para decidir la transicion a Patrol.
    /// </summary>
    public bool IsIdleFinished => _timer >= _idleDuration;
}
