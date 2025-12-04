using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player_Attack : MonoBehaviour
{
    [Header("Ataque Básico")]
    public KeyCode baseAttackKey = KeyCode.Mouse0;
    public float baseAttackDmg = 10f;
    public float baseAttackSpeed = 0.5f;
    public float attackDelay = 0.1f; // Garantir que tenha um valor inicial para Invoke
    public bool ReadyToAttack = true;

    [Header("Ataque de Mordida")]
    public KeyCode biteAttackKey = KeyCode.E;
    public float biteAttackDmg = 25f;
    public float biteCooldown = 2f;
    public float biteDelay = 0.2f;
    public bool ReadyToBite = true;
    public bool isBiting;

    [Header("Comparadores")]
    public bool isAttacking;

    [Header("Componentes")]
    public CapsuleCollider attackCollider;
    public Player_General plr;
    public SkillCoolDown biteCooldownUI;

    private List<Creature_Stats> enemiesInRange = new List<Creature_Stats>();

    void Start()
    {
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        attackCollider.enabled = true;
        ReadyToBite = true; // Garante que a mordida está pronta no início
    }

    void Update()
    {
        AttackInput();
        BiteInput(); // << CHAMAR NOVO INPUT
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

    // --- Lógica de Ataque Básico ---
    private void AttackInput()
    {
        if (Input.GetKey(baseAttackKey) && ReadyToAttack && !isBiting)
        {
            isAttacking = true;
            ReadyToAttack = false;

            StartCoroutine(LockMovementDuringAttack(baseAttackSpeed));

            Invoke(nameof(DealBaseDamage), attackDelay);

            Invoke(nameof(EndAttackState), attackDelay + 0.1f);

            Invoke(nameof(ResetAttack), baseAttackSpeed);
        }
    }

    private IEnumerator LockMovementDuringAttack(float duration)
    {
        // 1. Bloqueia o movimento imediatamente
        plr.stats.isAbleToMove = false;
        plr.stats.rb.linearVelocity = Vector3.zero;

        // 2. Espera pelo tempo de duração total do ciclo (cooldown do ataque)
        yield return new WaitForSeconds(duration);

        // 3. Libera o movimento.
        if(!Input.GetKey(baseAttackKey))
            ResetMovement();
    }

    public void ResetMovement()
    {
        plr.stats.ResetMovement();
    }

    private void DealBaseDamage()
    {
        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(baseAttackDmg);

                if (plr.skills.Veneno)
                {
                    enemy.ApplyPoison(3f);
                }
            }
        }
        isAttacking = false;
    }

    private void ResetAttack()
    {
        ReadyToAttack = true;
    }

    // --- Lógica de Ataque de Mordida (Bite) ---
    private void BiteInput()
    {
        if (Input.GetKeyDown(biteAttackKey) && ReadyToBite && !isAttacking && plr.skills.Presas)
        {
            Bite();
        }
    }

    private void Bite()
    {
        isBiting = true;
        ReadyToBite = false;

        if (biteCooldownUI != null)
        {
            biteCooldownUI.StartCooldown(biteCooldown);
        }

        // O dano de MORDA é causado após o delay
        Invoke(nameof(DealBiteDamage), biteDelay);

        // Reseta o estado isAttacking (visível) após o tempo de animação (por exemplo, 0.2s)
        Invoke(nameof(EndAttackState), biteDelay + 0.1f);

        // Cooldown total da Mordida
        Invoke(nameof(ResetBiteCooldown), biteCooldown);
    }

    private void DealBiteDamage()
    {
        float finalDamage = biteAttackDmg;

        // Aplica o bônus de dano de Mordida se a skill 'Garras' estiver ativa
        if (plr.skills.Garras)
        {
            finalDamage *= 2f; // Exemplo: Duplica o dano
        }

        foreach (var enemy in enemiesInRange)
        {
            if (enemy != null)
            {
                enemy.TakeDamage(finalDamage);
            }
        }
    }

    private void EndAttackState()
    {
        isAttacking = false;
        isBiting = false;
    }

    private void ResetBiteCooldown()
    {
        ReadyToBite = true;
    }

    // --- Hitbox Control (Compartilhado) ---
    /*private void DisableHitbox()
    {
        meshRenderer.enabled = false;
    }

    private void EnableHitBox()
    {
        meshRenderer.enabled = true;
        // Tempo de exposição da hitbox
        Invoke(nameof(DisableHitbox), 0.1f);
    }*/
}