using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class FrogAI : MonoBehaviour
{
    [Header("LOS")]
    [SerializeField] private float noticeRadius;

    [Header("Player Properties")]
    [SerializeField] private Transform player;

    [Header("Enemy Properties")]
    [SerializeField] private float enemySpeed;

    private bool lineOfSight = false;

    Rigidbody rb;
    SphereCollider sc;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        SphereCollider[] colliders = GetComponents<SphereCollider>();
        sc = colliders[0];
        sc.isTrigger = true;
        sc.radius = noticeRadius;
    }

    // Update is called once per frame
    void Update()
    {
        if (lineOfSight)
        {
            FollowPlayer();
        }
    }

    private void FollowPlayer()
    {
        Vector3 direction = (player.position - transform.position).normalized;

        rb.MovePosition(transform.position + direction * enemySpeed * Time.deltaTime);
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, enemySpeed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            lineOfSight = true;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        if (collider.CompareTag("Player"))
        {
            lineOfSight = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, noticeRadius/5);
    }
}
