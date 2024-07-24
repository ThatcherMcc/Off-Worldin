using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] Healthbar healthbar;
    public int maxHealth = 100;
    private int health;


    private void Start()
    {
        health = maxHealth;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            PlayerHeal(20);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerTakeDMG(20);
        }
    }

    public void PlayerHeal(int healing)
    {
        health += healing;
        healthbar.SetHealth(health);

        if (health > 100)
        {
            health = maxHealth;
        }
    }

    public void PlayerTakeDMG(int damage)
    {
        health -= damage;
        healthbar.SetHealth(health);

        if (health <= 0)
        {
            health = 0;
            Die();
        }
    }

    private void Die()
    {
        return;
    }
}
