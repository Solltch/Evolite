using UnityEngine;

public class Player_General : MonoBehaviour
{
    public Animator animator;
    public float lastMoveX;
    public float lastMoveZ;
    public Player_Movement stats;
    public Player_Attack stats2;
    public Vector3 pose;

    public Transform cameraTransform;
    public Transform plr_collider;
    public float timer;

    public bool isMoving;
    public bool isRunning;
    public bool isJumping;
    public bool isFalling;
    public bool isAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        cameraTransform = GameObject.Find("Camera").GetComponent<Transform>();
        stats = GameObject.Find("Player Collider").GetComponent<Player_Movement>();
        stats2 = GameObject.Find("Player Attack").GetComponent<Player_Attack>();
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion gira = cameraTransform.rotation;
        gira.x = 0;
        gira.z = 0;
        transform.rotation = gira;

        pose = plr_collider.transform.position;

        transform.position = pose;
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

        //se eu aperto 'a'
        if (lastMoveX < 0)
        {
            //ele verifica se já ta flipado
            if (transform.localScale.x > 0)
                //flipa
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
        //se nãot tiver
        else
            if (transform.localScale.x < 0)
                transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
            else
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);

        isMoving = stats.isMoving;
        isRunning = stats.isRunning;
        isAttacking = stats2.isAttacking;

        if (isJumping)
        {
            if (stats.isGrounded)
                isJumping = false;
            else
                return;
        }
        else
            isJumping = stats.isJumping;




        if (!stats.isGrounded && !isJumping)
        {
            isFalling = true;
        }
        else
        {
            if (isJumping && !stats.isGrounded)
            {
                timer += Time.fixedDeltaTime;
                if (timer > 2)
                {
                    isFalling = true;

                }
            }
            else
                timer = 0;
            isFalling = false;
        }

       if (lastMoveZ != 0)
            animator.SetFloat("Horizontal", 0);
        else
            animator.SetFloat("Horizontal", lastMoveX);

        animator.SetBool("Jumping", isJumping);
        animator.SetFloat("Vertical", lastMoveZ);
        animator.SetBool("Falling", isFalling);
        animator.SetBool("Moving", isMoving);
        animator.SetBool("Running", isRunning);
        
        animator.SetBool("Attacking", stats2.isAttacking);

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Jump") && !isJumping)
        {
            animator.Play("Jumpped");
        }

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


    }

}
