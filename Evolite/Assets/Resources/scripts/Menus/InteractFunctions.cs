using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class InteractFunctions : MonoBehaviour
{
    public GameObject customMenu;
    public Player_Stats stats;
    public Player_General playerGeneral;
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
        playerGeneral = GameObject.Find("Player Sprite").GetComponent<Player_General>();
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
        customMenu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        player.position = new Vector3(transform.position.x, transform.position.y + .3f, transform.position.z);
        Player_Movement pMove = player.GetComponent<Player_Movement>();
        MenuFunctions GM = GameObject.Find("GameMenager").GetComponent<MenuFunctions>();
        CinemachineRotationComposer cameraRotate = GameObject.Find("FreeLook Camera").GetComponent<CinemachineRotationComposer>();
        cameraRotate.Damping = Vector3.zero;
        cameraRotate.TargetOffset = new Vector3(-0.85f, 0, 0);
        pMove.isAbleToMove = false;
        GM.isAbleToPause = false;
        playerGeneral.isCustomizing = true;
        playerGeneral.lastMoveX = 0;
        playerGeneral.lastMoveZ = -1;
        customMenu.SetActive(true);
        customMenu.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        Time.timeScale = 0.0001f;
    }
}
