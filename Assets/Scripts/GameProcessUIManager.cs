using UnityEngine;
using UnityEngine.SceneManagement;
// Manages the in-game UI, handling pause, resume, and main menu navigation.
public class GameProcessUIManager : MonoBehaviour
{
    // Reference to the pause screen UI element.
    public GameObject pauseScreen;
    
    public void StopGame()
    {
        AudioListener.pause = true;
        Time.timeScale = 0;
        pauseScreen.SetActive(true);
    }
    
    public void ResumeGame()
    {
        AudioListener.pause = false;
        Time.timeScale = 1;
        pauseScreen.SetActive(false);
    }
    
    public void ToMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
