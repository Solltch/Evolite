using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Damage_Flash : MonoBehaviour
{
    [Header("Configurações de Flash")]
    public float flashTime = 0.2f;

    [Header("Referências")]
    // ALTERADO: Tornamos 'rend' uma lista
    public List<SpriteRenderer> renderers = new List<SpriteRenderer>();
    public bool isPlayer;
    public Creature_Stats statsCreat;
    public Player_Stats statsPlayer;

    // ALTERADO: Será preenchida com as novas instâncias de material.
    public List<Material> uniqueInstantiatedMaterials = new List<Material>();
    private bool isFlashing = false;

    // Novo: Flag para saber se o setup inicial já foi feito
    private bool isSetup = false;

    private void Awake()
    {
    }

    private void Start()
    {
        statsPlayer = GameObject.Find("Player Collider")?.GetComponent<Player_Stats>();

        // O setup inicial do Player (que tem materiais fixos) ainda pode ser feito aqui
        if (isPlayer && renderers.Count > 0)
        {
            foreach (var rend in renderers)
            {
                if (rend != null)
                {
                    // O .material cria uma instância automaticamente para cada renderer do Player
                    // e nós a armazenamos.
                    uniqueInstantiatedMaterials.Add(rend.material);
                }
            }
            isSetup = true;
        }

        if (!isPlayer)
        {
            SetupRenderer();
        }
    }

    // NOVO MÉTODO: Simplificado. Chamado pelo Creature_General ou por onde for necessário.
    public void SetupRenderer()
    {
        Debug.Log("Parte 1");

        if (isPlayer) return;

        uniqueInstantiatedMaterials.Clear();

        Debug.Log("Parte 2");
        foreach (var rend in renderers)
        {
            if (rend != null && rend.sharedMaterial != null)
            {
                Material uniqueMaterialInstance = rend.material;

                Debug.Log("Parte x");

                // Adiciona a instância única à lista para o Flash.
                if (!uniqueInstantiatedMaterials.Contains(uniqueMaterialInstance))
                {
                    Debug.Log("Parte x1");
                    uniqueInstantiatedMaterials.Add(uniqueMaterialInstance);
                }
            }
        }

        Debug.Log("Parte 3");

        isSetup = true;
    }

    private void Update()
    {
        if (!isSetup || isFlashing) return;

        // MANTEMOS: A checagem APENAS para o Player.
        if (isPlayer && statsPlayer != null && statsPlayer.tomouDanoNoFrame)
        {
            StartCoroutine(FlashCoroutine());
        }
    }

    public IEnumerator FlashCoroutine()
    {
        // Alteramos para a nova lista de materiais
        if (renderers.Count == 0 || uniqueInstantiatedMaterials.Count == 0)
        {
            isFlashing = false;
            yield break;
        }

        Debug.Log("Parte 4");

        isFlashing = true;

        float timer = 0f;

        // 1. Aplica o valor máximo de flash no início
        // Itera sobre a nova lista de materiais instanciados
        foreach (var mat in uniqueInstantiatedMaterials)
        {
            if (mat != null)
            {
                mat.SetColor("_FlashColor", Color.white);
                mat.SetFloat("_FlashAmount", 10f);
            }
        }

        Debug.Log("Parte 5");

        // 2. Interpola o _FlashAmount de 1 para 0 ao longo do flashTime
        while (timer < flashTime)
        {
            Debug.Log("Parte 6");
            timer += Time.deltaTime;
            float currentAmount = Mathf.Lerp(10f, 0f, timer / flashTime);

            // Aplica o valor interpolado em TODOS os materiais únicos
            foreach (var mat in uniqueInstantiatedMaterials)
            {
                if (mat != null)
                {
                    mat.SetFloat("_FlashAmount", currentAmount);
                }
                Debug.Log("Parte x.7");
            }
            yield return null;
        }

        Debug.Log("Parte 8");

        // 3. Garante que o _FlashAmount seja 0 no final
        foreach (var mat in uniqueInstantiatedMaterials)
        {
            if (mat != null)
            {
                mat.SetFloat("_FlashAmount", 0f);
                Debug.Log("Parte x.9");
            }
        }

        isFlashing = false;
    }
}