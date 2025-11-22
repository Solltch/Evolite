using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenager : MonoBehaviour
{

    public List<Skill_Class> skills = new List<Skill_Class>();
    public Player_General plr;

    void Awake()
    {
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        Skill_Class[] foundSkills = FindObjectsByType<Skill_Class>(FindObjectsSortMode.None);

        foreach (Skill_Class s in foundSkills)
        {
            skills.Add(s);
            AddSkillOnClick(s);
        }
    }

    void AddSkillOnClick(Skill_Class skill)
    {
        Button btn = skill.GetComponent<Button>();

        if (btn == null)
        {
            Debug.LogWarning("Skill sem botão: " + skill.nome);
            return;
        }

        btn.onClick.RemoveAllListeners();

        btn.onClick.AddListener(() => specificEvent(skill));
        btn.onClick.AddListener(() => genericEvent(skill));
    }

    void specificEvent(Skill_Class s)
    {
        if (!s.blocked)
        {
            switch (s.id)
            {
                case 0:
                    return;
                case 1:
                    plr.Herbiv = true;
                    return;
                case 2:
                    plr.Carniv = true;
                    return;
                case 3:
                    plr.Presis = true;
                    return;
                case 4:
                    plr.Couro = true;
                    return;
                case 5:
                    plr.Presas = true;
                    return;
                case 6:
                    plr.Olhos = true;
                    return;
                case 7:
                    return;
                case 8:
                    plr.Casco = true;
                    return;
                case 9:
                    plr.Dieta = true;
                    return;
                case 10:
                    plr.Garras = true;
                    return;
                case 11:
                    plr.Escond = true;
                    return;
                case 12:
                    plr.PatasA = true;
                    return;
                case 13:
                    plr.Esquiv = true;
                    return;
                case 14:
                    plr.Furtiv = true;
                    return;
                case 15:
                    plr.Ecoal = true;
                    return;
                case 16:
                    plr.Veneno = true;
                    return;
                case 17:
                    plr.Espinh = true;
                    return;
                case 18:
                    plr.Resist = true;
                    return;
                case 19:
                    plr.Carnic = true;
                    return;
                case 20:
                    plr.Regen = true;
                    return;
                case 21:
                    plr.Abraco = true;
                    return;
                case 22:
                    plr.Flor = true;
                    return;
                case 23:
                    plr.Chifre = true;
                    return;
                case 24:
                    plr.Salto = true;
                    return;
                case 25:
                    plr.Invisi = true;
                    return;
                case 26:
                    plr.Celere = true;
                    return;
                case 27:
                    plr.Gigant = true;
                    return;
                case 28:
                    plr.Titan = true;
                    return;
                case 29:
                    plr.Coloss = true;
                    return;
                case 30:
                    plr.Apex = true;
                    return;

            }

        }
    }

    void genericEvent(Skill_Class s)
    {
        Debug.Log("Melhorar skill: " + s.name);
    }
}
