using UnityEngine;
using UnityEngine.UI;

public class Sliders_Control : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
    
    public void SetMaxValue(float max)
    {
        slider.maxValue = max;
    }

    public void SetValue(float value)
    {
        slider.value = value;
    }
}
