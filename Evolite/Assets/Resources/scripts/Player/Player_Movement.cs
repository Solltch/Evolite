using System;
using System.Collections;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [Header("Movimento")]
    private float moveSpeed;
    public float bonusMoveSpeed;
    public float walkingMoveSpeed;
    public float runningMoveSpeed;
    public float sneakingMoveSpeed;
    private Vector3 lastRelativeDir;
    public Vector3 moveInput;
    public float lastMoveZ;
    public float lastMoveX;

    [Header("Dash")]
    public float dashForce = 20f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public bool canDash = true;
    private bool isDashing = false;

    [Header("iFrames")]
    public bool isInvulnerable = false;
    public float iFrameDuration = 0.15f;

    [Header("Pulo")]
    public float ray;
    public float WallRay;
    public float jumpForce;
    public float airMultiplier;

    public float jumpCoolDown;
    public bool readyToJump;

    public float jumpDamping;
    private Vector3 inercia;

    public LayerMask groundLayer;

    [Header("Ladeiras")]
    public float currentAngle;
    public float maxSlopeAngle;
    public float slopeRay;
    public float slopeGroundRay;
    public bool onSlope = false;
    private RaycastHit slopeHit;

    [Header("Comparações")]
    public bool isRunning;
    public bool isGrounded;
    public bool isFalling;
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
    public KeyCode DashKey = KeyCode.Q;

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
    public Player_General stats2;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        stats2 = GameObject.Find("Player Sprite").GetComponent<Player_General>();
        rb = GetComponent<Rigidbody>();
        cameraTransform = UnityEngine.Camera.main.transform;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        readyToJump = true;
        isAbleToMove = true;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGrounded();
        OnSlope();
        StateHandler();
        MyInputs();
        JumpInput();
        DashInput();
    }

    private void FixedUpdate()
    {
        MovementHandler();
        rotation();
    }

    private void CheckGrounded()
    {
        // Origem do ray (levemente acima do pivot para evitar colisão com o próprio colisor)
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        Debug.DrawRay(origin, Vector3.down * ray, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ray, groundLayer, QueryTriggerInteraction.Ignore))
        {
            isGrounded = true;
            slopeHit = hit; // atualiza com o hit atual

            // Calcular o ângulo DO HIT ATUAL
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);
            currentAngle = slopeAngle;

            // onSlope apenas se ângulo estiver entre um mínimo e maxSlopeAngle
            onSlope = slopeAngle > 2f && slopeAngle < maxSlopeAngle;
        }
        else
        {
            isGrounded = false;
            onSlope = false;
            currentAngle = 0f;
        }

        isSlopeGrounded = Physics.Raycast(origin, Vector3.down, slopeGroundRay, groundLayer, QueryTriggerInteraction.Ignore);
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

            //Condição para congelar X/Z: parado, está no chão, **não** em mt inclinada
            bool nearlyStopped = inputDir.sqrMagnitude < 0.01f;
            bool flatEnoughToFreeze = isGrounded && currentAngle < maxSlopeAngle;

            if (nearlyStopped && flatEnoughToFreeze)
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

            if (onSlope && isGrounded)
            {
                if (isMovingBU != isMoving)
                {
                    rb.linearVelocity = Vector3.zero;
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

        if (onSlope && currentAngle > maxSlopeAngle)
            rb.AddForce(jumpForce * slopeHit.normal, ForceMode.Impulse);
        else
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

    private void DashInput()
    {
        if (Input.GetKeyDown(DashKey) && canDash && !isDashing && !stats.isExhausted && stats2.skills.Esquiv)
        {
            stats.DashCost();
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        isInvulnerable = true;

        // direção do dash
        Vector3 dashDir;
        if (moveInput.sqrMagnitude > 0.01f)
            dashDir = moveInput.normalized;
        else
            dashDir = transform.forward;

        // remove velocidade vertical para dash limpo
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

        // aplica força instantânea
        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);

        // impede o Movement Handler de sobrescrever o dash
        isAbleToMove = false;

        // duração do dash + iFrames
        yield return new WaitForSeconds(dashDuration);

        // termina o dash
        isDashing = false;
        isAbleToMove = true;

        // termina iFrames
        yield return new WaitForSeconds(iFrameDuration - dashDuration);
        isInvulnerable = false;

        // cooldown
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    private void MovementHandler()
    {
        if (!isAbleToMove) return; // bloqueia movimento horizontal se não puder se mover

        Vector3 currentVel = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(currentVel.x, 0, currentVel.z);
        Vector3 targetVel = Vector3.zero;

        // Checa se há parede à frente
        if (moveInput.magnitude > 0.01f)
        {
            Vector3 origin = transform.position + Vector3.up * 0.5f;
            float checkDist = 0.5f; // ajuste para distância de checagem
            if (Physics.Raycast(origin, moveInput, out RaycastHit hit, checkDist, groundLayer))
            {
                float angle = Vector3.Angle(hit.normal, Vector3.up);
                if (angle > maxSlopeAngle) // parede íngreme à frente
                {
                    return; // cancela movimento horizontal
                }
            }
        }

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
            currentAngle = Vector3.Angle(hit.normal, Vector3.up);

            if (currentAngle > 2f)
            {
                onSlope = true;
                if (currentAngle <= maxSlopeAngle)
                {
                    slopeHit = hit;
                    Vector3 slopeDirection = Vector3.ProjectOnPlane(moveInput, hit.normal).normalized;
                    Debug.DrawRay(transform.position, slopeDirection);
                    if (isSlopeGrounded && isMoving && !isJumping)
                    {
                        rb.linearVelocity = slopeDirection * moveSpeed;
                        float moveAngle = Vector3.Angle(Vector3.up, slopeDirection);
                        if (moveAngle > 90)
                        {
                            float c = Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.z, 2) + Mathf.Pow(rb.linearVelocity.x, 2));
                            rb.linearVelocity -= new Vector3(0, c, 0);
                        }
                    }
                    isAbleToMove = true;
                }
                else
                {
                    slopeHit = hit;
                    isAbleToMove = false;
                    isFalling = true;

                    // direção de escorregamento
                    Vector3 slideDirection = Vector3.ProjectOnPlane(Vector3.down, hit.normal).normalized;
                    Debug.DrawRay(transform.position, slideDirection);

                    if (isSlopeGrounded && !isJumping)
                    {
                        // força de escorregamento constante
                        float slideForce = 8f;
                        rb.AddForce(slideDirection * slideForce, ForceMode.Acceleration);

                        onSlope = true;

                        rb.linearVelocity = slideDirection * moveSpeed;
                        float moveAngle = Vector3.Angle(Vector3.up, slideDirection);
                        if (moveAngle > 90)
                        {
                            float c = Mathf.Sqrt(Mathf.Pow(rb.linearVelocity.z, 2) + Mathf.Pow(rb.linearVelocity.x, 2));
                            rb.linearVelocity -= new Vector3(0, c, 0);
                        }
                    }

                    Invoke(nameof(ResetMovement), 0.15f);

                    
                }
            }

        }
    }

    public void ResetMovement()
    {
        if (currentAngle <= maxSlopeAngle)
        {
            isAbleToMove = true;
            isFalling = false;
        }
    }

    private void rotation()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0;
        camForward.Normalize();

        Vector3 camRight = cameraTransform.right;
        camRight.y = 0;
        camRight.Normalize();

        float rawX = Input.GetAxisRaw("Horizontal");
        float rawZ = Input.GetAxisRaw("Vertical");

        if (rawX != 0 || rawZ != 0)
        {
            // O jogador está se movendo:

            // 1. Calcula a direção de movimento (absoluta)
            Vector3 moveDir = camForward * rawZ + camRight * rawX;
            moveDir.y = 0;

            // 2. Salva a última direção absoluta (para Dash)
            lastRelativeDir = moveDir.normalized;

            // 3. Salva os inputs relativos para uso no estado PARADO
            lastMoveX = rawX;
            lastMoveZ = rawZ;

            // 4. Aplica a rotação instantânea
            transform.forward = lastRelativeDir;
        }
        else
        {
            // O jogador está PARADO.

            // Usamos lastRelativeDir.sqrMagnitude > 0.01f para checar se houve movimento anterior.
            if (lastRelativeDir.sqrMagnitude > 0.01f)
            {
                // Se o jogador estava se movendo, recalcule a direção desejada (targetDir)
                // usando os inputs relativos salvos (lastRawX/Z) e a rotação ATUAL da câmera.
                // Isso faz o personagem manter a orientação RELATIVA à câmera (ex: "olhando para trás")
                // e acompanhar a rotação da câmera instantaneamente (sem LERP).

                Vector3 targetDir = camForward * lastMoveZ + camRight * lastMoveX;
                targetDir.y = 0;

                if (targetDir.sqrMagnitude > 0.01f)
                {
                    transform.forward = targetDir.normalized;
                }
            }
        }
    }
}
