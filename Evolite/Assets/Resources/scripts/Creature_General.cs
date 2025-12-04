using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
public class Creature_General : MonoBehaviour
{
    [Header("Status Social")]
    public int raceID;
    public State generalHumor = State.neutral;
    public State playerHumor;
    public enum State { friendly, neutral, scared, angry }
    [SerializeField] private float lastTimePlayerSeen = 0f;
    [SerializeField] private const float neutralReturnTime = 10f; // 10 segundos

    [Header("Componentes")]
    public Transform player;
    private NavMeshAgent agent;
    public Creature_Stats stats;
    public Transform cam;
    public LayerMask WIGround, WIPlayer;
    public Animator animator;

    [Header("Movimento")]
    public float walkingMoveSpeed = 2f;
    public float runningMoveSpeed = 5f;
    public float acceleration = 8f;
    public float stunTime;

    public MovementState moveState;
    public enum MovementState { walking, running }

    [Header("Patrulha")]
    public Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange = 10f;
    public Vector3 nestPoint;
    public float maxDistance = 15f;

    [Header("Ataque")]
    public Creature_Attack attack;

    [Header("Estados")]
    public float sightRange = 10f;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Comparadores")]
    public bool isWalkingDelayed;
    public float hpInFrame;
    public bool isFleeing;
    public bool isStunned;

    [Header("Animação")]
    public float lastMoveX;
    public float lastMoveZ;

    [Header("Customização")]
    public List<CustomPart> parts = new List<CustomPart>();
    public CustomPart eyePart;
    public CustomPart pupilPart;

    public int bodyAcessoriesIndex;
    public int headIndex;
    public int FaceIndex;
    public int eyeIndex;
    public int pupilIndex;

    public float headSize;
    public float eyeSize;
    public float pawSize;

    public Transform head;
    public Transform paw1;
    public Transform paw2;
    public Transform leg1;
    public Transform leg2;
    public Transform eye1;
    public Transform eye2;

    private Vector3 headAcessOrigin;
    private Vector3 eyePos;
    private Vector3 eye2Pos;
    private Vector3 pulPos;

    public Creature_Race race;

    public List<SpriteRenderer> creatureSkin1 = new List<SpriteRenderer>();
    public List<SpriteRenderer> creatureSkin2 = new List<SpriteRenderer>();
    public List<SpriteRenderer> creatureSkin3 = new List<SpriteRenderer>();
    public List<SpriteRenderer  > creatureSkin4 = new List<SpriteRenderer>();
    public List<SpriteRenderer> creatureEye = new List<SpriteRenderer>();
    public List<SpriteRenderer> creaturePupil = new List<SpriteRenderer>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    Transform FindDeepChild(Transform parent, string name)
    {
        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
            if (child.name == name) return child;
        return null;
    }

    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        animator = transform.GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponentInChildren<Creature_Stats>();
        attack = GetComponentInChildren<Creature_Attack>();
        cam = GameObject.Find("Main Camera").GetComponent<Transform>();
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            Transform nest = transform.parent.Find("Centro").GetComponent<Transform>();
            if (nest != null)
                nestPoint = nest.position;
            else
                nestPoint = transform.position;
        }
        else
            nestPoint = transform.position;
        agent.acceleration = acceleration;
        SetCourage();

        

        agent.updateRotation = false;

        if (stats.courage > 1.5f)
            generalHumor = State.angry;

        playerHumor = generalHumor;

        hpInFrame = stats.curHealth;

    }

    private void OnEnable()
    {
        Ticker.OnTickAction += Tick;
    }

    private void OnDisable()
    {
        Ticker.OnTickAction -= Tick;
    }

    //roda a cada 0.2s
    private void Tick()
    {
        if (stats.curHealth < hpInFrame)
        {
            SetHumor();
        }
        hpInFrame = stats.curHealth;

        if (agent != null)
        {
            playerInSightRange = CanSeePlayer();
            playerInAttackRange = Physics.CheckSphere(transform.position, attack.attackRange - .1f, WIPlayer);

            // --- LÓGICA DE DETECÇÃO/CONTADOR DE TEMPO ---
            if (playerInSightRange)
            {
                Debug.Log("Tempo sem ver o plr atualizado");
                lastTimePlayerSeen = Time.time; // Atualiza o tempo em que o player foi visto
            }
            // Verifica se o player não está à vista E se já passou o tempo limite E o humor não é o original
            else if (!playerInSightRange && playerHumor != generalHumor && Time.time >= lastTimePlayerSeen + neutralReturnTime)
            {
                // Volta ao humor base (neutral)
                Debug.Log("Estado resetado");
                playerHumor = generalHumor;
                isFleeing = false;
                walkPointSet = false; // Força a criatura a procurar um novo ponto de patrulha
                if (agent.isActiveAndEnabled && agent.isStopped)
                {
                    agent.isStopped = false; // Garante que o agente possa retomar a Patrulha
                }
                // Não precisa chamar Patrulha() aqui, o fluxo da Tick() fará isso abaixo.
            }
            // ---------------------------------------------

            UpdateRunState();

            if (isStunned)
            {
                if (agent.isActiveAndEnabled && !agent.isStopped)
                {
                    agent.isStopped = true;
                }
                animator.SetBool("Stunned", true);
            }
            else // Se não estiver atordoado, executa a lógica normal de IA
            {
                animator.SetBool("Stunned", false);

                if (playerInSightRange)
                {
                    if (playerHumor == State.scared)
                    {
                        // Se não estiver ATORDOADO,
                        if (!isStunned)
                        {
                            // Se a criatura ainda NÃO estiver FUGINDO (primeira vez), inicie o STUN.
                            if (!isFleeing)
                            {
                                StartCoroutine(StunBeforeFleeCoroutine());
                            }
                            // Se a criatura já NÃO estiver atordoada e JÁ ESTIVER fugindo (Fuga() foi chamada),
                            // reajusta o destino de fuga.
                            else
                            {
                                Fuga();
                            }
                        }
                    }
                    else if (playerHumor == State.angry)
                    {
                        isFleeing = false;
                        if (playerInAttackRange)
                            Attack();
                        else
                            Perseguir();
                    }
                }
                else
                {
                    // Se o player saiu da vista OU o humor voltou para neutral/friendly
                    isFleeing = false;
                    Patrulha();
                }
            }

            if (!isStunned)
            {
                RotateAgent();
            }

            RotateAgent();
        }

        if (lastMoveZ != 0)
            animator.SetFloat("Horizontal", 0);
        else
            animator.SetFloat("Horizontal", lastMoveX);

        animator.SetFloat("Horizontal", lastMoveX);
        animator.SetFloat("Vertical", lastMoveZ);
        animator.SetBool("Moving", agent.velocity.magnitude > 0.1f);
        animator.SetBool("Running", stats.isRunning);
        animator.SetBool("Attacking", attack.isAttacking);

        UpdateLastMove();
        UpdateEyesAndHeadAcess();
        ChangePart();
        Resize();
        FlipSprite();
    }

    private void UpdateRunState()
    {
        bool isMoving = agent.velocity.sqrMagnitude > 0.01f;

        bool shouldRun = isMoving && !stats.isExhausted && playerInSightRange && (playerHumor == State.scared || playerHumor == State.angry);

        if (shouldRun)
        {
            moveState = MovementState.running;
            agent.speed = runningMoveSpeed;
            stats.isRunning = true;
        }
        else
        {
            moveState = MovementState.walking;
            agent.speed = walkingMoveSpeed;
            stats.isRunning = false;
        }
    }

    private void Patrulha()
    {
        if (isFleeing || isStunned) return;

        agent.stoppingDistance = 0;

        if (!walkPointSet && !isWalkingDelayed) // Só tenta novo WP se não tiver um e não estiver em atraso
        {
            StartCoroutine(PatrolDelay());
        }

        if (walkPointSet)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(walkPoint);
            }

            // Aumentei para 1.0f para dar mais margem de erro
            if (!agent.pathPending && agent.remainingDistance < 1.0f)
            {
                walkPointSet = false;
                // Não precisa de atraso aqui, o isWalkingDelayed inicia o próximo.
            }
        }
    }

    private IEnumerator PatrolDelay()
    {
        isWalkingDelayed = true;

        // Para o agente no local por um pequeno período antes de procurar o próximo WP
        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }

        yield return new WaitForSeconds(0.8f); // Tempo de "parada" antes de escolher o novo ponto

        NewWalkPoint();

        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = false; // Retoma o movimento se o NewWalkPoint for bem-sucedido
        }

        isWalkingDelayed = false;
    }

    private IEnumerator StunBeforeFleeCoroutine()
    {
        isStunned = true;

        if (agent.isActiveAndEnabled)
        {
            agent.isStopped = true;
        }

        if (animator != null)
        {
            animator.SetTrigger("DoStun");
        }

        // 2. Espera (stunTime)
        yield return new WaitForSeconds(stunTime);

        isStunned = false;


        Fuga();
    }

    private void Fuga()
    {
        isFleeing = true;
        animator.SetBool("Stunned", false);

        agent.stoppingDistance = 0;

        if (!walkPointSet)
        {
            walkPoint = GetFleePoint();
            walkPointSet = true;
        }

        if (walkPointSet)
        {
            if (agent != null && agent.isActiveAndEnabled)
            {
                agent.isStopped = false;
                agent.SetDestination(walkPoint);
            }

            if (!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                walkPointSet = false;
            }
        }
    }


    private void NewWalkPoint()
    {
        Vector3 randomXZ = new Vector3(
            UnityEngine.Random.Range(-walkPointRange, walkPointRange),
            0,
            UnityEngine.Random.Range(-walkPointRange, walkPointRange));

        Vector3 candidate = transform.position + randomXZ;

        if (Physics.Raycast(candidate + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 30f, WIGround))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < 90f)
            {
                if (Vector3.Distance(hit.point, nestPoint) <= maxDistance)
                {
                    NavMeshPath path = new NavMeshPath();

                    if (agent.CalculatePath(hit.point, path) && path.status == NavMeshPathStatus.PathComplete)
                    {
                        walkPoint = hit.point;
                        walkPointSet = true;
                        Debug.DrawLine(transform.position, walkPoint, Color.green, 2f);
                        return;
                    }
                }
            }
        }

        walkPointSet = false;
    }

    private Vector3 GetFleePoint()
    {
        Vector3 fleeDir = (transform.position - player.position).normalized;

        Vector3 target = transform.position + fleeDir * sightRange;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(target, out hit, 3f, NavMesh.AllAreas))
            return hit.position;

        for (int i = 1; i <= 6; i++)
        {
            float angle = 25f * i;

            // Direita
            Vector3 dirRight = Quaternion.Euler(0, angle, 0) * fleeDir;
            target = transform.position + dirRight * sightRange;

            if (NavMesh.SamplePosition(target, out hit, 3f, NavMesh.AllAreas))
                return hit.position;

            // Esquerda
            Vector3 dirLeft = Quaternion.Euler(0, -angle, 0) * fleeDir;
            target = transform.position + dirLeft * sightRange;

            if (NavMesh.SamplePosition(target, out hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }

        return transform.position;
    }

    private void Perseguir()
    {
        agent.stoppingDistance = .5f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        // A velocidade é controlada por UpdateRunState(), removemos a atribuição direta
        // agent.speed = runningMoveSpeed;
    }

    private void Attack()
    {
        Debug.Log("Inimigo Atacando");
        agent.stoppingDistance = .6f;
        attack.BaseAttack();

        Vector3 direction = player.position - transform.position;

        Quaternion lookRotation = Quaternion.LookRotation(direction);

        transform.rotation = lookRotation;

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attack.attackRange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(walkPoint, 0.3f);

        Gizmos.color = Color.grey;
        if (nestPoint != null)
            Gizmos.DrawWireSphere(nestPoint, maxDistance);
        else
            Gizmos.DrawWireSphere(transform.position, maxDistance);
    }

    private void RotateAgent()
    {
        if (agent.velocity.sqrMagnitude > 0.1f) // só rotaciona se estiver se movendo
        {
            // 1. Obtém a direção de movimento
            Vector3 direction = agent.velocity.normalized;

            // 2. Cria a rotação que aponta para essa direção
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            // 3. Obtém o ângulo Y (yaw) da rotação calculada
            float yAngle = lookRotation.eulerAngles.y;

            // 4. Cria um novo Quaternion com apenas a rotação Y
            // Mantém X e Z em 0 (sem inclinação/roll ou pitch)
            transform.rotation = Quaternion.Euler(0f, yAngle, 0f);
        }
    }

    public void SetCourage()
    {

        switch (stats.type)
        {
            case Creature_Stats.State.carni:
                stats.courage = UnityEngine.Random.Range(0.5f, 2f);
                break;
            case Creature_Stats.State.herbi:
                stats.courage = UnityEngine.Random.Range(0f, 1.5f);
                break;
            case Creature_Stats.State.oni:
                stats.courage = UnityEngine.Random.Range(0f, 2f);
                break;
            default:
                stats.courage = 1f;
                break;
        }
    }

    public void SetHumor()
    {
        if (stats.courage > 1f)
        {
            playerHumor = State.angry;
        }
        else
        {
            playerHumor = State.scared;
        }
    }

    private void UpdateLastMove()
    {
        Vector3 vel = agent.transform.forward;

            Vector3 camForward = cam.forward;
            camForward.y = 0;
            camForward.Normalize();

            Vector3 camRight = cam.right;
            camRight.y = 0;
            camRight.Normalize();

            float rawZ = Vector3.Dot(vel, camForward);
            float rawX = Vector3.Dot(vel, camRight);

            const float threshold = 0.5f;

            int z = Mathf.Abs(rawZ) < threshold ? 0 : (rawZ > 0 ? 1 : -1);
            int x = Mathf.Abs(rawX) < threshold ? 0 : (rawX > 0 ? 1 : -1);

            // --- REGRA NOVA ---
            // Impede X e Z de serem diferentes de 0 ao mesmo tempo
            if (x != 0) z = 0;
            else if (z != 0) x = 0;
            // -------------------

            lastMoveZ = z;
            lastMoveX = x;
    }


    private bool CanSeePlayer()
    {
        // Verifica se há colisores do jogador (LayerMask WIPlayer) dentro do raio de visão (sightRange).
        // Isso ignora obstáculos, sendo um cálculo "mais básico".
        if (Physics.CheckSphere(transform.position, sightRange, WIPlayer))
            return true;

        return false;
    }

    private void Start()
    {
        SetupParts();
        ApplyRandomCustomization();
    }

    void SetupParts()
    {
        head = FindDeepChild(transform, "Cabeça");
        paw1 = FindDeepChild(transform, "Mão Direita");
        paw2 = FindDeepChild(transform, "Mão Esquerda");
        leg1 = FindDeepChild(transform, "Pé Direito");
        leg2 = FindDeepChild(transform, "Pé Esquerdo");
        eye1 = FindDeepChild(transform, "Olho");

        ChangePart();


        if (eye2 == null)
        {
            eye2 = Instantiate(eye1, eye1.parent);
            eye2.localScale = new Vector3(eye1.localScale.x, -eye1.localScale.y, eye1.localScale.z);
            eye2.localPosition = new Vector3(eye1.localPosition.x, -eye1.localPosition.y, eye1.localPosition.z);


            CustomPart eye2_Part = eye2.Find("Eye").GetComponent<CustomPart>();
            CustomPart pupil2_Part = eye2.Find("Pupil").GetComponent<CustomPart>();
            pupil2_Part.transform.localScale = new Vector3(1, -1, 1);
            eyePart = eye2.Find("Eye").GetComponent<CustomPart>();
            pupilPart = eye2.Find("Pupil").GetComponent<CustomPart>();

            pupilPart.transform.localScale = new Vector3(1, -1, 1);
        }

        eyePos = eyePart.transform.localPosition;
        pulPos = pupilPart.transform.localPosition;
        eye2Pos = eye2.transform.localPosition;
    }

    public void ApplyRandomCustomization()
    {
        race = transform.parent.GetComponent<Creature_Race>();

        // Estes valores são gerados de forma 100% aleatória em tempo de execução
        bodyAcessoriesIndex = race.bodyAcessoriesIndex;
        headIndex = race.headIndex;
        FaceIndex = race.FaceIndex;
        eyeIndex = race.eyeIndex;
        pupilIndex = race.pupilIndex;

        headSize = race.headSize;
        eyeSize = race.eyeSize;
        pawSize = race.pawSize;

        foreach (var spriterender in creatureSkin1)
        {
            spriterender.material = race.creatureSkin1;
        }
        foreach (var spriterender in creatureSkin2)
        {
            spriterender.material = race.creatureSkin2;
        }
        foreach (var spriterender in creatureSkin3)
        {
            spriterender.material = race.creatureSkin3;
        }
        foreach (var spriterender in creatureSkin4)
        {
            spriterender.material = race.creatureSkin4;
        }
        foreach (var spriterender in creatureEye)
        {
            spriterender.material = race.creatureEye;
        }
        foreach (var spriterender in creaturePupil)
        {
            spriterender.material = race.creaturePupil;
        }

        Resize();
        ChangePart();
    }

    public void ChangePart()
    {
        int moveZ = Convert.ToInt32(lastMoveZ);

        parts[0].SetSprite(bodyAcessoriesIndex, moveZ); // Body Acessory
        parts[1].SetSprite(headIndex, moveZ);            // Head

        // --- EYE 1 ---
        parts[2].SetSprite(eyeIndex, moveZ);            // Eye 1 Sprite
        parts[3].SetSprite(pupilIndex, moveZ);           // Pupil 1 Sprite

        // --- EYE 2 (Clone) ---
        // Usamos as referências que foram ligadas ao EYE2 no Awake.
        if (eyePart != null)
        {
            eyePart.SetSprite(eyeIndex, moveZ);          // Eye 2 Sprite (IDêntico ao Eye 1)
        }
        if (pupilPart != null)
        {
            pupilPart.SetSprite(pupilIndex, moveZ);       // Pupil 2 Sprite (IDêntico ao Pupil 1)
        }
        parts[5].SetSprite(FaceIndex, moveZ);            // Face
    }

    void Resize()
    {
        head.localScale = Vector3.one * headSize;
        paw1.localScale = Vector3.one * pawSize;
        paw2.localScale = -Vector3.one * pawSize;
        eye1.localScale = Vector3.one * eyeSize;
        eye2.localScale = new Vector3(1, -1, 1) * eyeSize;
    }

    void UpdateEyesAndHeadAcess()
    {
        if (lastMoveZ != -1)
        {
            if (transform.localScale.y > 0)
                eye2.transform.localScale = eyeSize * new Vector3(1, 1, 1);

            eye1.gameObject.SetActive(false);
            pupilPart.transform.localPosition = pulPos;

            if (lastMoveZ != 1)
            {
                eye2.gameObject.SetActive(true);
            }
            else
            {
                eye2.gameObject.SetActive(false);
                eye2.transform.localPosition = new Vector3(eye2Pos.x - 0.02f, eye2Pos.y, eye2Pos.z);
            }
        }
        else
        {
            if (transform.localScale.y > 0)
                eye2.transform.localScale = eyeSize * new Vector3(1, -1, 1);


            eye1.gameObject.SetActive(true);
            eye2.gameObject.SetActive(true);
            eye2.transform.localPosition = eye2Pos;
            eyePart.transform.localPosition = eyePos;
            pupilPart.transform.localPosition = pulPos;
        }
    }

    private void FlipSprite()
    {
        if (lastMoveX > 0)
            transform.GetChild(0).transform.localScale = new Vector3(1, transform.GetChild(0).transform.localScale.y, transform.GetChild(0).transform.localScale.z);
        else if (lastMoveX < 0f)
            transform.GetChild(0).transform.localScale = new Vector3(-1, transform.GetChild(0).transform.localScale.y, transform.GetChild(0).transform.localScale.z);
    }

    public struct PartReference
    {
        public PlayerSetPart.PlayerPartType type;
        public int id;

        public PartReference(PlayerSetPart.PlayerPartType type, int id)
        {
            this.type = type;
            this.id = id;
        }
    }

}