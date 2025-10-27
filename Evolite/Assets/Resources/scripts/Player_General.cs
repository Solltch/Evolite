using System.Collections.Generic;
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

    [Header("Customização")]
    public List<CustomPart> parts = new List<CustomPart>();

    public int part0;
    public int part1;
    public int part2;
    public int part3;
    public int part4;
    public int part5;
    public int part6;

    public bool wings;
    public bool claws;
    public bool fangs;

    void Awake()
    {
        cameraTransform = GameObject.Find("FreeLook Camera").transform;
        stats = GameObject.Find("Player Collider").GetComponent<Player_Movement>();
        stats2 = GameObject.Find("Player Attack").GetComponent<Player_Attack>();
    }

    void Update()
    {
        Quaternion gira = cameraTransform.rotation;
        gira.x = 0;
        gira.z = 0;
        transform.rotation = gira;

        pose = plr_collider.position;
        transform.position = pose;
    }

    void FixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputZ = Input.GetAxisRaw("Vertical");

        if (inputX != 0 || inputZ != 0)
        {
            lastMoveX = inputX;
            lastMoveZ = inputZ;
        }

        // Flip
        if (lastMoveX < 0)
        {
            if (transform.localScale.x > 0)
                transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else if (transform.localScale.x < 0)
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

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
                if (timer > 2) isFalling = true;
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
            animator.Play("Jumpped");
    }

    public void ChangePart()
    {
        for (int i = 0; i < parts.Count; i++)
        {
            foreach (var sr in parts[i].front)
                sr.enabled = false;
        }

        parts[0].currentIndex = part0;
    }
}
