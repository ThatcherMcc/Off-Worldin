using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody rb;
    private Transform objectGrabPointTransform;
    public bool equipped = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 13f;
            Vector3 newPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            rb.MovePosition(newPosition);
        }
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        equipped = true;
        this.objectGrabPointTransform = objectGrabPointTransform;
        rb.useGravity = false;
        rb.drag = 5;
    }

    public void Drop()
    {
        //Vector3 momentum = new Vector3()
        equipped = false;
        this.objectGrabPointTransform = null;
        rb.useGravity = true;
        rb.drag = 0;
    }
}
