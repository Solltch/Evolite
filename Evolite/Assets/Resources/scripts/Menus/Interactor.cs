using System.Collections;
using TMPro;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public Camera cam;
    public Transform button;
    public TextMeshProUGUI action;
    public MenuFunctions GM;
    public Player_General plr;

    public float rayRange = 5f;
    public float sphereRadius = 0.5f;
    public KeyCode interactKey = KeyCode.E;
    public float interactTime = 0.2f;
    public float disappearDelay = 0.2f;
    public float fadeSpeed = 5f;

    private Collider targetItem;
    private bool interactableOnScreen;
    private bool isInteracting;
    private Vector3 originalButtonPos;
    private float disappearTimer;
    public CanvasGroup buttonGroup;
    

    void Awake()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        button = GameObject.Find("InteractBut").GetComponent<Transform>();
        GM = GameObject.Find("GameMenager").GetComponent<MenuFunctions>();
        plr = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        originalButtonPos = button.position;
        buttonGroup = button.GetComponent<CanvasGroup>();
        buttonGroup.alpha = 0f;
        action = button.GetComponentInChildren<TextMeshProUGUI>();
    }

    void Update()
    {
        Interaction();
    }

    void Interaction()
    {

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;

        Collider[] hits = Physics.OverlapSphere(transform.position, rayRange);
        Collider bestItem = null;
        float bestDot = -1f;
        Vector3 camForward = cam.transform.forward;

        foreach (Collider col in hits)
        {
            if (col.CompareTag("Interactable"))
            {
                Vector3 dirToItem = (col.transform.position - transform.position).normalized;
                float dot = Vector3.Dot(camForward, dirToItem);
                if (dot > bestDot)
                {
                    bestDot = dot;
                    bestItem = col;
                }
            }
        }

        targetItem = bestItem;

        if (targetItem != null)
        {
            if (GM != null)
            {
                if (!GM.isPaused)
                {
                    if ((plr.skills.Carniv == true && targetItem.GetComponent<InteractFunctions>().isMeat == true) || (plr.skills.Herbiv == true && targetItem.GetComponent<InteractFunctions>().isFruit == true) || (targetItem.GetComponent<InteractFunctions>().isMeat == false && targetItem.GetComponent<InteractFunctions>().isFruit == false))
                    {
                        Vector3 screenPos = cam.WorldToScreenPoint(targetItem.transform.position);
                        buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 1f, Time.deltaTime * fadeSpeed);

                        action.text = targetItem.GetComponent<InteractFunctions>().action;

                        if (interactableOnScreen)
                            button.position = Vector3.Lerp(button.position, new Vector3(screenPos.x, screenPos.y - Screen.height * 0.1f, screenPos.z), 0.25f);
                        else
                            button.position = new Vector3(screenPos.x, screenPos.y - Screen.height * 0.1f, screenPos.z);

                        if (targetItem != null && Input.GetKeyDown(interactKey) && !isInteracting)
                        {
                            StartCoroutine(Visual());
                        }

                        if (Input.GetKeyDown(interactKey))
                        {
                            button.GetChild(1).localScale = Vector3.Lerp(button.GetChild(1).localScale, Vector3.one * 0.58f, fadeSpeed);
                            interactableOnScreen = true;
                            disappearTimer = disappearDelay;
                            return;
                        }
                    }
                }
            }
        }
        else
        {
            if (disappearTimer > 0) disappearTimer -= Time.deltaTime;
            else buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            interactableOnScreen = false;
        }

    }

    private IEnumerator Visual()
    {
        if (targetItem == null) yield break;

        isInteracting = true;
        Transform buttonChild = button.GetChild(1);
        Vector3 startScale = buttonChild.localScale;
        Vector3 targetScale = Vector3.one * 0.8f;
        float elapsed = 0f;

        // Enquanto a tecla está pressionada e o tempo não acabou
        while (Input.GetKey(interactKey) && elapsed < interactTime)
        {
            elapsed += Time.deltaTime;
            buttonChild.localScale = Vector3.Lerp(startScale, targetScale, elapsed / interactTime);
            yield return null;
        }

        if (elapsed < interactTime)
        {
            // volta o botão visualmente
            yield return StartCoroutine(ReturnButton(buttonChild));
            isInteracting = false;
            yield break;
        }

        targetItem.GetComponent<InteractFunctions>().Interact();

        if (targetItem.GetComponent<InteractFunctions>().isFood)
        {
            plr.Eating();
            StartCoroutine(DelayedResetEating());
        }

        // Volta o botão visualmente
        yield return StartCoroutine(ReturnButton(buttonChild));

        isInteracting = false;
    }

    private IEnumerator DelayedResetEating()
    {
        yield return new WaitForSeconds(0.6f);
        plr.ResetEating();
    }

    private IEnumerator ReturnButton(Transform buttonChild)
    {
        float returnTime = 0.15f;
        Vector3 originalScale = Vector3.one * 0.58f;
        float t = 0f;

        while (t < returnTime)
        {
            t += Time.deltaTime;
            buttonChild.localScale = Vector3.Lerp(buttonChild.localScale, originalScale, t / returnTime);
            yield return null;
        }
        buttonChild.localScale = originalScale;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rayRange);
    }
}
