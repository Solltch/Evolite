using UnityEngine;
using UnityEngine.UI;

public class Interact : MonoBehaviour
{
    public Camera cam;
    public Transform button;
    public float rayRange = 100f;
    public KeyCode interactKey = KeyCode.E;
    public float interactTime = 0.2f;
    public float disappearDelay = 0.2f; // tempo antes de sumir

    private Collider item;
    private bool interactableOnScreen;
    private bool isInteracting;
    private Vector3 originalButtonPos;
    private float disappearTimer;
    public float fadeSpeed = 5f;


    private CanvasGroup buttonGroup;

    void Start()
    {
        originalButtonPos = button.position;
        buttonGroup = button.GetComponent<CanvasGroup>();
        buttonGroup.alpha = 0f;
    }

    void Update()
    {
        Interaction();
    }

    void Interaction()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * rayRange, Color.red);

        if (Physics.Raycast(ray, out RaycastHit hit, rayRange))
        {
            item = hit.collider;
            if (item.CompareTag("Interactable"))
            {
                Vector3 screenPos = cam.WorldToScreenPoint(hit.point);

                buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 1f, Time.deltaTime * fadeSpeed);

                if (interactableOnScreen)
                    button.position = Vector3.Lerp(button.position, new Vector3(screenPos.x, screenPos.y - 60, screenPos.z), 0.25f);
                else
                    button.position = new Vector3(screenPos.x, screenPos.y - 60, screenPos.z);

                if (Input.GetKey(interactKey))
                {
                    isInteracting = true;
                    Visual();
                }
                if (Input.GetKeyDown(interactKey))
                {
                    {
                        isInteracting = false;
                        button.GetChild(1).GetComponent<Transform>().localScale = Vector3.Lerp(button.GetChild(1).GetComponent<Transform>().localScale, Vector3.one * 0.58f, fadeSpeed);
                    }

                    interactableOnScreen = true;
                    disappearTimer = disappearDelay;
                    return;
                }
            }

            // Quando não acerta nada, espera o delay antes de sumir
            if (disappearTimer > 0)
            {
                disappearTimer -= Time.deltaTime;
            }
            else
            {
                buttonGroup.alpha = Mathf.Lerp(buttonGroup.alpha, 0f, Time.deltaTime * fadeSpeed);
            }
        }

        void Visual()
        {
            if (isInteracting)
            {
                // animação de "interagir" (encolhe e volta)
                button.GetChild(1).GetComponent<Transform>().localScale = Vector3.Lerp(button.GetChild(1).GetComponent<Transform>().localScale, Vector3.one * 0.8f, fadeSpeed);
                item.GetComponent<InteractFunctions>().Interact();
                isInteracting = false;
            }
        }
    }
}