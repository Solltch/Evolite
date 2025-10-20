using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

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

    [Header("Stamina e Movimento")]
    public float runCost = 5f;
    public float staminaRecovery;
    public float restDelay = 1f;
    public bool isExhausted;

    private float restTimer;
    private bool gastouStaminaNoFrame = false;
    public bool tomouDanoNoFrame = false;
    private float hpNoFrame;

    private void Awake()
    {
        // Referências seguras
        if (particles == null)
            particles = GetComponentInChildren<ParticleSystem>();

        if (flash == null && sprite != null)
            flash = sprite.GetComponent<Damage_Flash>();

        if (agent == null)
            agent = GetComponentInParent<NavMeshAgent>();

        if (sprite == null && transform.parent != null)
            sprite = transform.parent.Find("Dummy_Sprite");

        if (attack == null)
            attack = GetComponentInChildren<Creature_Attack>();

        if (general == null)
            general = GetComponentInParent<Creature_General>();

        // Inicialização de valores
        curHealth = maxHealth;
        curStamina = maxStamina;
        staminaRecovery = maxStamina / 10f;
        hpNoFrame = curHealth;
    }

    private void Update()
    {
        // Verifica se tomou dano
        tomouDanoNoFrame = !Mathf.Approximately(hpNoFrame, curHealth);
        hpNoFrame = curHealth;

        // Controla partículas
        if (particles != null)
        {
            if (isRunning && !particles.isPlaying)
                particles.Play();
            else if (!isRunning && particles.isPlaying)
                particles.Stop();
        }

        // Rotaciona o sprite para acompanhar o pai
        if (sprite != null)
            sprite.rotation = Quaternion.Euler(sprite.rotation.eulerAngles.x, transform.parent.rotation.eulerAngles.y, sprite.rotation.eulerAngles.z);
    }

    private void FixedUpdate()
    {
        gastouStaminaNoFrame = false;

        // Aqui você deve definir isRunning e isGrounded baseado na entrada ou IA
        // Exemplo: isRunning = agent.velocity.magnitude > 0.1f;
        // Exemplo: isGrounded = true; // placeholder

        // Gasto de stamina
        if (isRunning && !isExhausted)
        {
            curStamina -= runCost * Time.fixedDeltaTime;
            gastouStaminaNoFrame = true;
        }

        // Controle de descanso
        if (gastouStaminaNoFrame)
            restTimer = 0f;
        else
            restTimer += Time.fixedDeltaTime;

        Rest();

        // Verifica exaustão
        isExhausted = curStamina <= 0.01f;

        Limitador();
    }

    private void Rest()
    {
        if (restTimer > restDelay && curStamina < maxStamina)
            curStamina += staminaRecovery * Time.fixedDeltaTime;
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        Limitador();

        if (curHealth <= 0f)
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

        // Para partículas completamente
        if (particles != null)
            particles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            particles.gameObject.SetActive(false);

        transform.parent.localRotation = Quaternion.Euler(transform.parent.localRotation.eulerAngles.x, transform.parent.localRotation.eulerAngles.y, 90f);

        // Desativa NavMeshAgent
        if (agent != null)
        {
            agent.ResetPath();
            agent.velocity = Vector3.zero;
            agent.enabled = false;
        }

        // Desativa ataque
        if (attack != null)
            attack.enabled = false;

        // Desativa IA/movimento
        if (general != null)
            general.enabled = false;

        // Adiciona InteractFunctions de forma segura
        if (GetComponent<InteractFunctions>() == null)
        {
            InteractFunctions interact = gameObject.AddComponent<InteractFunctions>();
            interact.isFood = true;
        }

        // Define a tag
        gameObject.tag = "Interactable";
    }
}
