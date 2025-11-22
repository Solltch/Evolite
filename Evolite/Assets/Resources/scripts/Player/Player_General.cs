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
    [Range(-1, 20)] public int headAcessoriesIndex;
    [Range(-1, 20)] public int eyeIndex;
    [Range(-1, 20)] public int pupilIndex;
    [Range(-1, 20)] public int part4;
    [Range(-1, 20)] public int part5;
    [Range(-1, 20)] public int part6;
    
    [Range(0.5f, 2)] public float headSize = 1;
    public Transform head;
    [Range(0.5f, 2)] public float headAcessSize = 1;
    public Transform headAcess;
    [Range(0.5f, 2)] public float pawSize = 1;
    public Transform paw1;
    public Transform paw2;
    [Range(0.5f, 2)] public float eyeSize = 1;
    public Transform eye1;
    public Transform eye2;
    [Range(0.5f, 2)] public float feetSize = 1;
    public Transform leg1;
    public Transform leg2;

    public Vector3 headAcessOrigin;
    [Range(2, 2)] public float headAcessX = 1;
    [Range(2, 2)] public float headAcessY = 1;
    [Range(2, 2)] public float headAcessZ = 1;


    public bool Carniv;
    public bool Herbiv;
    public bool Presas;
    public bool Garras;
    public bool Abraco;
    public bool Dieta;
    public bool Resist;
    public bool Gigant;
    public bool Apex;
    public bool Carnic;
    public bool Regen;
    public bool Olhos;
    public bool Escond;
    public bool PatasA;
    public bool Flor;
    public bool Couro;
    public bool Esquiv;
    public bool Salto;
    public bool Chifre;
    public bool Casco;
    public bool Espinh;
    public bool Titan;
    public bool Presis;
    public bool Veneno;
    public bool Celere;
    public bool Coloss;
    public bool Ecoal;
    public bool Furtiv;
    public bool Invisi;


    public Vector3 eyePos;
    public Vector3 pulPos;
    public Vector3 eye2Pos;

    public Slider cabeSize;
    public Slider cabeAcessSize;
    public Slider cabeAcessY;
    public Slider cabeAcessX;
    public Slider cabeAcessZ;
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
        headAcess = GameObject.Find("Head Acessory").GetComponent<Transform>();
        paw1 = GameObject.Find("Mão Direita").GetComponent<Transform>();
        paw2 = GameObject.Find("Mão Esquerda").GetComponent<Transform>();
        leg1 = GameObject.Find("Pé Direito").GetComponent<Transform>();
        leg2 = GameObject.Find("Pé Esquerdo").GetComponent<Transform>();
        eye1 = GameObject.Find("Olho").GetComponent<Transform>();

        headAcessOrigin = headAcess.localPosition;

        eye2 = Instantiate(eye1, eye1.parent);
        eye2.localScale = new Vector3(eye1.localScale.x, -eye1.localScale.y, eye1.localScale.z);
        eye2.localPosition = new Vector3(eye1.localPosition.x, -eye1.localPosition.y, eye1.localPosition.z);
        eyePart = eye2.Find("Eye").GetComponent<CustomPart>();
        pupilPart = eye2.Find("Pupil").GetComponent<CustomPart>();
        pupilPart.transform.localScale = new Vector3(1, -1, 1);

        eyePos = eyePart.transform.localPosition;
        pulPos = pupilPart.transform.localPosition;
        eye2Pos = eye2.transform.localPosition;

        cabeSize = GameObject.Find("Cabesize Slider").GetComponent<Slider>();
        cabeAcessSize = GameObject.Find("CabeAcesssize Slider").GetComponent<Slider>();
        cabeAcessX = GameObject.Find("XAcess Slider").GetComponent<Slider>();
        cabeAcessY = GameObject.Find("YAcess Slider").GetComponent<Slider>();
        cabeAcessZ = GameObject.Find("ZAcess Slider").GetComponent<Slider>();
        //olhoSize = GameObject.Find("Olhosize Slider").GetComponent<Slider>();
        //pataSize = GameObject.Find("Patasize Slider").GetComponent<Slider>();
        //peSize = GameObject.Find("Pesize Slider").GetComponent<Slider>();
        characterSlider = GameObject.Find("Character Slider").GetComponent<Slider>();

        headSize = cabeSize.value;
        headAcessSize = cabeAcessSize.value;
        headAcessX = cabeAcessX.value;
        headAcessY = cabeAcessY.value;
        headAcessZ = cabeAcessZ.value;

        if (lastMoveZ != -1)
        {
            headAcess.localPosition = new Vector3(headAcessOrigin.x + headAcessY - 0.02f, headAcessOrigin.y + headAcessZ, headAcess.localPosition.z);
        }
        else
        {
            headAcess.localPosition = new Vector3(headAcessOrigin.x + headAcessY, headAcessOrigin.y + headAcessX, headAcess.localPosition.z);
        }
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
            headSize = cabeSize.value;
            headAcessSize = cabeAcessSize.value;
            headAcessX = cabeAcessX.value;
            headAcessY = cabeAcessY.value;
            headAcessZ = cabeAcessZ.value;
            //eyeSize = olhoSize.value;
            //pawSize = pataSize.value;
            //feetSize = peSize.value;


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
            if(lastMoveZ != 1)
                headAcess.localPosition = new Vector3(headAcessOrigin.x + headAcessY - 0.035f, headAcessOrigin.y + headAcessZ, headAcess.localPosition.z);
            else
                headAcess.localPosition = new Vector3(headAcessOrigin.x + headAcessY - 0.02f, headAcessOrigin.y + headAcessOrigin.y + headAcessX, headAcess.localPosition.z);

        }
        else
        {
            if (transform.localScale.y > 0)
                eye2.transform.localScale = eyeSize * new Vector3(1, -1, 1);

            headAcess.localPosition = new Vector3(headAcessOrigin.x + headAcessY, headAcessOrigin.y + headAcessX, headAcess.localPosition.z);

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
            if (transform.localScale.x > 0 && lastMoveZ == 0)
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
        parts[4].SetSprite(headAcessoriesIndex, Convert.ToInt32(lastMoveZ));
    }

    public void Resize()
    {
        head.localScale = Vector3.one * headSize;
        headAcess.localScale = Vector3.one * headAcessSize;
        paw1.localScale = Vector3.one * pawSize;
        paw2.localScale = -Vector3.one * pawSize;
        eye1.localScale = Vector3.one * eyeSize;
        eye2.localScale = new Vector3(1, -1, 1) * eyeSize;
        leg1.localScale = Vector3.one * feetSize;
        leg2.localScale = new Vector3(1, -1, 1) * feetSize;
    }
}
