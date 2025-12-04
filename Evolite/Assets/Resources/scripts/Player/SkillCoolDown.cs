using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SkillCoolDown : MonoBehaviour
{
    public Slider slider;

    private Coroutine fillRoutine;

    void Reset()
    {
        // tenta pegar o slider caso esqueça de arrastar no inspector
        if (slider == null) slider = GetComponent<Slider>();
    }

    void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    public void SetMinMax(float maxValue, float minValue = 0f)
    {
        if (slider == null) return;
        slider.maxValue = maxValue;
        slider.minValue = minValue;
    }

    public void StartCooldown(float duration)
    {
        if (slider == null)
        {
            Debug.LogWarning("SkillCoolDown: slider não atribuído.");
            return;
        }

        // Se já estiver enchendo, para e reinicia (resetando tempo)
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        // Ajusta os valores do slider: min = 0, max = duration (opcionalmente)
        slider.minValue = 0f;
        slider.maxValue = duration;
        slider.value = slider.minValue;

        fillRoutine = StartCoroutine(FillOverTime(duration));
    }

    private IEnumerator FillOverTime(float duration)
    {
        float elapsed = 0f;

        // Proteção para duration zero (preenche instantaneamente)
        if (duration <= 0f)
        {
            slider.value = slider.maxValue;
            fillRoutine = null;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            slider.value = Mathf.Lerp(slider.minValue, slider.maxValue, t);
            yield return null;
        }

        slider.value = slider.maxValue;
        fillRoutine = null;
    }

    public void CancelCooldown()
    {
        if (fillRoutine != null)
            StopCoroutine(fillRoutine);

        if (slider != null)
            slider.value = slider.minValue;

        fillRoutine = null;
    }
}
