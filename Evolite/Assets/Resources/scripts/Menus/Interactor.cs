using System.Collections;
using TMPro;
using UnityEngine;

public class Interact : MonoBehaviour
{
    public Camera cam;
    public Transform button;
    public TextMeshProUGUI action;

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
    private CanvasGroup buttonGroup;
    

    void Awake()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        button = GameObject.Find("InteractBut").GetComponent<Transform>();
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

        // Enquanto a tecla está pressionada E o tempo ainda não acabou
        while (Input.GetKey(interactKey) && elapsed < interactTime)
        {
            elapsed += Time.deltaTime;
            buttonChild.localScale = Vector3.Lerp(startScale, targetScale, elapsed / interactTime);

            // Se o botão já chegou no tamanho alvo, interrompe e executa
            if (Vector3.Distance(buttonChild.localScale, targetScale) < 0.01f)
                break;

            yield return null;
        }

        // Executa ação
        targetItem.GetComponent<InteractFunctions>().Interact();

        // Anima o botão voltando ao tamanho original
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
        isInteracting = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, rayRange);
    }
}
