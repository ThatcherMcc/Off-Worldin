using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class WolfAI : MonoBehaviour, IEnemy
{
    [Header("LOS")]
    [SerializeField] private float noticeRadius;
    private bool chasing;
    private float chaseTimer;

    public Transform player { get; set; }
    private NavMeshAgent agent;

    [Header("Idle")]
    private Vector3 mainIdlePos;
    private float moveDuration;
    [SerializeField] private float maxWanderDist;
    private float minWanderDist;

    private float idleSpeed;
    private float idleMovingTimer = 0;
    private float idleBreakTimer = 0;
    public float enemySpeed;

    private bool returningToStart;
    [Header("Movement State")]
    public MovementState state;

    public enum MovementState
    {
        attacking, idle, returning
    }

    // Interface IEnemy
    public void EnableAI(bool enable)
    {
        aiEnabled = enable;
    }
    private bool aiEnabled = true;


    private Vector3 idleDirection;

    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        mainIdlePos = transform.position;
        idleSpeed = enemySpeed / 2;
        minWanderDist = maxWanderDist / 3;
    }

    // Update is called once per frame
    void Update()
    {
        if (aiEnabled)
        {
            if (InLineOfSight() || chasing)
            {
                ChasePlayer();
                // sets a timer to continue following for a second without LOS
                if (chaseTimer <= 0)
                {
                    chasing = false;
                }
                else
                {
                    chaseTimer -= Time.deltaTime;
                }
            }
            else
            {
                Idle();
            }
        }
        else
        {
            agent.speed = 0;
            Vector3 direction = (player.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = rotation;
        }
    }

    private bool InLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, noticeRadius))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                chasing = true;
                chaseTimer = 2;
                return true;
            }
        }
        return false;
    }

    private void ChasePlayer()
    {
        agent.speed = enemySpeed;
        agent.SetDestination(player.position);
        state = MovementState.attacking;
    }

    private void Idle()
    {
        agent.speed = idleSpeed;

        isReturning();
        if (!returningToStart)
        {
            IdleWander();
            state = MovementState.idle;
        }
        else
        {
            ReturnToStart();
            state = MovementState.returning;
        }

    }

    private void isReturning()
    {
        if (Vector3.Distance(transform.position, mainIdlePos) > maxWanderDist)
        {
            returningToStart = true;
        }
        else if (Vector3.Distance(transform.position, mainIdlePos) < minWanderDist)
        {
            returningToStart = false;
        }
    }

    private void ReturnToStart()
    {
        MoveMob(mainIdlePos);
    }

    private void IdleWander()
    {
        if (idleBreakTimer <= 0)
        {
            // Not resting or currently moving? Try moving!
            if (idleMovingTimer <= 0)
            {
                moveDuration = Random.Range(4f, 7f);
                GetIdleDirection();

                idleMovingTimer = moveDuration;
                idleBreakTimer = 3;
            }
            else
            {
                MoveMob(idleDirection);
                // Debug.Log(moveDuration);
                idleMovingTimer -= Time.deltaTime;
            }

        }
        else
        {
            agent.speed = idleSpeed;
            // Debug.Log("Break tIme");
            idleBreakTimer -= Time.deltaTime;
        }
    }

    private void GetIdleDirection()
    {
        float angle = Random.Range(0f, 2f * Mathf.PI);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;

        float moveDistance = idleSpeed * moveDuration;

        idleDirection = transform.position + (randomDirection * moveDistance);
        //  Debug.Log(idleDirection);
    }

    private void MoveMob(Vector3 position)
    {
        agent.SetDestination(position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxWanderDist);
    }

}
