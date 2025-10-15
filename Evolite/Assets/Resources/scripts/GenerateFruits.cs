using System.Collections;
using UnityEngine;

public class GenerateFruits : MonoBehaviour
{
    [Header("Prefab e Tempo")]
    public GameObject fruit;
    public float spawnInterval = 5f;
    public float growDuration = 2f;

    [Header("Limite de Frutas")]
    public int maxFruits = 5;

    [Header("Controle de Crescimento")]
    public Vector3 startScale = Vector3.zero;
    public Vector3 targetScale = Vector3.one;
    public Vector3 spawnArea = new Vector3(2f, 0f, 2f);
    public float spawnHeight;

    [Tooltip("Distância mínima entre as frutas")]
    public float spawnMargin = 0.5f;

    private void Start()
    {
        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (transform.childCount < maxFruits)
                yield return StartCoroutine(SpawnAndGrow());
        }
    }

    private IEnumerator SpawnAndGrow()
    {
        Vector3 randomPos = Vector3.zero;
        bool foundSpot = false;

        
            Vector3 candidate = new Vector3
            (
                Random.Range(-spawnArea.x / 2f, spawnArea.x / 2f),
                Random.Range(-spawnArea.y / 2f + spawnHeight, spawnArea.y / 2f + spawnHeight),
                0 - .005f
            );

            if (IsPositionFree(candidate))
            {
                randomPos = candidate;
                foundSpot = true;
            }

        if (!foundSpot)
            yield break;

        GameObject child = Instantiate(fruit, transform);
        child.transform.localPosition = randomPos;
        child.transform.localScale = startScale;

        InteractFunctions[] components = child.GetComponents<InteractFunctions>();
        foreach (var comp in components)
        {
            comp.isMature = false;
            comp.gameObject.tag = "Untagged";
        }   

        float elapsed = 0f;
        while (elapsed < growDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / growDuration);
            child.transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        foreach (var comp in components)
            comp.isMature = true;
    }

    private bool IsPositionFree(Vector3 position)
    {
        foreach (Transform child in transform)
        {
            if (Vector3.Distance(child.localPosition, position) < spawnMargin)
                return false;
        }
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawCube(new Vector3(0, spawnHeight, 0 + .005f), spawnArea);
    }
}