using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform fpsCam;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private Transform objectGrabPointOffHandTransform;

    [Header("Type2Grab")]
    [SerializeField] private LayerMask pickUpLayerMask;
    [SerializeField] private LayerMask enemyPickUpLayerMask;

    [Header("Properties")]
    public float pickUpRange = 3f;
    public float pickUpRadius = .5f;
    public float dropForwardForce, dropUpwardForce;

    [Header("Equip Status")]
    public bool isEquipped = false;
    public bool isEquippedOffHand = false;

    [Header("KeyBinds")]
    public KeyCode pick;
    public KeyCode drop;
    public KeyCode eat;

    private GameObject heldObject;
    private GameObject heldObjectOffHand;

    private ObjectGrabbable objectGrabbable;
    private EnemyGrabbable enemyGrabbable;


    void Update()
    {
        // Pickup and Drop
        if (Input.GetKeyDown(pick) && !isEquipped && objectGrabbable == null)
        {
            PickUp();
        }
        if (Input.GetKeyDown(drop) && isEquipped && objectGrabbable != null)
        {
            Drop();
        }

        if (Input.GetKeyDown(eat) && isEquipped && heldObject != null)
        {
            Eat();
        }
        if (Input.GetMouseButtonDown(0))
        {
            UseAction();
        }
        if (Input.GetMouseButtonDown(1))
        {
            UseAltAction();
        }

    }

    private void PickUp()
    {
        if (Physics.SphereCast(fpsCam.position, pickUpRadius, fpsCam.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ObjectGrabbable newObjectGrabbable))
            {
                isEquipped = true;
                objectGrabbable = newObjectGrabbable;
                heldObject = objectGrabbable.gameObject;
                objectGrabbable.Grab(objectGrabPointTransform);
            }
        }
    }

    private void Drop()
    {
        Rigidbody rb = heldObject.GetComponent<Rigidbody>();

        isEquipped = false;
        objectGrabbable.Drop();

        rb.velocity = fpsCam.gameObject.GetComponentInParent<Rigidbody>().velocity;
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);

        objectGrabbable = null;
        heldObject = null;
    }

    private void Eat()
    {
        PowerItemI powerItem = heldObject.GetComponent<PowerItemI>() as PowerItemI;
        if (powerItem != null)
        {
            powerItem.Eat();
            isEquipped = false;
            heldObject = null;
        }
    }

    private void UseAction()
    {
        if (isEquipped && heldObject.GetComponent<NetScript>())
        {
            if (Physics.SphereCast(fpsCam.position, pickUpRadius, fpsCam.forward, out RaycastHit raycastHit, pickUpRange, enemyPickUpLayerMask))
            {
                if (raycastHit.transform.TryGetComponent(out EnemyGrabbable newEnemyGrabbable))
                {
                    isEquippedOffHand = true;
                    enemyGrabbable = newEnemyGrabbable;
                    heldObjectOffHand = enemyGrabbable.gameObject;
                    enemyGrabbable.Capture(objectGrabPointOffHandTransform);
                }
            }
        }
    }

    private void UseAltAction()
    {
        if (isEquippedOffHand)
        {
            isEquippedOffHand = false;
            enemyGrabbable.Release();
            enemyGrabbable = null;
            heldObjectOffHand = null;
        }
    }
}
