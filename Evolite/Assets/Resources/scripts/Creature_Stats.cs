using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MovementState = Creature_General.MovementState;

public class Creature_Stats : MonoBehaviour
{
    [Header("Referências")]
    public ParticleSystem particles;
    public Damage_Flash flash;
    public Transform sprite;
    public NavMeshAgent agent;
    public Creature_Attack attack;
    public Creature_General general;

    [Header("Status")]
    public bool isRunning;
    public bool isGrounded;
    public bool isDead;
    public State type;
    public enum State { carni, herbi, oni }
    public float courage;

    [Header("Vida e Stamina")]
    public float maxHealth;
    public float curHealth;
    public float maxStamina;
    public float curStamina;

    [Header("Veneno")]
    public float poisonTickDamage = 1f; // Dano por tick do veneno
    public float poisonTickInterval = 1f; // Intervalo entre ticks de dano
    private float poisonEndTime; // Tempo em que o veneno irá parar
    public bool isPoisoned = false;
    private Coroutine poisonCoroutine; // Para controlar a rotina de dano

    [Header("Stamina e Movimento")]
    public float runCost = 5f;
    public float staminaRecovery;
    public float restDelay = 1f;
    public bool isExhausted;

    private float restTimer;
    private bool gastouStaminaNoFrame = false;
    public bool tomouDanoNoFrame = false;
    public bool justTookDamage = false;

    private void Awake()
    {
        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();

        if (general == null)
            general = GetComponentInParent<Creature_General>();

        if (attack == null)
            attack = GetComponentInChildren<Creature_Attack>();

        if (sprite == null && transform.parent != null)
            sprite = transform.parent.Find("Dummy_Sprite");

        if (flash == null && sprite != null)
            flash = sprite.GetComponent<Damage_Flash>();

        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();

        curHealth = maxHealth;
        curStamina = maxStamina;
        staminaRecovery = maxStamina / 10f;

    }


    private void Update()
    {

        if (particles != null)
        {
            if (general.moveState == MovementState.running && !particles.isPlaying)
            {
                isRunning = true;
                particles.Play();
            }
            else if (general.moveState != MovementState.running && particles.isPlaying)
            {
                isRunning = false;
                particles.Stop();
            }
        }


        if (isPoisoned && Time.time >= poisonEndTime)
        {
            isPoisoned = false;
            // Opcional: Efeito visual/sonoro de fim de veneno
        }

    }

    private void FixedUpdate()
    {
        gastouStaminaNoFrame = false;

        if (isRunning && !isExhausted)
        {
            curStamina -= runCost * Time.fixedDeltaTime;
            gastouStaminaNoFrame = true;
        }

        if (gastouStaminaNoFrame)
            restTimer = 0f;
        else
            restTimer += Time.fixedDeltaTime;

        Rest();

        if (curStamina <= 0.01f)
            isExhausted = true;
        else if (Mathf.Approximately(curStamina, maxStamina))
            isExhausted = false;

        Limitador();
    }

    private void Rest()
    {
        if (restTimer > restDelay && curStamina < maxStamina)
            curStamina += staminaRecovery * Time.fixedDeltaTime;
    }

    public void ApplyPoison(float duration)
    {
        // Se já estiver envenenado, reinicia a duração e o dano.
        if (isPoisoned)
        {
            // Se já há uma rotina rodando, paramos para evitar duplicidade
            if (poisonCoroutine != null)
            {
                StopCoroutine(poisonCoroutine);
            }
        }

        isPoisoned = true;

        // Define o tempo final do veneno
        poisonEndTime = Time.time + duration;

        // Inicia a rotina de dano por tick
        poisonCoroutine = StartCoroutine(PoisonDamageRoutine(duration));
    }

    // NOVO: Coroutine para aplicar dano por tick
    private IEnumerator PoisonDamageRoutine(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            // Aplica o dano do veneno (diretamente, sem chamar TakeDamage para evitar loop)
            curHealth -= poisonTickDamage;
            Limitador(); // Garante que a vida não exceda os limites

            // Opcional: Efeito visual/sonoro de tick de dano

            if (curHealth <= 0f)
            {
                Die();
                yield break; // Sai da coroutine se a criatura morrer
            }

            timer += poisonTickInterval;
            yield return new WaitForSeconds(poisonTickInterval);
        }

        // Força a desativação se a duração terminar
        isPoisoned = false;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        Limitador();
        general.SetHumor();

        Debug.Log("Inimigo Apanhou");

        // Mantemos a flag para o caso de outros sistemas precisarem saber
        justTookDamage = true;

        // A chamada direta é a forma mais segura de garantir o flash:
        if (flash != null)
        {
            // Se a coroutine já estiver rodando, o StartCoroutine tenta rodar uma nova,
            // mas o Damage_Flash usa a flag 'isFlashing' para se proteger contra isso.
            StartCoroutine(flash.FlashCoroutine());
        }

        if (curHealth <= 1f)
            Die();
    }

    private void Limitador()
    {
        curStamina = Mathf.Clamp(curStamina, 0f, maxStamina);
        curHealth = Mathf.Clamp(curHealth, 0f, maxHealth);
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        // 1. DESATIVAÇÃO DE MOVIMENTO/LÓGICA
        if (general != null)
            general.enabled = false;

        if (attack != null)
            attack.enabled = false;

        if (agent != null)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.enabled = false; // Desativa o NavMeshAgent para parar completamente
        }

        // 2. EFEITOS E PARTICULAS
        if (particles != null)
        {
            // Para as partículas e desativa o GameObject delas
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.gameObject.SetActive(false);
        }

        // 3. MANIPULAÇÃO VISUAL (SPRITE)
        if (sprite != null)
        {
            // OPÇÃO 1: Desativar todos os componentes SpriteRenderer nos filhos
            GameObject spriteParts = sprite.GetChild(0).gameObject;
            spriteParts.SetActive(false);

            sprite.GetComponent<SpriteRenderer>().enabled = true;
        }

        // 4. TRANSFORMAÇÃO EM ITEM INTERAGÍVEL
        // Adiciona o componente de interação e o configura
        if (GetComponent<InteractFunctions>() == null)
        {
            InteractFunctions interact = gameObject.AddComponent<InteractFunctions>();
            interact.isFood = true;
            interact.isMeat = true;
            interact.action = "Devorar";
        }

        // Altera a Tag para Interagível
        gameObject.tag = "Interactable";

        // OPÇÃO 2: Se você quiser desativar o GameObject que segura a criatura (parent)
        // Desativar o parent pode quebrar outras referências. É melhor desativar a lógica e o visual.
    }
}
