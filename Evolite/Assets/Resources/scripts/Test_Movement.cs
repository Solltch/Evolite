using System;
using UnityEngine;

public class test_movement : MonoBehaviour
{
    [Header("Movimento")]
    private float moveSpeed;
    public float bonusMoveSpeed;
    public float walkingMoveSpeed;
    public float runningMoveSpeed;
    public float sneakingMoveSpeed;
    private Vector3 lastRelativeDir;
    public Vector3 moveInput;

    [Header("Pulo")]
    public float ray;

    public LayerMask groundLayer;

    [Header("Comparações")]
    public bool isRunning;
    public bool isGrounded;
    public bool isSlopeGrounded;
    public bool isSneaking;
    public bool isMoving;
    private bool isMovingBU;
    public bool isJumping;
    public bool testCheck;
    public bool isAbleToMove;
    public DateTime Falling;

    [Header("Butões")]
    public KeyCode runKey;
    public KeyCode sneakKey;
    public KeyCode jumpKey;

    public MovementState state;
    public enum MovementState
    {
        walking,
        running,
        sneaking,
        air,
    }

    [Header("Componentes")]
    public Transform cameraTransform;
    private Rigidbody rb;
    public Transform scale;
    public Player_Stats stats;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        cameraTransform = UnityEngine.Camera.main.transform;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        isAbleToMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        StateHandler();
        MyInputs();
        //rotation();
    }

    private void FixedUpdate()
    {
        MovementHandler();
    }

    private void CheckGrounded()
    {
        // Origem do ray (levemente acima do pivot para evitar colisão com o próprio colisor)
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        Debug.DrawRay(origin, Vector3.down * ray, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ray, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;

            // Calcular o ângulo DO HIT ATUAL
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
        }
        else
        {
            isGrounded = false;
        }

    }

    private void StateHandler()
    {
        // Mode - Running
        if (Input.GetKey(runKey) && isGrounded && !stats.isExhausted)
        {
            state = MovementState.running;
            moveSpeed = runningMoveSpeed;
        }

        // Mode - Sneaking
        else if (Input.GetKey(sneakKey) && isGrounded)
        {
            state = MovementState.sneaking;
            moveSpeed = sneakingMoveSpeed;
        }

        // Mode - Walking
        else if (isGrounded)
        {
            state = MovementState.walking;
            moveSpeed = walkingMoveSpeed;
        }

        if (state == MovementState.running && moveInput != Vector3.zero)
            isRunning = true;
        else
            isRunning = false;
        if (state == MovementState.sneaking)
            isSneaking = true;
        else
            isSneaking = false;
    }

    private void MyInputs()
    {
        //wasd + setinhas
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        //Alinhar os eixos de movimento xyz com a camera e etc
        {
            Vector3 inputDir = new Vector3(inputX, 0f, inputZ).normalized;

            //Condição para congelar X/Z: parado, está no chão, **não** em mt inclinada
            bool nearlyStopped = inputDir.sqrMagnitude < 0.01f;

            if (nearlyStopped)
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation
                               | RigidbodyConstraints.FreezePositionX
                               | RigidbodyConstraints.FreezePositionZ;
            }
            else
            {
                rb.constraints = RigidbodyConstraints.FreezeRotation;
            }

            Vector3 camFrente = cameraTransform.forward;
            Vector3 camDireita = cameraTransform.right;
            camFrente.y = 0;
            camDireita.y = 0;
            camFrente.Normalize();
            camDireita.Normalize();

            moveInput = camFrente * inputDir.z + camDireita * inputDir.x;
            //moveInput.Normalize();

            if (moveInput != Vector3.zero)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }
            isMovingBU = isMoving;

        }
    }

    private void MovementHandler()
    {
        if (!isAbleToMove) return;

        Vector3 currentVel = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);

        // velocidade alvo
        Vector3 targetVel = moveInput.normalized * moveSpeed;

        // aplica diferença de velocidade (mudança direta de velocidade horizontal)
        Vector3 velDiff = targetVel - horizontalVel;
        rb.AddForce(velDiff, ForceMode.VelocityChange);
    }



    private void rotation()
    {
        bool compar = moveInput == Vector3.zero;
        if (!compar) // esta tendo input
        {
            Vector3 camRight = cameraTransform.right;
            Vector3 camForward = cameraTransform.forward;
            camRight.y = 0;
            camForward.y = 0;
            camRight.Normalize();
            camForward.Normalize();

            // Salva a direção relativa ao eixo da câmera
            lastRelativeDir = new Vector3(
                Vector3.Dot(moveInput, camRight),
                0,
                Vector3.Dot(moveInput, camForward)
            );

            transform.forward = moveInput;
        }
        else
        {
            Vector3 camRight = cameraTransform.right;
            Vector3 camForward = cameraTransform.forward;
            camRight.y = 0;
            camForward.y = 0;
            camRight.Normalize();
            camForward.Normalize();

            Vector3 adjustedLookDir = camRight * lastRelativeDir.x + camForward * lastRelativeDir.z;
            transform.forward = adjustedLookDir.normalized;
        }


    }
}
