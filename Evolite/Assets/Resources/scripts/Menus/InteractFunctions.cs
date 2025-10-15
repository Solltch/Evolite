using System.Collections;
using UnityEngine;

public class InteractFunctions : MonoBehaviour
{
    public Player_Stats stats;
    public bool isFood;
    public bool isMature;
    private bool lastMatureState;

    private void Awake()
    {
        isMature = true;
        stats = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
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
            stats.curHealth += 10;
            stats.DNA += 5;
            stats.curHunger += 20;
        }

        StartCoroutine(DestroyAfterEffect());
    }

    private IEnumerator DestroyAfterEffect()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }
}
