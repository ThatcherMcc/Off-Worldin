using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform objectGrabPointTransform;
    private EnemyAI enemyAI;

    private float tempSpeed;

    public bool equipped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enemyAI = GetComponent<EnemyAI>();
        tempSpeed = enemyAI.enemySpeed;
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 9f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }

    public void Capture(Transform objectGrabPointTransform)
    {
        equipped = true;
        enemyAI.enemySpeed = 0;
        this.objectGrabPointTransform = objectGrabPointTransform;
        rb.useGravity = false;
        rb.drag = 5;
    }

    public void Release()
    {
        //Vector3 momentum = new Vector3()
        equipped = false;
        enemyAI.enemySpeed = tempSpeed;
        this.objectGrabPointTransform = null;
        rb.useGravity = true;
        rb.drag = 0;
    }
}
