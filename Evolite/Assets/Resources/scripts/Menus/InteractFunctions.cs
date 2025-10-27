using System.Collections;
using UnityEngine;

public class InteractFunctions : MonoBehaviour
{
    public Player_Stats stats;
    public Transform player;
    public string action;

    public bool isFood;
    public bool isMature;
    public bool isFruit;
    private bool lastMatureState;

    public bool isNest;

    private void Awake()
    {
        stats = GameObject.Find("Player Collider").GetComponent<Player_Stats>();
        player = GameObject.Find("Player Collider").GetComponent<Transform>();
        if (isFood)
        {
            isMature = true;
            if (isFruit)
                action = "Comer";
            else
                action = "Devorar";
        }
        if (isNest)
        {
            action = "Procriar";
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
            FoodInteract();
        if (isNest)
            NestInteract();
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

    public void NestInteract()
    {
        player.position = new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z);
        Player_Movement pMove = player.GetComponent<Player_Movement>();
        pMove.isAbleToMove = false;
        //codigo pra deixar tudo invisível e por um efeito preto de fundo

    }
}
