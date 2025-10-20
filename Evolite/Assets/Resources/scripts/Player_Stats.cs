using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static UnityEngine.ParticleSystem;
using Image = UnityEngine.UI.Image;

public class Player_Stats : MonoBehaviour
{
    public Player_Movement movement;
    public Sliders_Control healthControl;
    public Sliders_Control staminaControl;
    public Sliders_Control hungerControl;
    public Damage_Flash flash;
    public GameObject deathScreen;
    public Image deathImage;
    public bool isRunning;
    public bool isGrounded;
    public bool isDead;

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
    public bool tomouDanoNoFrame = false;
    private float hpNoFrame;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        healthControl = GameObject.Find("FillBarH").GetComponent<Sliders_Control>();
        staminaControl = GameObject.Find("FillBarS").GetComponent<Sliders_Control>();
        hungerControl = GameObject.Find("FillBarHu").GetComponent<Sliders_Control>();
        curHealth = maxHealth;
        curStamina = maxStamina;
        curHunger = maxHunger;
        hpNoFrame = curHealth;
        staminaRecovery = maxStamina / 10;
        deathScreen = GameObject.Find("Death Screen");
        deathScreen.SetActive(false);
        deathImage = deathScreen.transform.Find("You Died").GetComponent<Image>(); ;
        deathImage.color = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, 0);
    }

    // Update is called once per frame
    void Update()
    {
        tomouDanoNoFrame = false;

        if (curHealth != hpNoFrame)
        {
            tomouDanoNoFrame = true;
            hpNoFrame = curHealth; 
        }
    }


    void FixedUpdate()
    {
        gastouStaminaNoFrame = false;

        isRunning = movement.isRunning;
        isGrounded = movement.isGrounded;

        healthControl.SetMaxValue(maxHealth);
        staminaControl.SetMaxValue(maxStamina);

        

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
        staminaControl.SetValue(curStamina);
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
        if (!isDead)
        {
            curHealth -= damage;
            Limitador();
            if (curHealth <= 0)
            {
                isDead = true;
                Die();
            }
        }
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

    private void Die()
    {
        movement.isAbleToMove = false;
        deathScreen.SetActive(true);
        GameObject usableMenus = GameObject.Find("UsableMenus");
        usableMenus.SetActive(false);

        StartCoroutine(FadeDeathScreen());
    }

    private IEnumerator FadeDeathScreen()
    {
        Color startColor = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, 0);
        Color targetColor = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, 1);
        float duration = 1f; // segundos para o fade
        float timer = 0f;

        // começa totalmente transparente
        deathImage.color = startColor;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float t = timer / duration;
            deathImage.color = Color.Lerp(startColor, targetColor, t);
            deathScreen.GetComponent<Image>().color = deathImage.color;
            yield return null;
        }

        // garante que fica totalmente visível no final
        deathImage.color = targetColor;
        Invoke(nameof(Respawn), 2f);
    }

    public void Respawn()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);

        isDead = false;
        movement.isAbleToMove = true;

        curHealth = maxHealth;
        curStamina = maxStamina;
        curHunger = maxHunger;

        deathScreen.SetActive(false);

        GameObject usableMenus = GameObject.Find("UsableMenus");
        if (usableMenus != null)
            usableMenus.SetActive(true);

        deathImage.color = new Color(deathImage.color.r, deathImage.color.g, deathImage.color.b, 0);
        deathScreen.GetComponent<Image>().color = deathImage.color;
    }
}
