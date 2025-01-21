using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerChatController : MonoBehaviour
{
    [SerializeField] private Transform fpsCam;
    [SerializeField] private LayerMask villagerLayerMask;
    private GameObject villager;
    private float castRadius;
    private float chatDistance;
    

    // Update is called once per frame
    void Update()
    {
        if (Physics.SphereCast(fpsCam.position, castRadius, fpsCam.forward, out RaycastHit raycastHit, chatDistance, villagerLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out VillagerChat villagerChat))
            {
                
            }
        }
    }
}
