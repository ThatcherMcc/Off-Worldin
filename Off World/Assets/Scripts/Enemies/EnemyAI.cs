using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("LOS")]
    [SerializeField] private float noticeRadius;
    [SerializeField] private LayerMask enviornmentMask;

    [Header("PlayerProperties")]
    [SerializeField] private Transform player;

    [Header("Enemy Idle")]
    [SerializeField] private Vector3 mainIdlePos;
    [SerializeField] private float moveDuration;
    [SerializeField] private float breakDuration;
    [SerializeField] private float maxWanderDist;
    private float minWanderDist;

    private float idleSpeed;
    private float idleMovingTimer;
    private float idleBreakTimer;
    private bool onBreak;

    public float enemySpeed;

    private bool returningToStart;

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
        if (InLineOfSight())
        {
            FollowPlayer();
        }
        else
        {
            Idle();
        }
    }

    private bool InLineOfSight()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, noticeRadius))
        {
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                return true;
            }
        }
        return false;
    }

    private void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, enemySpeed * Time.deltaTime);
    }

    private void Idle()
    {
        isReturning();

        if (!returningToStart) {
            IdleWander();
        } else {
            ReturnToStart();
        }


    }

    private void ReturnToStart()
    {
        Vector3 direction = (player.position - transform.position).normalized;
        rb.MovePosition(transform.position + direction * idleSpeed * Time.deltaTime);
    }

    private void IdleWander()
    {
        if (idleMovingTimer <= 0 && idleBreakTimer <= 0)
        {
            GetIdleDirection();
            onBreak = false;
        }
        else if (idleMovingTimer <= 0 && idleBreakTimer > 0)
        {
            onBreak = true;
            idleBreakTimer -= Time.deltaTime;
        }

        if (!onBreak)
        {
            rb.MovePosition(transform.position + idleDirection * idleSpeed * Time.deltaTime);
            idleMovingTimer -= Time.deltaTime;
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

    private void GetIdleDirection()
    {
        idleMovingTimer = moveDuration;

        float angle = Random.Range(0f, 360f);
        Vector3 randomDirection = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)).normalized;
        Vector3 targetPosition = transform.position + randomDirection * enemySpeed * idleMovingTimer;

        idleDirection = randomDirection;
        idleBreakTimer = breakDuration;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(mainIdlePos, maxWanderDist);
    }

    private bool IsValidIdleDirection(Vector3 targetPosition)
    {
        // Check if within max wander distance
        if (Vector3.Distance(mainIdlePos, targetPosition) > maxWanderDist)
        {
            return false;
        }

        // Check if line of sight to starting position
        Vector3 directionToStart = (mainIdlePos - targetPosition).normalized;
        float distanceToStart = Vector3.Distance(mainIdlePos, targetPosition);
        if (!Physics.Raycast(targetPosition, directionToStart, distanceToStart))
        {
            return true;
        }
        return false;
    }
}
