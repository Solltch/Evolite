using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public MeshRenderer meshRenderer;
    public Creature_General race;

    private List<Creature_Stats> enemiesInRange = new List<Creature_Stats>();
    private Player_Stats playerStats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        attackCollider = GetComponent<CapsuleCollider>();
        meshRenderer = GetComponent<MeshRenderer>();
        race = GetComponentInParent<Creature_General>();
        attackCollider.enabled = true;
        meshRenderer.enabled = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            Player_Stats candidate = other.GetComponent<Player_Stats>();
            if (candidate != null)
            {
                playerStats = candidate;
            }
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
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
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(baseAttackDmg);
                playerStats.TakeDamage(baseAttackDmg);
            }
        }
        isAttacking = false;
    }

    public void DisableHitbox()
    {
        meshRenderer.enabled = false;
    }
    public void EnableHitBox()
    {
        meshRenderer.enabled = true;
        Invoke(nameof(DisableHitbox), 0.1f);
    }

    public void ResetAttack()
    {

        ReadyToAttack = true;
    }
}
