using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public float maxHealth = 100f; // player's max health
    public float currentHealth; // player's current health

    void Start()
    {
        currentHealth = maxHealth; // set current health to max health
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount; // subtract amount from current health
        if (currentHealth <= 0)
        {
            currentHealth = 0; // if current health is less than or equal to 0, set current health to 0 
            Debug.Log("Player is dead"); // log "Player is dead"
            // FindObjectOfType<GameManager>().GameOver(); // call GameOver function from GameManager
        }
    }

    public void Repair(float amount)
    {
        currentHealth += amount; // add amount to current health
        if (currentHealth > maxHealth) 
        {
            currentHealth = maxHealth; // if current health is greater than max health, set current health to max health
        }
    }
}
