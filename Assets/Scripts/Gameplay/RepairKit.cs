using UnityEngine;

public class RepairKit : MonoBehaviour
{
    public float repairAmount = 45f; // amount to repair the player

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>(); // get the player's health
            if (playerHealth != null)
            {
                playerHealth.Repair(repairAmount); // repair the player
                FindObjectOfType<GameManager>().CollectRepairKit(); // call CollectRepairKit function from GameManager
                Destroy(gameObject); // destroy the repair kit
            } 
        }
    }
}