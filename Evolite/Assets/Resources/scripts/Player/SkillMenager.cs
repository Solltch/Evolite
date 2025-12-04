using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class SkillMenager : MonoBehaviour
{
    public GameObject discoverScreen;
    public GameObject herbivoreIcon;
    public GameObject carnivoreIcon;
    public GameObject DashIcon;
    public GameObject BiteIcon;
    public Sprite chifreSprite;
    public Sprite florSprite;
    public List<Skill_Class> skills = new List<Skill_Class>();
    public Player_General plr;
    public HabilidadeAPI habilidadeApi;
    public CriatHabilAPI criatHabilApi;
    public Transform healthBar;

    void Awake()
    {
        herbivoreIcon.SetActive(false);
        carnivoreIcon.SetActive(false);
        DashIcon.SetActive(false);
        BiteIcon.SetActive(false);

        if (habilidadeApi == null)
        {
            habilidadeApi = FindObjectOfType<HabilidadeAPI>();
        }
        // Adicionar inicialização do CriatHabilAPI
        if (criatHabilApi == null)
        {
            criatHabilApi = FindObjectOfType<CriatHabilAPI>();
        }
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();

    }

    void Start()
    {
        Skill_Class[] foundSkills = FindObjectsByType<Skill_Class>(FindObjectsSortMode.None);

        foreach (Skill_Class s in foundSkills)
        {
            skills.Add(s);
        }

        if (habilidadeApi != null)
        {
            StartCoroutine(LoadSkillsFromDatabase());
        }
        else
        {
            InitializeSkillClicks();
        }
    }

    IEnumerator LoadSkillsFromDatabase()
    {
        yield return habilidadeApi.List((habilidadesArray) =>
        {
            if (habilidadesArray == null)
            {
                return;
            }

            foreach (var dbHabilidade in habilidadesArray)
            {
                Skill_Class targetSkill = skills.FirstOrDefault(s => s.id == dbHabilidade.id);

                if (targetSkill != null)
                {
                    targetSkill.nome = dbHabilidade.nome;
                    targetSkill.descricao = dbHabilidade.descricao;
                    targetSkill.custo = dbHabilidade.custo_DNA;
                }
            }
            InitializeSkillClicks();
        });
    }

    void VerifySkillRequirements(Skill_Class s)
    {
        if (s.requisitos == null || s.requisitos.Count == 0)
        {
            s.blocked = false;
            return;
        }

        List<Skill_Class> requisitosPegos = new List<Skill_Class>();

        for (int i = 0; i < s.requisitos.Count; i++)
        {
            if (s.requisitos[i].isTaken)
            {
                requisitosPegos.Add(s.requisitos[i]);
            }
        }

        if (requisitosPegos.Count == s.requisitos.Count)
        {
            s.blocked = false;
        }
        else
        {
            s.blocked = true;
        }
    }

    void UnlockDependents(Skill_Class s, Skill_Class sO)
    {
        if (s.requisitos == null || s.requisitos.Count == 0)
        {
            s.blocked = false;
        }
        else if (s.requisitos.Contains(sO))
        {
            bool all = s.requisitos.All(r => r.isTaken);
            s.blocked = !all;
        }

        s.BlockButton();
        s.ColorButton();
    }

    void InitializeSkillClicks()
    {
        foreach (Skill_Class s in skills)
        {
            AddSkillOnClick(s);
        }

        foreach (Skill_Class s in skills)
        {
            VerifySkillRequirements(s);
            s.BlockButton();
            s.ColorButton();
        }
    }

    void AddSkillOnClick(Skill_Class skill)
    {
        Button btn = skill.GetComponent<Button>();

        if (btn == null)
        {
            btn = skill.GetComponentInChildren<Button>();

            if (btn == null)
            {
                return;
            }
        }

        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(() => specificEvent(skill));
    }


    void specificEvent(Skill_Class s)
    {
            if (s.blocked) return;
            if (s.isTaken) return;

            int idCriatura = plr.idCriatura;
            int idHabilidade = s.id;

            if (idCriatura <= 0)
            {
                string msg = "Salve sua criatura (ID) antes de comprar habilidades permanentes!";
                Debug.LogWarning(msg);
            }

            if (plr.stats3.DNA >= s.custo)
            {
                plr.stats3.DNA -= s.custo;
                s.isTaken = true;

                if (criatHabilApi != null && idCriatura > 0 && idHabilidade > 0)
                {

                    StartCoroutine(criatHabilApi.Add(idCriatura, idHabilidade, (ok, res) =>
                    {
                        if (!ok)
                        {
                            string failMsg = $"Falha ao salvar no DB! Erro: {res}";
                            Debug.LogError(failMsg);
                        }
                        else
                        {
                            string successMsg = $"Habilidade {s.nome} adquirida e salva com sucesso!";
                             Debug.Log(successMsg);
                        }
                    }));
                }
                else if (idCriatura > 0)
                {
                    string apiWarning = "CriatHabilAPI não encontrado. Habilidade não salva no DB.";
                    Debug.LogWarning(apiWarning);
                }

                VerifySkillRequirements(s);

                foreach (Skill_Class sk in skills)
                {
                    UnlockDependents(sk, s);
                }

            switch (s.id)
            {
                case 0:
                    return;

                case 1:
                    plr.skills.Herbiv = true;
                    herbivoreIcon.SetActive(true);
                    return;

                case 2:
                    plr.skills.Carniv = true;
                    carnivoreIcon.SetActive(true);
                    return;

                case 3:
                    plr.skills.Presis = true;
                    return;

                case 4:
                    plr.skills.Couro = true;
                    plr.stats3.maxHealth += 50;
                    plr.stats3.curHealth += 50;
                    healthBar.localScale = new Vector3(healthBar.localScale.x * 1.5f, healthBar.localScale.y, healthBar.localScale.z);
                    return;

                case 5:
                    plr.skills.Presas = true;
                    BiteIcon.SetActive(true);
                    return;

                case 6:
                    return;

                case 8:
                    return;

                case 9:
                    plr.skills.Dieta = true;
                    plr.stats3.hungerDecaySpeed /= 2;
                    return;

                case 10:
                    plr.skills.Garras = true;
                    plr.stats2.baseAttackDmg += 5;
                    return;

                case 11:
                    plr.skills.Escond = true;
                    return;

                case 12:
                    plr.skills.PatasA = true;
                    return;

                case 13:
                    plr.skills.Esquiv = true;
                    DashIcon.SetActive(true);
                    return;

                case 14:
                    return;

                case 15:
                    plr.skills.Ecoal = true;
                    return;

                case 16:
                    plr.skills.Veneno = true;
                    return;

                case 17:
                    plr.skills.Espinh = true;
                    return;

                case 18:
                    plr.skills.Resist = true;
                    return;

                case 19:
                    plr.skills.Carnic = true;
                    return;

                case 20:
                    plr.skills.Regen = true;
                    return;

                case 21:
                    plr.skills.Abraco = true;
                    return;

                case 22:
                    plr.skills.Flor = true;
                    discoverScreen.SetActive(true);
                    discoverScreen.transform.GetChild(1).GetComponent<Image>().sprite = florSprite;
                    return;

                case 23:
                    plr.skills.Chifre = true;
                    discoverScreen.SetActive(true);
                    discoverScreen.transform.GetChild(1).GetComponent<Image>().sprite = chifreSprite;
                    return;

                case 24:
                    plr.skills.Salto = true;
                    plr.stats.dashForce += plr.stats.dashForce;
                    return;

                case 25:
                    plr.skills.Invisi = true;
                    return;

                case 26:
                    plr.skills.Celere = true;
                    return;

                case 27:
                    plr.skills.Gigant = true;
                    return;

                case 28:
                    plr.skills.Titan = true;
                    return;

                case 29:
                    plr.skills.Coloss = true;
                    return;

                case 30:
                    plr.skills.Apex = true;
                    return;
            }
        }
            else
            {
                string dnaMsg = $"DNA insuficiente! Custo: {s.custo}. Você tem: {plr.stats3.DNA}";
                Debug.LogWarning(dnaMsg);
            }
    }
}
