using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerItem : MonoBehaviour
{
    private GameObject player;
    private PlayerMovement playerMovement;

    [Header("Powerup")]
    [SerializeField] private float speedBoost = 2;

    void Start()
    {
        player = GameObject.Find("Player");
        playerMovement = player.GetComponent<PlayerMovement>();
        
    }

    public void Eat()
    {
        if (playerMovement != null)
        {
            playerMovement.walkSpeed = playerMovement.walkSpeed * speedBoost;
            playerMovement.sprintSpeed = playerMovement.sprintSpeed * speedBoost;

            Destroy(gameObject);
        }
    }
}
