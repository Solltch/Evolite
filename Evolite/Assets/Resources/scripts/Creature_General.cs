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
        nestPoint = transform.position;
        playerHumor = generalHumor;

        agent.acceleration = acceleration;
    }

    // Update is called once per frame
    private void Update()
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
        if (!walkPointSet) NewWalkPoint();
        else if (!isWalkingDelayed)
        {
            if (NavMesh.SamplePosition(walkPoint, out NavMeshHit hit, 1f, NavMesh.AllAreas))
                StartCoroutine(SetDestinoDelay(hit.position, Random.Range(0.1f, 2f)));
        }

        if (Vector3.Distance(transform.position, walkPoint) < 1f)
            walkPointSet = false;
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
        NavMeshHit hit;
        if (NavMesh.SamplePosition(player.position, out hit, 1f, NavMesh.AllAreas))
            agent.SetDestination(hit.position);

        agent.speed = runningMoveSpeed;
    }

    private void Attack()
    {
        attack.BaseAttack();
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
        Gizmos.DrawWireSphere(nestPoint, maxDistance);
    }
}
