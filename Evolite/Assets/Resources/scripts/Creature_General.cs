using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;
public class Creature_General : MonoBehaviour
{
    [Header("Estatus Social")]
    public int raceID;
    public State generalHumor;
    public State playerHumor;
    public enum State
    {
        friendly,
        neutral,
        scared,
        angry,
    }
    

    [Header("Componentes")]
    public Transform player;
    public NavMeshAgent agent;
    public Creature_Stats stats;
    public LayerMask WIGround, WIPlayer;

    [Header("Movimento")]
    public float bonusMoveSpeed;
    public float walkingMoveSpeed;
    public float runningMoveSpeed;
    public float acelleration;



    public MovementState moveState;
    public enum MovementState
    {
        walking,
        running,
    }

    [Header("Pulo")]
    public bool isGrounded;
    public float ray;

    [Header("Patrulha")]
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;
    public Vector3 nestPoint;
    public float distanceToNest;
    public float maxDistance;

    [Header("Ataque")]
    public float AttackSpeed;
    bool alreadyAttacked;

    [Header("Stados")]
    public float sightRange;
    public float attackrange;
    public bool playerInSightRange, playerInAttackRange;

    [Header("Comparadores")]
    private bool isWalkingDelayed;
    public bool isWalking;
    public bool isRunning;
    public bool isPatroling;
    public bool isChasing;
    public bool isAttacking;
    public bool isFleeing;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        player = GameObject.Find("player_Collider").transform;
        agent = GetComponent<NavMeshAgent>();
        nestPoint = transform.position;
        generalHumor = State.neutral;
        playerHumor = generalHumor;


        agent.acceleration = acelleration;
    }

    // Update is called once per frame
    void Update()
    {
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WIPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackrange, WIPlayer);

        CheckGrounded();

        if (playerInSightRange && !playerInAttackRange)
            Perseguir();
        else if (playerInSightRange && playerInAttackRange) 
            Attack();
        else
            Patrulha();

        altura();

        StateHandler();
    }

    private void CheckGrounded()
    {
        // Origem do ray (levemente acima do pivot para evitar colisão com o próprio colisor)
        Vector3 origin = transform.position + Vector3.up * 0.1f;

        Debug.DrawRay(origin, Vector3.down * ray, Color.red);

        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, ray, WIGround, QueryTriggerInteraction.Ignore))
            isGrounded = true;
        else
        {
            isGrounded = false;
        }
    }

    private void StateHandler()
    {
        // Mode - Running
        if (isGrounded && !stats.isExhausted)
        {
            moveState = MovementState.running;
            agent.speed = runningMoveSpeed;
        }

        // Mode - Walking
        else if (isGrounded)
        {
            moveState = MovementState.walking;
            agent.speed = walkingMoveSpeed;
        }

    }

    private void Patrulha()
    {
        if (!walkPointSet)
        {
            NewWalkPoint();
        }
        else
        {
            if (!isWalkingDelayed) // evita acumular coroutines
            {
                NavMeshHit hit;
                if (NavMesh.SamplePosition(walkPoint, out hit, 1f, NavMesh.AllAreas))
                {
                    float tempo = Random.Range(0.1f, 2f);
                    StartCoroutine(SetDestinoDelay(hit.position, tempo));
                }
            }
        }

        Vector3 distanceToWP = transform.position - walkPoint;
        if (distanceToWP.magnitude < 1f)
            walkPointSet = false;

        agent.speed = walkingMoveSpeed;
    }

    private IEnumerator SetDestinoDelay(Vector3 destino, float delay)
    {
        isWalkingDelayed = true;
        yield return new WaitForSeconds(delay);

        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.SetDestination(destino);
        }
        isWalkingDelayed = false;
    }

    private void NewWalkPoint()
    {
        // gera ponto aleatório no XZ
        Vector3 randomXZ = new Vector3(
            Random.Range(-walkPointRange, walkPointRange),
            0f,
            Random.Range(-walkPointRange, walkPointRange)
        );

        Vector3 candidatePoint = transform.position + randomXZ;

        // dispara raycast de cima pra baixo pra achar o chão
        if (Physics.Raycast(candidatePoint + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, WIGround))
        {
            float slopeAngle = Vector3.Angle(hit.normal, Vector3.up);

            // aceita apenas pontos com ângulo < 90° (andável)
            if (slopeAngle < 90f)
            {
                walkPoint = hit.point;
                distanceToNest = Vector3.Distance(walkPoint, nestPoint);

                // cancela se ficar longe demais do ninho
                if (distanceToNest <= maxDistance)
                    walkPointSet = true;
                else
                    walkPointSet = false;

                Debug.DrawLine(transform.position, walkPoint, Color.green, 2f);
            }
        }
    }

    private void Perseguir()
    {
        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        agent.speed = runningMoveSpeed;
    }
    
    private void Attack()
    {
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Codigo do Atk

            alreadyAttacked = true;
            Invoke(nameof(ResetAttack), AttackSpeed);
        }
    }

    private void ResetAttack()
    {
        alreadyAttacked = false;
    }

    private void altura()
    {
        CapsuleCollider col = GetComponentInChildren<CapsuleCollider>();
        if (col != null)
        {
            // Altura total do collider
            float altura = col.height;

            // Centro do collider em relação ao transform do objeto
            Vector3 centro = col.center;

            // Direção "para baixo" do collider
            Vector3 baixo = -col.transform.up;

            // Ponto de origem do raycast (topo do collider)
            Vector3 origem = col.transform.position + col.transform.up * (altura / 2) + centro;

            // Raycast para o chão
            if (Physics.Raycast(origem, baixo, out RaycastHit hit, altura, WIGround))
            {
                agent.baseOffset = hit.point.y - transform.position.y;
            }
            Debug.DrawRay(origem, baixo, Color.magenta);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackrange);

        Gizmos.color = Color.cyan;
        Gizmos.DrawSphere(walkPoint, 0.3f);

        Gizmos.color = Color.grey;
        Gizmos.DrawWireSphere(nestPoint, maxDistance);
    }
}
