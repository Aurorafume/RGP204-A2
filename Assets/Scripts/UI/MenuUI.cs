using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("Instructions");
    }

    public void OpenBackstory()
    {
        SceneManager.LoadScene("Backstory");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}