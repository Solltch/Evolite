using UnityEngine;

public class CustomPart : MonoBehaviour
{
    public string name;
    public SpriteRenderer[] front;
    public SpriteRenderer[] side;
    public SpriteRenderer[] back;
    public int currentIndex;

    private void Awake()
    {
        SetAllEnabled(false);
    }

    /*public void Update()
    {
        for (int i = 0; i < front.Length; i++)
        {
            if (i == currentIndex && front[i] != null)
            {
                front[i].enabled = true;
            }
            else
            {
                front[i].enabled = false;
            }
        }
    }*/
    private void SetAllEnabled(bool enabled)
    {
        foreach (var sr in front)
            if (sr != null) sr.enabled = enabled;
        foreach (var sr in side)
            if (sr != null) sr.enabled = enabled;
        foreach (var sr in back)
            if (sr != null) sr.enabled = enabled;
    }
}
