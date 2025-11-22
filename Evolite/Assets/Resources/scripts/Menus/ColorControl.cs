using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorControl : MonoBehaviour
{
    public Image myColor;

    [Header("Cores configuráveis")]
    public Color normalColor;
    public Color disableColor;

    private List<ColorControl> others = new List<ColorControl>();

    private void Awake()
    {
        myColor = GetComponent<Image>();

        // Achar todos ColorControl
        ColorControl[] all = FindObjectsByType<ColorControl>(FindObjectsSortMode.None);

        foreach (var c in all)
        {
            if (c != this)
                others.Add(c);
        }
    }

    public void ChangeColor()
    {
        // Ativa a cor deste
        myColor.color = normalColor;

        // Desativa os outros
        foreach (var c in others)
        {
            c.myColor.color = disableColor;
        }
    }
}
