using TMPro;
using UnityEngine;

public class Scream_Updaters : MonoBehaviour
{
    public Player_Stats dnaValue;
    public TextMeshProUGUI dnaText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        dnaText = GameObject.Find("DNA").GetComponent<TextMeshProUGUI>();
        dnaValue = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
    }

    // Update is called once per frame
    void Update()
    {
        dnaText.text = $"DNA: {dnaValue.DNA}";
    }
}
