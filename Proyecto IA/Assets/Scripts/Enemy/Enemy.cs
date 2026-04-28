using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Componente Enemy: implementa IAttack usando LineOfSight para el rango de ataque.
/// Cuando golpea al jugador, invoca el Game Over (recarga la escena).
/// Hereda de Entity para obtener Rigidbody y movimiento base.
/// </summary>
public class Enemy : Entity, IAttack
{
    [Header("Ataque")]
    public LayerMask attackMask;

    [SerializeField]
    LineOfSight _attackOfSight;

    public float attackCooldownTime = 1.5f;

    private Action _onAttack;
    private Cooldown _attackCooldown;

    protected override void Awake()
    {
        base.Awake();
        _attackCooldown = new Cooldown(attackCooldownTime);
    }

    public float GetAttackRange => _attackOfSight.range;
    public Action OnAttack { get => _onAttack; set => _onAttack = value; }
    public Cooldown Cooldown => _attackCooldown;

    public void Attack()
    {
        // Buscamos colliders dentro del rango de ataque
        Collider[] colls = Physics.OverlapSphere(transform.position, _attackOfSight.range, attackMask);

        foreach (var item in colls)
        {
            var currTarget = item.transform;

            // Verificamos angulo y vision libre de obstaculos
            if (!_attackOfSight.CheckAngle(currTarget)) continue;
            if (!_attackOfSight.CheckView(currTarget))  continue;

            // Golpeamos al jugador: activamos Game Over
            Debug.Log("[Enemy] Jugador atrapado! GAME OVER.");
            GameOver();
            break;
        }

        _attackCooldown.ResetCooldown();
        _onAttack?.Invoke();
    }

    /// <summary>
    /// Termina el juego recargando la escena actual.
    /// Se puede reemplazar por una pantalla de Game Over personalizada.
    /// </summary>
    private void GameOver()
    {
        // Opcion 1: recargar la escena (simple para prototipo)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

        // Opcion 2 (descomentar si tienen GameOverManager):
        // GameOverManager.Instance.ShowGameOver();
    }
}
