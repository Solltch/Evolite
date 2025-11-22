using UnityEngine;

[ExecuteAlways]
public class CustomPart : MonoBehaviour
{
    public Sprite[] front;
    public Sprite[] side;
    public Sprite[] back;
    public SpriteRenderer partFront;
    public SpriteRenderer partSide;
    public SpriteRenderer partBack;
    public void SetSprite(int i, int Z)
    {
        if (Z < 0)
        {
            if (front != null && i > -1 && i < front.Length && partFront != null)
                partFront.sprite = front[i];
            else
                partFront.sprite = null;
        }
        else if (partFront != null)
            partFront.sprite = null;

        if (Z == 0)
        {
            if (side != null && i > -1 && i < side.Length && partSide != null)
                partSide.sprite = side[i];
            else
                partFront.sprite = null;
        }   
        else if (partSide != null)
            partSide.sprite = null;

        if (Z > 0)
        {
            if (back != null && i > -1 && i < back.Length && partBack != null)
                partBack.sprite = back[i];
            else
                partFront.sprite = null;
        }
        else if (partBack != null)
            partBack.sprite = null;
    }
    public void Refresh() => SetSprite(-1, 0);
}
