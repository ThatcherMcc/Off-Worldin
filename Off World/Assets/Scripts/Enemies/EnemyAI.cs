using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IEnemy
{
    [Header("LOS")]
    [SerializeField] private float noticeRadius;
    private bool chasing;
    private float chaseTimer = 5;

    [Header("PlayerProperties")]
    [SerializeField] private Transform player;

    [Header("Enemy Idle")]
    private Vector3 mainIdlePos;
    [SerializeField] private float moveDuration;
    [SerializeField] private float maxWanderDist;
    private float minWanderDist;

    private float idleSpeed;
    private float idleMovingTimer = 0;
    private float idleBreakTimer = 0;
    public float enemySpeed;

    private bool returningToStart;

    // Interface IEnemy
    public void EnableAI(bool enable)
    {
        aiEnabled = enable;
    }
    private bool aiEnabled = true;


    private Vector3 idleDirection;
    Rigidbody rb;

    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
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
                RunToPlayer();
                if (chaseTimer <= 0)
                {
                    chasing = false;
                }
                chaseTimer -= Time.deltaTime;   
            }
            else
            {
                Idle();
            }
        }
        else
        {
            Vector3 direction = (player.position - transform.position).normalized;
            rb.velocity = Vector3.zero;
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
                return true;
            }
        }
        return false;
    }

    private void RunToPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        float newSpeed = enemySpeed * 1.5f;

        MoveAndLook(direction, newSpeed);
    }

    private void Idle()
    {
        isReturning();

        if (!returningToStart)
        {
            IdleWander();
        }
        else
        {
            ReturnToStart();
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
        Vector3 direction = (mainIdlePos - transform.position).normalized;

        MoveAndLook(direction, enemySpeed);
    }

    private void IdleWander()
    {
        if (idleBreakTimer <= 0)
        {
            // Not resting or currently moving? Try moving!
            if (idleMovingTimer <= 0)
            {
                GetIdleDirection();
                moveDuration = Random.Range(6, 10);
                idleMovingTimer = moveDuration;
                idleBreakTimer = Random.Range(moveDuration - 1, moveDuration - 2);
            }
            else
            {
                MoveAndLook(idleDirection, idleSpeed);

                idleMovingTimer -= Time.deltaTime;
            }

        } else
        {
            idleBreakTimer -= Time.deltaTime;
        }
    }

    private void GetIdleDirection()
    {
        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;

        idleDirection = randomDirection;
    }

    private void MoveAndLook(Vector3 direction, float speed)
    {
        rb.MovePosition(transform.position + direction * speed * Time.deltaTime);

        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, enemySpeed * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, maxWanderDist);
    }

}
