using System.Collections;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;
using static Unity.Collections.Unicode;
public class Creature_General : MonoBehaviour
{
    [Header("Status Social")]
    public int raceID;
    public State generalHumor = State.neutral;
    public State playerHumor;
    public enum State { friendly, neutral, scared, angry }

    [Header("Componentes")]
    public Transform player;
    private NavMeshAgent agent;
    public Creature_Stats stats;
    public LayerMask WIGround, WIPlayer;

    [Header("Movimento")]
    public float walkingMoveSpeed = 2f;
    public float runningMoveSpeed = 5f;
    public float acceleration = 8f;

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
    [HideInInspector] public bool playerInSightRange, playerInAttackRange;

    [Header("Comparadores")]
    private bool isWalkingDelayed;
    public bool isAttacking;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        player = GameObject.FindWithTag("Player")?.transform;
        agent = GetComponent<NavMeshAgent>();
        stats = GetComponentInChildren<Creature_Stats>();
        attack = GetComponentInChildren<Creature_Attack>();
        isWalkingDelayed = false;
        Transform nest = transform.parent.Find("Centro").GetComponent<Transform>();
        if (nest != null)
            nestPoint = nest.position;
        else
            nestPoint = transform.position;
        

        agent.acceleration = acceleration;
        SetCourage();

        if (stats.courage > 1.5f)
            generalHumor = State.angry;

        playerHumor = generalHumor;

    }

    // Update is called once per frame
    private void Update()
    {
        if (agent != null)
        {
            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WIPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attack.attackRange - .1f, WIPlayer);

            StateHandler();

            if (playerInSightRange)
            {
                if (playerInAttackRange) Attack();
                else Perseguir();
            }
            else Patrulha();

            RotateAgent();
        }
    }

    private void StateHandler()
    {
        // Mode - Running
        if (!stats.isExhausted)
        {
            moveState = MovementState.running;
            agent.speed = runningMoveSpeed;
            stats.isRunning = true;
        }

        // Mode - Walking
        else
        {
            moveState = MovementState.walking;
            agent.speed = walkingMoveSpeed;
            stats.isRunning = false;
        }

    }

    private void Patrulha()
{
    agent.stoppingDistance = 0;

    if (!walkPointSet)
    {
        NewWalkPoint();
    }

    if (walkPointSet)
    {
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.isStopped = false;
            agent.SetDestination(walkPoint);
        }

        // Marca como não setado quando chegou
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            walkPointSet = false;
        }
    }
}

    private IEnumerator SetDestinoDelay(Vector3 destino, float delay)
    {
        isWalkingDelayed = true;
        yield return new WaitForSeconds(delay);

        if (agent != null && agent.isActiveAndEnabled)
            agent.SetDestination(destino);
        isWalkingDelayed = false;
    }

    private void NewWalkPoint()
    {
        Vector3 randomXZ = new Vector3(Random.Range(-walkPointRange, walkPointRange), 0, Random.Range(-walkPointRange, walkPointRange));
        Vector3 candidate = transform.position + randomXZ;

        if (Physics.Raycast(candidate + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 20f, WIGround))
        {
            if (Vector3.Angle(hit.normal, Vector3.up) < 90f)
            {
                if (Vector3.Distance(hit.point, nestPoint) <= maxDistance)
                {
                    walkPoint = hit.point;
                    walkPointSet = true;
                    Debug.DrawLine(transform.position, walkPoint, Color.green, 2f);
                }
            }
        }
    }

    private void Perseguir()
    {
        agent.stoppingDistance = .5f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        agent.speed = runningMoveSpeed;
    }

    private void Attack()
    {
        agent.stoppingDistance = .6f;
        attack.BaseAttack();

        Vector3 direction = player.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);

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
            Vector3 direction = agent.velocity.normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 10f);
        }
    }

    public void SetCourage()
    {

        switch (stats.type)
        {
            case Creature_Stats.State.carni:
                stats.courage = Random.Range(0.5f, 2f);
                break;
            case Creature_Stats.State.herbi:
                stats.courage = Random.Range(0f, 1.5f);
                break;
            case Creature_Stats.State.oni:
                stats.courage = Random.Range(0f, 2f);
                break;
            default:
                stats.courage = 1f;
                break;
        }
    }

    public void SetHumor()
    {
        if(stats.courage > 1f)
        {
            playerHumor = State.angry;
        }
        else
        {
            playerHumor = State.scared;
        }
    }
}
