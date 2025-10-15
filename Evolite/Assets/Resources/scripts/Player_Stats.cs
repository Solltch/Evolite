using UnityEngine;

public class Player_Stats : MonoBehaviour
{
    public Player_Movement movement;
    public Sliders_Control healthControl;
    public Sliders_Control staminaCcontrol;
    public Sliders_Control hungerControl;
    public Damage_Flash flash;
    public bool isRunning;
    public bool isGrounded;

    public float maxHealth;
    public float curHealth;
    public float maxStamina;
    public float curStamina;
    public float maxHunger;
    public float curHunger;

    public float runCost;
    public float jumpCost;
    public float staminaRecovery;
    public float restDelay;
    public float hungerDecaySpeed;
    public bool isExhausted;

    public float DNA;

    public float restTimer;
    private bool gastouStaminaNoFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
        curStamina = maxStamina;
        curHunger = maxHunger;
    }

    // Update is called once per frame
    void Update()
    {
    }


    void FixedUpdate()
    {
        gastouStaminaNoFrame = false;

        isRunning = movement.isRunning;
        isGrounded = movement.isGrounded;
        staminaRecovery = maxStamina / 10;

        healthControl.SetMaxValue(maxHealth);
        staminaCcontrol.SetMaxValue(maxStamina);

        

        if (isRunning)
        {
            curStamina -= runCost * Time.fixedDeltaTime;
            gastouStaminaNoFrame = true;
        }
        else
            gastouStaminaNoFrame = false;


        if (gastouStaminaNoFrame)
        {
            restTimer = 0;
        }
        else
        {
            restTimer += Time.fixedDeltaTime;
        }

        Rest();
        Fome();

        healthControl.SetValue(curHealth);
        staminaCcontrol.SetValue(curStamina);
        hungerControl.SetValue(curHunger);

        isExhausted = curStamina <= 0.01f;

    }

    public void JumpCost()
    {
        curStamina -= jumpCost;
        gastouStaminaNoFrame = true;
        return;
    }

    private void Rest()
    {
        if (restTimer > restDelay && curStamina < maxStamina)
        {

            curStamina += staminaRecovery * Time.fixedDeltaTime;
        }
        Limitador();
    }

    public void TakeDamage(float damage)
    {
        curHealth -= damage;
        Limitador();
        flash.Flash();
    }

    private void Limitador()
    {
        curStamina = Mathf.Clamp(curStamina, 0, maxStamina);
        curHealth = Mathf.Clamp(curHealth, 0, maxHealth);
        curHunger = Mathf.Clamp(curHunger, 0, maxHunger);
    }

    private void Fome()
    {
        curHunger -= hungerDecaySpeed * Time.fixedDeltaTime;
        Limitador();
    }
}
