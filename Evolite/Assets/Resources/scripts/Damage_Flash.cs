using System.Collections;
using UnityEngine;

public class Damage_Flash : MonoBehaviour
{
    [Header("Configurações de Flash")]
    public Color flashColor = Color.white;
    public float flashTime = 0.2f;

    [Header("Referências")]
    public SpriteRenderer rend;
    public Material flashMaterial;
    public bool isPlayer;
    public Creature_Stats statsCreat;
    public Player_Stats statsPlayer;

    private Material originalMaterial;
    private bool isFlashing = false;
    private Material loadedMat;

    private void Awake()
    {
        if (!isPlayer)
        {
            statsCreat = transform.parent.GetComponentInChildren<Creature_Stats>();
            statsPlayer = null;
        }
        else
        {
            statsPlayer = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
            statsCreat = null;
        }

        rend = GetComponent<SpriteRenderer>();
        originalMaterial = rend.material;
    }

    private void Update()
    {
        // Detecta dano e inicia flash se necessário
        if (!isFlashing)
        {
            if (!isPlayer && statsCreat != null && statsCreat.tomouDanoNoFrame)
                StartCoroutine(FlashCoroutine());
            else if (isPlayer && statsPlayer != null && statsPlayer.tomouDanoNoFrame)
                StartCoroutine(FlashCoroutine());
        }
    }

    private IEnumerator FlashCoroutine()
    {
        isFlashing = true;
        rend.material = flashMaterial;
        flashMaterial.SetColor("_FlashColor", flashColor);
        rend.receiveShadows = false;

        float timer = 0f;

        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            float currentAmount = Mathf.Lerp(1f, 0f, timer / flashTime);
            flashMaterial.SetFloat("_FlashAmount", currentAmount);
            yield return null; // espera o próximo frame
        }

        rend.material = originalMaterial;
        rend.receiveShadows = true;
        isFlashing = false;
    }
}