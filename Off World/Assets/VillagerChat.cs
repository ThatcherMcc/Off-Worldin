using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VillagerChat : MonoBehaviour
{
    // variables
    private float interactionDistance;
    private bool inRange;
    [SerializeField] private GameObject popUp;

    // when player enters radius around villager, interaction pops up
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
        }
    }

    // keep track of which villager i am and purpose
    // probably through a set of scripts with all the information
    // and this script just uses the info from that script

    // OpenChat()
    // opens the chat pop up using the info

    // CloseChat()
    // closes chat pop up
}
