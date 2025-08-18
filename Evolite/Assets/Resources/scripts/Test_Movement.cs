using UnityEngine;
using static UnityEngine.UI.Image;

public class Test_Movement : MonoBehaviour
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
    public float jumpForce;
    public float airMultiplier;

    public float jumpCoolDown;
    public bool readyToJump;
    
    public float jumpDamping;
    private Vector3 inercia;

    public LayerMask groundLayer;

    [Header("Ladeiras")]
    public float maxSlopeAngle;
    public float slopeRay;
    public bool onSlope = false;
    private RaycastHit slopeHit;
    private Vector3 groundNormal = Vector3.up;

    [Header("Comparações")]
    public bool isRunning;
    public bool isGrounded;
    public bool isSneaking;
    public bool isMoving;
    private bool isMovingBU;
    public bool isJumping;
    public bool testCheck;

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
        readyToJump = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        StateHandler();
        MyInputs();
        JumpInput();
        OnSlope();

        rotation();
    }

    private void FixedUpdate()
    {
        MovementHandler();
    }

    private void CheckGrounded()
    {
        float slopeAngle = Vector3.Angle(slopeHit.normal, Vector3.up);

        float minRay = 0.2f;
        float maxRay = 0.4f;
        float adjustedRay = Mathf.Lerp(minRay, maxRay, slopeAngle / maxSlopeAngle);

        Debug.DrawRay(transform.position + Vector3.up * 0.1f, Vector3.down * adjustedRay, Color.green);

        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, adjustedRay, groundLayer))
        {
            isGrounded = true;
            slopeHit = hit;
            onSlope = slopeAngle > 0.1f && slopeAngle < maxSlopeAngle;
        }
        else
        {
            isGrounded = false;
            onSlope = false;
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
        else
        {
            state = MovementState.air;
            if (moveSpeed == sneakingMoveSpeed)
            {
                moveSpeed = sneakingMoveSpeed * airMultiplier;
            }
            else if (moveSpeed == walkingMoveSpeed)
            {
                moveSpeed = walkingMoveSpeed * airMultiplier;
            }
            else
            {
                moveSpeed = runningMoveSpeed * airMultiplier;
            }
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

            if (inputDir.sqrMagnitude < 0.01f && isGrounded)
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
            moveInput.Normalize();

            if (moveInput != Vector3.zero)
            {
                isMoving = true;
            }
            else
            {
                isMoving = false;
            }

            if (onSlope && isGrounded)
            {
                if (isMovingBU != isMoving)
                {
                    ZerarVelocity();
                }
            }

            isMovingBU = isMoving;

        }
    }

    private void JumpInput()
    {
        if (Input.GetKey(jumpKey) && isGrounded && readyToJump && !stats.isExhausted)
        {
            readyToJump = false;

            Jumping();
            stats.JumpCost();

            Invoke(nameof(ResetJump), jumpCoolDown);
        }
    }

    private void Jumping()
    {
        isJumping = true;
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        inercia = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

        Invoke(nameof(EndJumpBuffer), 0.15f); // 150ms de buffer para evitar override da rampa
        Invoke(nameof(ResetJump), jumpCoolDown);
    }

    private void EndJumpBuffer()
    {
        isJumping = false;
    }

    private void ResetJump()
    {
        readyToJump = true;
        isJumping = false;
    }

    private void MovementHandler()
    {
        Vector3 currentVel = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);
        Vector3 targetVel = Vector3.zero;

        if (isGrounded)
        {
            targetVel = moveInput * moveSpeed;
            inercia = targetVel;
            
        }
        else
        {
            if (moveInput.magnitude > 0.1f)
            {
                Vector3 desiredVel = moveInput * moveSpeed;
                inercia = Vector3.Lerp(inercia, desiredVel, Time.fixedDeltaTime * jumpDamping);
            }
            targetVel = inercia;
        }

        Vector3 velDiff = targetVel - horizontalVel;
        rb.AddForce(velDiff, ForceMode.VelocityChange);

        
    }

    private void OnSlope()
    {
        onSlope = false;
        Vector3 velocity = rb.linearVelocity;
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit hit, slopeRay, groundLayer))
        {
            float angle = Vector3.Angle(hit.normal, Vector3.up);

            if (angle > 2f)
            {
                if (angle <= maxSlopeAngle && hit.normal != Vector3.up)
                {
                    slopeHit = hit;
                    onSlope = true;
                    Vector3 slopeDirection = Vector3.ProjectOnPlane(moveInput, hit.normal).normalized;
                    Debug.DrawRay(transform.position, slopeDirection);
                    if (isGrounded)
                        if (isMoving)
                            if (!isJumping)
                            {
                                rb.linearVelocity = slopeDirection * moveSpeed;
                                float moveAngle = Vector3.Angle(Vector3.up, slopeDirection);
                                if (moveAngle > 90)
                                {
                                    float a = rb.linearVelocity.z;
                                    float b = rb.linearVelocity.x;
                                    float c = Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.z, 2) + Mathf.Pow(rb.linearVelocity.x, 2));
                                    rb.linearVelocity -= new Vector3(0, c, 0);
                                }
                            }
                }
                else
                {
                    if (isGrounded)
                    {
                        SlideHandler();
                    }
                }
            }
            
        }
    }

    private void ZerarVelocity()
    {
        rb.linearVelocity = Vector3.zero;
    }

    private void SlideHandler()
    {

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
