using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CompletedGameUI : MonoBehaviour
{
    public void RetryGame()
    {
        SceneManager.LoadScene("Main");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}