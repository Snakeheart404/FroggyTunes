using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class CreateLevelMenuManager : MonoBehaviour
{
    public TMP_InputField nameInput;
    public TMP_InputField artistInput;
    public GameObject confirmationPopup;

    private string name;
    private string artist;
    private string levelFilePath;
    // Start is called before the first frame update
    void Start()
    {
        contentInteracting(true);
        levelFilePath = PlayerPrefs.GetString("SelectedLevelFilePath");
        confirmationPopup.SetActive(false); 
        LoadLevelInfo();
    }

    void LoadLevelInfo()
    {
        if (File.Exists(levelFilePath))
        {
            string[] lines = File.ReadAllLines(levelFilePath);
            foreach (string line in lines)
            {
                if (line.StartsWith("Name:"))
                {
                    name = line.Substring(6).Trim();
                    nameInput.text = name;
                }
                else if (line.StartsWith("Artist:"))
                {
                    artist = line.Substring(8).Trim();
                    artistInput.text = artist;
                }
            }
        }
    }
    
    public void StartGame()
    {
        SaveLevelData();
        SceneManager.LoadScene("Game");
    }

    private void SaveLevelData()
    {
        if (File.Exists(levelFilePath))
        {
            string[] lines = File.ReadAllLines(levelFilePath);
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("Name:"))
                {
                    lines[i] = "Name: " + nameInput.text;
                }
                else if (lines[i].StartsWith("Artist:"))
                {
                    lines[i] = "Artist: " + artistInput.text;
                }
            }

            File.WriteAllLines(levelFilePath, lines);
        }
    }

    private void contentInteracting(bool flag)
    {
        foreach (var button in GetComponentsInChildren<Button>(true))
        {
            button.interactable = flag;
        }
        foreach (var inputField in GetComponentsInChildren<TMP_InputField>(true))
        {
            inputField.interactable = flag;
        }
    }
    
    public void BackToMenu()
    {
        if ((name != nameInput.text) || (artist != artistInput.text))
        {
            confirmationPopup.SetActive(true);
            contentInteracting(false);
        }
        else
        {
            SceneManager.LoadScene("LevelMenu");
        }
        
    }
    
    public void Cancel()
    {
        confirmationPopup.SetActive(false);
        contentInteracting(true);
        
    }
    public void DontSaveLevel()
    {
        SceneManager.LoadScene("LevelMenu");
        
    }
    
    public void SaveLevel()
    {
        SaveLevelData();
        SceneManager.LoadScene("LevelMenu");
    }
}
