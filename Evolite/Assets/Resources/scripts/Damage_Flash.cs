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

    private void Awake()
    {
        if (!isPlayer)
        {
            statsCreat = transform.parent.GetComponentInChildren<Creature_Stats>();
        }
        else
        {
            statsPlayer = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
        }

        rend = GetComponent<SpriteRenderer>();
        originalMaterial = rend.sharedMaterial;
    }

    private void Update()
    {
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

        // troca temporariamente pelo material de flash
        rend.sharedMaterial = flashMaterial;
        flashMaterial.SetColor("_FlashColor", flashColor);

        float timer = 0f;

        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            float currentAmount = Mathf.Lerp(1f, 0f, timer / flashTime);
            flashMaterial.SetFloat("_FlashAmount", currentAmount);
            yield return null;
        }

        // volta pro material original
        rend.sharedMaterial = originalMaterial;
        isFlashing = false;
    }
}
