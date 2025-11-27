using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Skill_Class : MonoBehaviour
{
    public int id;
    public string nome;
    public string descricao;
    public float custo;
    public bool isTaken;
    public bool blocked;

    public List<Skill_Class> requisitos;

    public void BlockButton()
    {
        Button btn = GetComponent<Button>();

        if (isTaken)
        {
            btn.interactable = true;
        }
        else if (blocked)
        {
            btn.interactable = false;
        }
        else
        {
            btn.interactable = true;
        }
    }

    public void ColorButton()
    {
        Button btn = GetComponent<Button>();
        ColorBlock colors = btn.colors;

        if (isTaken)
        {
            colors.normalColor = Color.white;
        }

        btn.colors = colors;
    }
}
