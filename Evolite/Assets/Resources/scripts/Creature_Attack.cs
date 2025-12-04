using System.Collections.Generic;
using UnityEngine;

public class Creature_Attack : MonoBehaviour
{
    [Header("Ataque Básico")]
    public float baseAttackDmg = 10f;
    public float baseAttackSpeed = 0.5f; // Tempo entre ataques
    public float attackDelay;
    public bool ReadyToAttack = true;
    public float attackRange;

    [Header("Comparadores")]
    public bool isAttacking;

    [Header("Componentes")]
    public CapsuleCollider attackCollider;
    public Creature_General race;
    public Player_Movement plrMovement;

    public List<Creature_Stats> enemiesInRange = new List<Creature_Stats>();
    public Player_Stats playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        plrMovement = FindObjectOfType<Player_Movement>();
        attackCollider = GetComponent<CapsuleCollider>();
        race = GetComponentInParent<Creature_General>();
        attackCollider.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        // Detecta criaturas
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
                enemiesInRange.Add(enemy);
        }

        // Detecta o player
        if (other.CompareTag("Player"))
        {
            playerStats = other.GetComponent<Player_Stats>();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            if (enemy != null && enemiesInRange.Contains(enemy))
                enemiesInRange.Remove(enemy);
        }

        if (other.CompareTag("Player"))
        {
            playerStats = null;
        }
    }

    public void BaseAttack()
    {
        if (ReadyToAttack)
        {
            isAttacking = true;
            ReadyToAttack = false;

            Invoke(nameof(EnableHitBox), attackDelay);
            Invoke(nameof(DealDamage), attackDelay);
            Invoke(nameof(ResetAttack), baseAttackSpeed);
        }
    }

    public void DealDamage()
    {
        if (plrMovement.isInvulnerable) return;

        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
                enemy.TakeDamage(baseAttackDmg);
        }

        if (playerStats != null)
            playerStats.TakeDamage(baseAttackDmg);
    }

    public void DisableHitbox()
    {
    }
    public void EnableHitBox()
    {
        Invoke(nameof(DisableHitbox), 0.1f);
    }

    public void ResetAttack()
    {
        isAttacking = false;
        ReadyToAttack = true;
    }
}
