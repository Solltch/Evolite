using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Damage_Flash : MonoBehaviour
{
    public Color flashColor = Color.white;
    public float flashTime = .2f;
    public SpriteRenderer rend;
    public Material flashMaterial;

    private Material originalMaterial;
    private bool isFlashing = false;
    private Coroutine flashCoroutine = null;

    void Start()
    {
        Material loadedMat = Resources.Load<Material>("Shaders/FlashMaterial");
        flashMaterial = new Material(loadedMat);
        flashTime = .2f;

        rend = GetComponent<SpriteRenderer>();
        originalMaterial = rend.material;
    }

    public void Flash()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        rend.material = flashMaterial;
        flashMaterial.SetColor("_FlashColor", flashColor);

        float timer = 0f;

        while (timer < flashTime)
        {
            timer += Time.deltaTime;
            float currentAmount = Mathf.Lerp(1f, 0f, timer / flashTime);
            flashMaterial.SetFloat("_FlashAmount", currentAmount);
            yield return null;
        }

        rend.material = originalMaterial;
        flashCoroutine = null;
    }
}
