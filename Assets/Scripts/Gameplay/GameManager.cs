using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public float timeLeft = 45f; // 45 seconds
    public PlayerHealth playerHealth;
    public Text timerText;
    public Image lightningBolt; // Reference to the lightning bolt Image

    private Coroutine flashCoroutine;

    void Start()
    {
        if (playerHealth == null)
        {
            playerHealth = FindObjectOfType<PlayerHealth>();
        }

        // Ensure the lightning bolt is initially hidden
        if (lightningBolt != null)
        {
            lightningBolt.enabled = false;
        }
    }

    void Update()
    {
        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            UpdateTimerUI();
            float healthDecreaseRate = playerHealth.maxHealth / 45f * Time.deltaTime;
            playerHealth.TakeDamage(healthDecreaseRate);
        }
        else
        {
            GameOver();
        }
    }

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(timeLeft / 60);
        int seconds = Mathf.FloorToInt(timeLeft % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

        if (timeLeft > 30)
        {
            timerText.color = Color.green;
            StopFlashingLightningBolt();
        }
        else if (timeLeft > 15)
        {
            timerText.color = Color.yellow;
            StopFlashingLightningBolt();
        }
        else
        {
            timerText.color = Color.red;
            StartFlashingLightningBolt();
        }
    }

    void StartFlashingLightningBolt()
    {
        if (flashCoroutine == null)
        {
            flashCoroutine = StartCoroutine(FlashLightningBolt());
        }
    }

    void StopFlashingLightningBolt()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }
        lightningBolt.enabled = false;
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

    public void GameOver()
    {
        Debug.Log("Game Over!");
    }
}