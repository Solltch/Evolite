using System.Collections;
using UnityEngine;

public class InteractFunctions : MonoBehaviour
{
    public Player_Stats stats;
    public bool isFood;
    public bool isMature;
    public bool isFruit;
    public string action;
    private bool lastMatureState;
    

    private void Awake()
    {
        isMature = true;
        stats = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
        if (isFood)
        {
            if (isFruit)
                action = "Comer";
            else
                action = "Devorar";
        }
    }

    private void Update()
    {
        if (isFood && isMature != lastMatureState)
        {
            gameObject.tag = isMature ? "Interactable" : "Untagged";
            lastMatureState = isMature;
        }
    }

    public void Interact()
    {
        if (isFood)
        {
            
            FoodInteract();
        }
    }

    public void FoodInteract()
    {
        if (!isMature) 
            return;

        if (stats != null)
        {
            if (isFruit)
            {
                stats.curHealth += 20;
                stats.DNA += 5;
                stats.curHunger += 10;
            }
            else
            {
                stats.curHealth += 10;
                stats.DNA += 10;
                stats.curHunger += 20;
            }
        }

        StartCoroutine(DestroyAfterEffect());
    }

    private IEnumerator DestroyAfterEffect()
    {
        yield return new WaitForSeconds(0.1f);

        if (isFruit)
            Destroy(gameObject);
        else
            Destroy(transform.parent.gameObject);
    }
}
