using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScan : MonoBehaviour
{
    private GameObject itemScannable;
    private GameObject powerItem;

    private ObjectGrabbable objectGrabbable;

    private void OnTriggerStay(Collider collider)
    {
        if (collider.GetComponent<ItemScannable>())
        {
            ScanItem(collider);
        }
    }

    private void ScanItem(Collider collider)
    {
        itemScannable = collider.gameObject;
        powerItem = itemScannable.GetComponent<ItemScannable>().powerItem;
        objectGrabbable = itemScannable.GetComponent<ObjectGrabbable>();

        if (objectGrabbable.equipped == false)
        {
            Destroy(itemScannable);
            Instantiate(powerItem, transform.position, transform.rotation);
        }
    }
}
