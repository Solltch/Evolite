using UnityEditor.Experimental.GraphView;
using UnityEngine;

[System.Serializable]
public class CustomPart : MonoBehaviour
{
    public string name;
    public SpriteRenderer[] front;
    public SpriteRenderer[] side;
    public SpriteRenderer[] back;
    public int Length;
    public int currentIndex;

    public Player_General plr;

    public void Awake()
    {
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        Length = front.Length;
    }
    public void ChangePart()
    {
        int i = 0;
        while (i < Length)
        {
            if (i == currentIndex)
            {
                front[i].enabled = true;
            }
            else
            {
                front[i].enabled = false;
            }
        }
    }
}
