using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class temptry : MonoBehaviour
{
    Rigidbody rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        ApplyFloating();
    }

    public float RideHeight = 2f;
    public float RideSpringStrength = 10.0f;
    public float RideSpringDamper = 5.0f;

    void ApplyFloating()
    {
        RaycastHit hit;

        // Perform the raycast
        if (Physics.Raycast(transform.position, Vector3.down, out hit))
        {
            Vector3 vel = rb.velocity;
            Vector3 rayDir = -Vector3.up;

            Vector3 otherVel = hit.rigidbody != null ? hit.rigidbody.velocity : Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if (hitBody != null)
            {
                otherVel = hitBody.velocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherDirVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherDirVel;

            float x = hit.distance - RideHeight;

            // Calculate spring force
            float springForce = (x * RideSpringStrength) - (relVel * RideSpringDamper);

            // Apply force to the player
            rb.AddForce(rayDir * springForce);

            // Apply an equal but opposite force to the hit rigidbody (if any)
            if (hit.rigidbody != null)
            {
                hit.rigidbody.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        }
    }
}
