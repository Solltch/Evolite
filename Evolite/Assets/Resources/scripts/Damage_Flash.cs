using System.Collections;
using UnityEngine;
using System.Collections.Generic; // Adicionar para usar List

public class Damage_Flash : MonoBehaviour
{
    [Header("Configurações de Flash")]
    public Color flashColor = Color.white;
    public float flashTime = 0.2f;

    [Header("Referências")]
    // ALTERADO: Tornamos 'rend' uma lista
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    public Material flashMaterial;
    public bool isPlayer;
    public Creature_Stats statsCreat;
    public Player_Stats statsPlayer;

    private List<Material> originalMaterials = new List<Material>();
    private bool isFlashing = false;

    // Novo: Flag para saber se o setup inicial já foi feito
    private bool isSetup = false;

    private void Awake()
    {
        // NOTA: O 'rend' que antes era o SpriteRenderer deste objeto é substituído 
        // pela lista 'renderers', que será populada pelo Creature_General.
        // O Awake para Player pode ser mais complexo dependendo de onde estão os renderers dele.

        if (!isPlayer)
        {
            statsCreat = transform.parent.GetComponentInChildren<Creature_Stats>();
            // Não precisamos mais do originalMaterial aqui, ele será setado pelo SetupRenderer.
        }
        statsPlayer = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
        if (renderers.Count > 0)
        {
            // Pega o material original de CADA renderer do Player
            foreach (var rend in renderers)
            {
                if (rend != null)
                    originalMaterials.Add(rend.sharedMaterial);
            }
            isSetup = true;
        }
    }

    // NOVO MÉTODO: Chamado pelo Creature_General para definir a lista de renderers e o material base
    public void SetupRenderer(Material material)
    {
        if (isPlayer) return;

        // O material da raça é o mesmo para todas as partes da criatura
        originalMaterials.Clear();
        for (int i = 0; i < renderers.Count; i++)
        {
            originalMaterials.Add(material);
        }
        isSetup = true;
    }

    private void Update()
    {
        if (!isSetup || isFlashing) return;

        // REMOVEMOS: a checagem para a criatura (!isPlayer) pois ela chama o FlashCoroutine() diretamente.

        // MANTEMOS: A checagem APENAS para o Player.
        if (isPlayer && statsPlayer != null && statsPlayer.tomouDanoNoFrame) // OU statsPlayer.justTookDamage
        {
            // NOTA: Se o Player_Stats usa 'justTookDamage' ou outra flag, mude 'tomouDanoNoFrame' aqui.
            StartCoroutine(FlashCoroutine());
        }
    }

    public IEnumerator FlashCoroutine()
    {
        if (renderers.Count == 0)
        {
            isFlashing = false;
            yield break;
        }

        isFlashing = true;

        // 1. Aplica o material de flash a TODOS os renderers
        foreach (var rend in renderers)
        {
            if (rend != null)
            {
                // Troca para o material de flash (o flashMaterial é um material temporário que pode ser compartilhado)
                rend.sharedMaterial = flashMaterial;
            }
        }
        flashMaterial.SetColor("_FlashColor", flashColor);

        float timer = 0f;

        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            float currentAmount = Mathf.Lerp(1f, 0f, timer / flashTime);
            flashMaterial.SetFloat("_FlashAmount", currentAmount);
            yield return null;
        }

        // 2. Volta para o material original em TODOS os renderers
        for (int i = 0; i < renderers.Count; i++)
        {
            if (renderers[i] != null && i < originalMaterials.Count)
            {
                // Usa a lista de materiais originais
                renderers[i].sharedMaterial = originalMaterials[i];
            }
        }
        isFlashing = false;
    }
}