using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float timeLeft = 45f; // 45 seconds
    private int repairKitCount = 0; // Counter for collected repair kits
    private int lastSpawnIndex = -1; // Last spawn index for repair kits

    public PlayerHealth playerHealth; // Reference to the PlayerHealth script
    public GameObject repairKitPrefab; // Reference to the repair kit prefab
    public Transform[] repairKitSpawnPoints; // Array of repair kit spawn points

    private Coroutine flashCoroutine; // Reference to the flash coroutine

    public Text timerText; // Reference to the timer Text UI element
    public Image lightningBolt; // Reference to the lightning bolt Image
    public Text balanceAmountText; // Reference to the balance amount Text UI element
    public Text scoreAmountText; // Reference to the score amount Text UI element

    private Color darkGreen; // Dark green color
    private Color orange; // Orange color
    private Color red; // Red color

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>(); // Find the PlayerHealth script
        }
        if (lightningBolt != null)
        {
            lightningBolt.enabled = false; // Hide the lightning bolt
        }
        ColorUtility.TryParseHtmlString("#024b30", out darkGreen); // Dark green hex code
        ColorUtility.TryParseHtmlString("#ff9000", out orange); // Orange hex code
        ColorUtility.TryParseHtmlString("#cc0000", out red); // Red hex code
        SpawnRepairKit(); // Spawn the first repair kit
        UpdateTimerUI(); // Update the timer display
        UpdateBalanceUI(); // Update the balance display
        UpdateScoreUI(); // Update the score display
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime; // Decrease the time left
            UpdateTimerUI(); // Update the timer display
            float healthDecreaseRate = playerHealth.maxHealth / 45f * Time.deltaTime; // Calculate the health decrease rate
            playerHealth.TakeDamage(healthDecreaseRate); // Decrease the player's health
        }
        else
        {
            GameOver(); // Call the GameOver function
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60); // Calculate the minutes
        int seconds = Mathf.FloorToInt(timeLeft % 60); // Calculate the seconds
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds); // Update the timer display
        if (timeLeft > 30)
        {
            timerText.color = darkGreen; // Set the timer color to dark green
            StopFlashingLightningBolt(); // Stop flashing the lightning bolt
        }
        else if (timeLeft > 15)
        {
            timerText.color = orange; // Set the timer color to orange
            StopFlashingLightningBolt(); // Stop flashing the lightning bolt
        }
        else
        {
            timerText.color = red; // Set the timer color to red
            StartFlashingLightningBolt(); // Start flashing the lightning bolt
        }
    }

    void StartFlashingLightningBolt()
    {
        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashLightningBolt()); // Start flashing the lightning bolt
        }
    }

    void StopFlashingLightningBolt()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine); // Stop flashing the lightning bolt
            flashCoroutine = null; // Reset the flash coroutine
        }
        lightningBolt.enabled = false; // Hide the lightning bolt
    }

    IEnumerator FlashLightningBolt()
    {
        while (timeLeft <= 15 && timeLeft > 0)
        {
            lightningBolt.enabled = true; // Show lightning bolt
            yield return new WaitForSeconds(1f); // Wait for 1 second
            lightningBolt.enabled = false; // Hide lightning bolt
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }
    }

    public void CollectRepairKit()
    {
        timeLeft = 46f; // Add 1 second to the timer
        playerHealth.Repair(playerHealth.maxHealth); // Repair the player's health
        repairKitCount++; // Increment the repair kit counter
        UpdateBalanceUI(); // Update the balance display
        UpdateScoreUI(); // Update the score display
        SpawnRepairKit();
    }

    void SpawnRepairKit()
    {
        int spawnIndex; // Spawn index for repair kits
        do 
        {
            spawnIndex = Random.Range(0, repairKitSpawnPoints.Length); // Randomly select a spawn index
        } while (spawnIndex == lastSpawnIndex);
        Instantiate(repairKitPrefab, repairKitSpawnPoints[spawnIndex].position, Quaternion.identity); // Spawn a repair kit
        lastSpawnIndex = spawnIndex; // Update last spawn index
    }

    void UpdateBalanceUI()
    {
        int balance = repairKitCount * 100; // Calculate the balance
        balanceAmountText.text = "$" + balance.ToString(); // Update the balance display
        if (balance == 0)
        {
            balanceAmountText.color = red; // Set the balance color to red
        }
        else
        {
            balanceAmountText.color = darkGreen; // Set the balance color to dark green
        }
    }

    void UpdateScoreUI()
    {
        scoreAmountText.text = repairKitCount.ToString(); // Update the score display
        if (repairKitCount == 0)
        {
            scoreAmountText.color = red; // Set the score color to red
        }
        else
        {
            scoreAmountText.color = darkGreen; // Set the score color to dark green
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over!");
        // TODO: Implement game over logic
    }
}