using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Skill_Class : MonoBehaviour
{
    public int id;
    public string nome;
    public string descricao;
    public float custo;
    public bool isTaken;
    public bool blocked;

    public List<Skill_Class> requisitos;
}
