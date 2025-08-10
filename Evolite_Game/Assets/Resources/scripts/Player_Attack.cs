using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player_Attack : MonoBehaviour
{
    [Header("Ataque Básico")]
    public KeyCode baseAttackKey = KeyCode.Mouse0;
    public float baseAttackDmg = 10f;
    public float baseAttackSpeed = 0.5f; // Tempo entre ataques
    public bool ReadyToAttack = true;

    [Header("Comparadores")]
    public bool isAttacking;

    [Header("Componentes")]
    public CapsuleCollider attackCollider;
    public MeshRenderer meshRenderer;

    private List<Creature_Stats> enemiesInRange = new List<Creature_Stats>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackCollider.enabled = false;
        meshRenderer.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        AttackInput();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            if (enemy != null && !enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Creature"))
        {
            Creature_Stats enemy = other.GetComponent<Creature_Stats>();
            if (enemy != null && enemiesInRange.Contains(enemy))
            {
                enemiesInRange.Remove(enemy);
            }
        }
    }

    private void AttackInput()
    {
        if (Input.GetKey(baseAttackKey) && ReadyToAttack)
        {
            isAttacking = true;
            ReadyToAttack = false;

            attackCollider.enabled = true;
            meshRenderer.enabled = true;
            DealDamage();

            Invoke(nameof(DisableHitbox), 0.1f);
            Invoke(nameof(ResetAttack), baseAttackSpeed);
        }
    }

    private void DealDamage()
    {
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(baseAttackDmg);
            }
        }
    }

    private void DisableHitbox()
    {
        attackCollider.enabled = false;
        meshRenderer.enabled = false;

    }

    private void ResetAttack()
    {
        isAttacking = false;
        ReadyToAttack = true;
    }
}
