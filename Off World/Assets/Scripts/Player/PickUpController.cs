using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpController : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] private Transform fpsCam;
    [SerializeField] private Transform objectGrabPointTransform;
    [SerializeField] private LayerMask pickUpLayerMask;

    [Header("Properties")]
    public float pickUpRange = 3f;
    public float dropForward, dropUpwardForce;

    [Header("Equip Status")]
    public bool isEquipped = false;

    [Header("KeyBinds")]
    public KeyCode pick = KeyCode.E;
    public KeyCode drop = KeyCode.G;
    public KeyCode eat = KeyCode.R;

    private GameObject heldObject;
    private ObjectGrabbable objectGrabbable;


    void Update()
    {
        // Pickup and Drop
        if (Input.GetKeyDown(pick))
        {
            PickUp();
        }
        if (Input.GetKeyDown(drop))
        {
            Drop();
        }
        if (Input.GetKeyDown(eat))
        {
            Eat();
        }

    }

    private void PickUp()
    {
        if (!isEquipped && objectGrabbable == null)
        {
            if (Physics.Raycast(fpsCam.position, fpsCam.forward, out RaycastHit raycastHit, pickUpRange, pickUpLayerMask))
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
    }

    private void Drop()
    {
        if (isEquipped && objectGrabbable != null)
        {
            isEquipped = false;
            objectGrabbable.Drop();
            objectGrabbable = null;
            heldObject = null;
        }
    }

    private void Eat()
    {
        if (isEquipped && heldObject.GetComponent<PowerItem>())
        {
            PowerItem powerItem = heldObject.GetComponent<PowerItem>();
            powerItem.Eat();
            isEquipped = false;
            heldObject = null;
        }
    }
}
