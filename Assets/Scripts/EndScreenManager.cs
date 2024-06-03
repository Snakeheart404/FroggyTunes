using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
// Manages the end screen UI, displaying current and best scores, and handling menu navigation.
public class EndScreenManager : MonoBehaviour
{
    // Text elements to display the current score and the best score.
    public TextMeshProUGUI currentScoreText;
    public TextMeshProUGUI bestScoreText;
    
    void Start()
    {
        string personalBestVarName = PlayerPrefs.GetString("SelectedLevelFilePath") + "SCORE";

        int currentScore = int.Parse(PlayerPrefs.GetString("temp"));
        int oldBestScore = PlayerPrefs.GetString(personalBestVarName) == "" ? 0 : int.Parse(PlayerPrefs.GetString(personalBestVarName));

        if (currentScore > oldBestScore)
        {
            PlayerPrefs.SetString(personalBestVarName, currentScore.ToString());
            currentScoreText.text = "Новий рекорд: " + PlayerPrefs.GetString("temp") + ". Ура!";

            bestScoreText.text = "Старий рекорд: " + oldBestScore.ToString();
        }
        else
        {
            currentScoreText.text = "Рахунок: " + currentScore.ToString();
            bestScoreText.text = "Рекорд: " + oldBestScore.ToString();
        }
    }

    public void ToMenu()
    {
        SceneManager.LoadScene("LevelMenu");
    }
}
