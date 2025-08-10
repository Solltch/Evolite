using UnityEngine;

public class Creature_Stats : MonoBehaviour
{
    public Test_Movement movement;
    public Sliders_Control healthControl;
    public Sliders_Control staminaCcontrol;
    public Damage_Flash flash;
    public bool isRunning;
    public bool isGrounded;

    public float maxHealth;
    public float curHealth;
    public float maxStamina;
    public float curStamina;

    public float runCost;
    public float jumpCost;
    public float staminaRecovery;
    public float restDelay;
    public bool isExhausted;

    public float restTimer;
    private bool gastouStaminaNoFrame = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        curHealth = maxHealth;
        curStamina = maxStamina;
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

        healthControl.SetValue(curHealth);
        staminaCcontrol.SetValue(curStamina);

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
    }
}
