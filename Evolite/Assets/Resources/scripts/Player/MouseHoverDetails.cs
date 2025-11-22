using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MouseHoverDetails : MonoBehaviour
{
    public Skill_Class target;
    public bool OnMenu;
    public Vector3 mousePos;
    public GameObject ToolTip;
    public TextMeshProUGUI TTnome;
    public TextMeshProUGUI TTdescricao;
    public TextMeshProUGUI TTcusto;

    private GraphicRaycaster raycaster;
    private PointerEventData pointerEventData;
    private EventSystem eventSystem;

    void Awake()
    {
        raycaster = FindFirstObjectByType<GraphicRaycaster>();
        eventSystem = FindFirstObjectByType<EventSystem>();

        ToolTip = GameObject.Find("SkillToolTip");
        TTnome = GameObject.Find("STTNome").GetComponent<TextMeshProUGUI>();
        TTdescricao = GameObject.Find("STTDescrição").GetComponent<TextMeshProUGUI>();
        TTcusto = GameObject.Find("STTCusto").GetComponent<TextMeshProUGUI>();

        ToolTip.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (OnMenu)
        {
            // Criar o dado do pointer do mouse
            pointerEventData = new PointerEventData(eventSystem);
            pointerEventData.position = Input.mousePosition;

            // Resultados do Raycast
            var results = new List<RaycastResult>();

            // Faz o Raycast UI
            raycaster.Raycast(pointerEventData, results);

            if (results.Count > 0)
            {
                // Pega o primeiro objeto UI
                GameObject hovered = results[0].gameObject;

                // Verifica se esse objeto tem um script SkillButton, por ex
                var skillButton = hovered.GetComponent<Skill_Class>();

                if (skillButton != null)
                {
                    if (!skillButton.blocked)
                        ShowTooltip(skillButton);
                    else
                        BlockedSkillTolltip();
                }
                else
                    HideTooltip();
            }
            else
            {
                HideTooltip();
            }
        }
    }

    void ShowTooltip(Skill_Class skill)
    {
        ToolTip.SetActive(true);
        ToolTip.transform.position = Input.mousePosition;

        TTnome.text = skill.nome;
        TTdescricao.text = skill.descricao;
        TTcusto.text = skill.custo.ToString();
    }

    void BlockedSkillTolltip()
    {
        ToolTip.SetActive(true);
        ToolTip.transform.position = Input.mousePosition;

        TTnome.text = "???";
        TTdescricao.text = "??????";
        TTcusto.text = "?";
    }

    void HideTooltip()
    {
        ToolTip.SetActive(false);
    }
}
