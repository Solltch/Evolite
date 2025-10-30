using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool isCustomizing;

    [Header("Customização")]
    public CustomPart eyePart;
    public CustomPart pupilPart;
    public List<CustomPart> parts = new List<CustomPart>();
    
    [Range(-1, 20)] public int bodyAcessoriesIndex;
    [Range(-1, 20)] public int headIndex;
    [Range(-1, 20)] public int eyeIndex;
    [Range(-1, 20)] public int pupilIndex;
    [Range(-1, 20)] public int part4;
    [Range(-1, 20)] public int part5;
    [Range(-1, 20)] public int part6;
    
    [Range(0.5f, 2)] public float headSize = 1;
    public Transform head;
    [Range(0.5f, 2)] public float pawSize = 1;
    public Transform paw1;
    public Transform paw2;
    [Range(0.5f, 2)] public float eyeSize = 1;
    public Transform eye1;
    public Transform eye2;
    [Range(0.5f, 2)] public float feetSize = 1;
    public Transform leg1;
    public Transform leg2;


    public bool wings;
    public bool claws;
    public bool fangs;

    public Vector3 eyePos;
    public Vector3 pulPos;
    public Vector3 eye2Pos;

    public Slider acessorios;
    public Slider cabecas;
    public Slider olhos;
    public Slider pupilas;

    public Slider cabeSize;
    public Slider olhoSize;
    public Slider pataSize;
    public Slider peSize;
    public Slider characterSlider;

    void Awake()
    {
        cameraTransform = GameObject.Find("FreeLook Camera").transform;
        stats = GameObject.Find("Player Collider").GetComponent<Player_Movement>();
        stats2 = GameObject.Find("Player Attack").GetComponent<Player_Attack>();
        lastMoveZ = 1;

        head = GameObject.Find("Cabeça").GetComponent<Transform>();
        paw1 = GameObject.Find("Mão Direita").GetComponent<Transform>();
        paw2 = GameObject.Find("Mão Esquerda").GetComponent<Transform>();
        leg1 = GameObject.Find("Pé Direito").GetComponent<Transform>();
        leg2 = GameObject.Find("Pé Esquerdo").GetComponent<Transform>();
        eye1 = GameObject.Find("Olho").GetComponent<Transform>();


        eye2 = Instantiate(eye1, eye1.parent);
        eye2.localScale = new Vector3(eye1.localScale.x, -eye1.localScale.y, eye1.localScale.z);
        eye2.localPosition = new Vector3(eye1.localPosition.x, -eye1.localPosition.y, eye1.localPosition.z);
        eyePart = eye2.Find("Eye").GetComponent<CustomPart>();
        pupilPart = eye2.Find("Pupil").GetComponent<CustomPart>();
        pupilPart.transform.localScale = new Vector3(1, -1, 1);

        eyePos = eyePart.transform.localPosition;
        pulPos = pupilPart.transform.localPosition;
        eye2Pos = eye2.transform.localPosition;

        cabecas = GameObject.Find("Cabeça Slider").GetComponent<Slider>();
        olhos = GameObject.Find("Olho Slider").GetComponent<Slider>();
        pupilas = GameObject.Find("Pupila Slider").GetComponent<Slider>();
        acessorios = GameObject.Find("Acessorio Slider").GetComponent<Slider>();
        cabeSize = GameObject.Find("Cabesize Slider").GetComponent<Slider>();
        olhoSize = GameObject.Find("Olhosize Slider").GetComponent<Slider>();
        pataSize = GameObject.Find("Patasize Slider").GetComponent<Slider>();
        peSize = GameObject.Find("Pesize Slider").GetComponent<Slider>();
        characterSlider = GameObject.Find("Character Slider").GetComponent<Slider>();
    }

    void Update()
    {
        Quaternion gira = cameraTransform.rotation;
        gira.x = 0;
        gira.z = 0;
        transform.rotation = gira;

        pose = plr_collider.position;
        transform.position = pose;

        ChangePart();

        if (isCustomizing)
        {
            bodyAcessoriesIndex = Convert.ToInt32(acessorios.value - 1);
            headIndex = Convert.ToInt32(cabecas.value - 1);
            eyeIndex = Convert.ToInt32(olhos.value - 1);
            pupilIndex = Convert.ToInt32(pupilas.value - 1);

            headSize = cabeSize.value;
            eyeSize = olhoSize.value;
            pawSize = pataSize.value;
            feetSize = peSize.value;

            if (characterSlider != null)
            {
                switch (characterSlider.value)
                {
                    case 0:
                        lastMoveX = 1;
                        lastMoveZ = 0;
                        break;
                    case 1:
                        lastMoveX = 0;
                        lastMoveZ = -1;
                        break;
                    case 2:
                        lastMoveX = -1;
                        lastMoveZ = 0;
                        break;
                    case 3:
                        lastMoveX = 0;
                        lastMoveZ = 1;
                        break;
                }

                if (lastMoveX < 0)
                {
                    if (transform.localScale.x > 0)
                        transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
                }
                else if (transform.localScale.x < 0)
                    transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);

                if (lastMoveZ != 0)
                    animator.SetFloat("Horizontal", 0);
                else
                    animator.SetFloat("Horizontal", lastMoveX);

                animator.SetFloat("Vertical", lastMoveZ);
            }

            Resize();

            if (isAttacking)
            {
                stats.isAbleToMove = false;
                Invoke(nameof (stats.ResetMovement), stats2.baseAttackSpeed);
            }

        }

        if (lastMoveZ != -1)
        {
            if (transform.localScale.y > 0)
                eye2.transform.localScale = eyeSize * new Vector3(1, 1, 1);

            eye1.gameObject.SetActive(false);
            eye2.transform.localPosition = new Vector3(eye2Pos.x - 0.02f, eye2Pos.y, eye2Pos.z);
            pupilPart.transform.localPosition = pulPos + new Vector3(0f, 0f, 0);

            
        }
        else
        {
            if (transform.localScale.y > 0)
                eye2.transform.localScale = eyeSize * new Vector3(1, -1, 1);

            eye1.gameObject.SetActive(true);
            eye2.transform.localPosition = eye2Pos;
            eyePart.transform.localPosition = eyePos;
            pupilPart.transform.localPosition = pulPos;
        }

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
        parts[0].SetSprite(bodyAcessoriesIndex, Convert.ToInt32(lastMoveZ));
        parts[1].SetSprite(headIndex, Convert.ToInt32(lastMoveZ));
        parts[2].SetSprite(eyeIndex, Convert.ToInt32(lastMoveZ));
        eyePart.SetSprite(eyeIndex, Convert.ToInt32(lastMoveZ));
        parts[3].SetSprite(pupilIndex, Convert.ToInt32(lastMoveZ));
        pupilPart.SetSprite(pupilIndex, Convert.ToInt32(lastMoveZ));
    }

    public void Resize()
    {
        head.localScale = Vector3.one * headSize;
        paw1.localScale = Vector3.one * pawSize;
        paw2.localScale = -Vector3.one * pawSize;
        eye1.localScale = Vector3.one * eyeSize;
        eye2.localScale = new Vector3(1, -1, 1) * eyeSize;
        leg1.localScale = Vector3.one * feetSize;
        leg2.localScale = new Vector3(1, -1, 1) * feetSize;
    }
}
