using UnityEngine;
using UnityEngine.Windows;

public class Player_General : MonoBehaviour
{
    public Animator animator;
    private float lastMoveX;
    private float lastMoveZ;
    public Test_Movement stats;

    public Transform cameraTransform;
    public Transform plr_collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gira = cameraTransform.rotation;
        gira.x = 0;
        gira.z = 0;
        transform.rotation = gira;

        Vector3 position = plr_collider.position;
        transform.position = position; 
    }

    void FixedUpdate()
    {
        
        float inputX = UnityEngine.Input.GetAxisRaw("Horizontal");
        float inputZ = UnityEngine.Input.GetAxisRaw("Vertical");

        if (inputX != 0 || inputZ != 0)
        {
            lastMoveX = inputX;
            lastMoveZ = inputZ;
        }

        Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

        if (inputX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (inputX > 0)
            transform.localScale = new Vector3(1, 1, 1);

        animator.SetFloat("Horizontal", lastMoveX);
        animator.SetFloat("Vertical", lastMoveZ);
        animator.SetFloat("Moving", inputDir.sqrMagnitude);

        Vector3 camFrente = cameraTransform.forward;
        Vector3 camDireita = cameraTransform.right;
        camFrente.y = 0;
        camDireita.y = 0;
        camFrente.Normalize();
        camDireita.Normalize();

        if (inputDir.sqrMagnitude > 0.01f)
        {
            lastMoveX = inputX;
            lastMoveZ = inputZ;
        }

        animator.speed = stats.isRunning ? 1.5f : 1f;
    }
}
