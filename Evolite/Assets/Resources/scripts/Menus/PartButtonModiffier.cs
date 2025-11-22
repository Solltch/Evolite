using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PartButtonModiffier : MonoBehaviour
{
    public Image childImage;
    public Button button;
    public Player_General plr;
    public Image buttonImage;
    public PlayerSetPart comparator;

    private List<PartButtonModiffier> others = new List<PartButtonModiffier>();

    void Awake()
    {
        buttonImage = GetComponent<Image>();
        button = GetComponent<Button>();
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        comparator = GetComponent<PlayerSetPart>();

        // Pega imagem do filho real
        childImage = transform.GetChild(0).GetComponent<Image>();

        // Lista dos outros botões
        PartButtonModiffier[] all = FindObjectsByType<PartButtonModiffier>(FindObjectsSortMode.None);
        foreach (var c in all)
        {
            if (c != this)
                others.Add(c);
        }
    }

    void Start()
    {
        // Espera 1 frame para garantir que tudo da UI está inicializado
        StartCoroutine(InvokeNextFrame());
    }

    private IEnumerator InvokeNextFrame()
    {
        yield return null; // 1 frame

        // Só depois disso chamamos o click automático
        AutoSelect();
    }

    private void AutoSelect()
    {
        switch (comparator.partType)
        {
            case PlayerSetPart.PlayerPartType.Head:
                if (comparator.iD == plr.headIndex)
                    button.onClick.Invoke();
                break;

            case PlayerSetPart.PlayerPartType.Eye:
                if (comparator.iD == plr.eyeIndex)
                    button.onClick.Invoke();
                break;

            case PlayerSetPart.PlayerPartType.Pupil:
                if (comparator.iD == plr.pupilIndex)
                    button.onClick.Invoke();
                break;

            case PlayerSetPart.PlayerPartType.BodyAccessory:
                if (comparator.iD == plr.bodyAcessoriesIndex)
                    button.onClick.Invoke();
                break;

            case PlayerSetPart.PlayerPartType.HeadAccessory:
                if (comparator.iD == plr.headAcessoriesIndex)
                    button.onClick.Invoke();
                break;
        }
    }

    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    // Roda a cada 0.2s
    private void Tick()
    {
        if (plr.isCustomizing)
        {
            if (button.interactable)
                childImage.color = buttonImage.color;
            else
                childImage.color = button.colors.disabledColor;

        }
    }

    public void ChangeColor()
    {
        if (button.interactable)
        {
            // Ativa este ícone
            childImage.color = button.colors.selectedColor;

            // Desativa todos os outros ícones
            foreach (var c in others)
            {
                if (c.button.interactable)
                    c.childImage.color = c.button.colors.normalColor;
                else
                    c.childImage.color = c.button.colors.disabledColor;
            }
        }
        else
        {
            childImage.color = button.colors.disabledColor;
        }
    }

}
