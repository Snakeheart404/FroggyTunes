using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.IO.Compression;

using SFB;

[System.Serializable] public class Response
{
    public static Response CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<Response>(jsonString);
    }
}
// Manages level menu.
public class LevelMenuManager : MonoBehaviour
{
    // The Content GameObject in the Scroll View
    public GameObject content;

// The Button Prefab
    public GameObject buttonPrefab;

// The Scroll View for levels
    public GameObject levelScrollView;

// The Panel for displaying level details
    public GameObject levelDetailsPanel;

// Text component for the level name
    public TextMeshProUGUI levelNameText;

// Text component for the artist
    public TextMeshProUGUI artistText;

// Text component for the personal best
    public TextMeshProUGUI personalBestText;

// The "Start Game" button
    public Button startGameButton;

// The "Back" button
    public Button backButton;
    
// Path to the level which was selected to play
    private string selectedLevelFilePath;
    void Start()
    {
        PopulateMenu();
        levelScrollView.SetActive(true);
        levelDetailsPanel.SetActive(false); 
        backButton.onClick.AddListener(BackToMenu);
    }
    
    void PopulateMenu()
    {
        string[] filesStreamingAssets = Directory.GetFiles(Application.streamingAssetsPath, "*.txt");
        string[] filesPersistentData = Directory.GetFiles(Application.persistentDataPath, "*.txt");

        string[] files = new string[filesStreamingAssets.Length + filesPersistentData.Length];
        filesStreamingAssets.CopyTo(files, 0);
        filesPersistentData.CopyTo(files, filesStreamingAssets.Length);
        foreach (string file in files)
        {
            
            string levelName = ExtractLevelName(file);

            if (!string.IsNullOrEmpty(levelName))
            {
                GameObject panel = Instantiate(buttonPrefab, content.transform);
                Button button = panel.GetComponentInChildren<Button>();
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                buttonText.text = levelName;
                button.onClick.AddListener(() => LoadLevel(file));
            }
        }
    }

    string ExtractLevelName(string filePath)
    {
        string levelName = "";
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.StartsWith("Name:"))
                {
                    levelName = line.Substring(6).Trim();
                    break;
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to read file: " + filePath + " Error: " + ex.Message);
        }

        return levelName;
    }

    void LoadLevel(string filePath)
    {
        selectedLevelFilePath = filePath;

        string name = "", artist = "", personalBest = ""; 

        string personalBestVarName = filePath + "SCORE";
        if (PlayerPrefs.GetString(personalBestVarName) != "")
        {
            personalBest = PlayerPrefs.GetString(personalBestVarName);
        }
        else
        {
            personalBest = 0.ToString();
        }
        
        try
        {
            string[] lines = File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                if (line.StartsWith("Name:"))
                {
                    name = line.Substring(6).Trim();
                }
                else if (line.StartsWith("Artist:"))
                {
                    artist = line.Substring(8).Trim();
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Failed to read file: " + filePath + " Error: " + ex.Message);
        }

        // Update the level details panel
        levelNameText.text = "Назва: " + name;
        artistText.text = "Виконавець: " + artist;
        personalBestText.text = "Рекорд: " + personalBest;

        // Show the level details panel and hide the levels list
        levelScrollView.SetActive(false);
        levelDetailsPanel.SetActive(true);

        // Set up the start game button
        startGameButton.onClick.RemoveAllListeners();
        startGameButton.onClick.AddListener(StartGame);
    }

    void StartGame()
    {
        // Load the game scene and pass the level file name
        PlayerPrefs.SetString("SelectedLevelFilePath", selectedLevelFilePath);
        SceneManager.LoadScene("Game");
    }

    void BackToMenu()
    {
        levelScrollView.SetActive(true);

        levelDetailsPanel.SetActive(false);
    }

    IEnumerator SendDataToServer(string path)
    {
        // URL of Flask server
        string url = "https://levelgenerator.onrender.com/";

        
        // Create form and add file
        WWWForm form = new WWWForm();
        byte[] fileData = File.ReadAllBytes(path);
        form.AddBinaryData("file", fileData, Path.GetFileName(path), GetMimeType(path));

        // Send POST request
        UnityWebRequest request = UnityWebRequest.Post(url, form);
        
        yield return request.SendWebRequest();
        
        // Wait for response
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Data sent successfully");

            // Get the received ZIP file
            byte[] receivedData = request.downloadHandler.data;
            string zipFilePath = Path.Combine(Application.persistentDataPath, "received_files.zip");

            File.WriteAllBytes(zipFilePath, receivedData);

            // Extract the ZIP file
            string extractPath = Path.Combine(Application.persistentDataPath, "extracted_files");
            if (Directory.Exists(extractPath))
            {
                Directory.Delete(extractPath, true);
            }
            Directory.CreateDirectory(extractPath);
            ZipFile.ExtractToDirectory(zipFilePath, extractPath);

            string txtFilePath = null;
            string[] extractedFiles = Directory.GetFiles(extractPath);
            foreach (string extractedFile in extractedFiles)
            {
                if (Path.GetExtension(extractedFile).ToLower() == ".txt")
                {
                    txtFilePath = extractedFile;
                    break;
                }
                
                
            }

            if (txtFilePath != null)
            {
                PlayerPrefs.SetString("SelectedLevelFilePath", txtFilePath);
                SceneManager.LoadScene("CreateLevelMenu");
            }
        }
        
    }
    
    private string GetMimeType(string filePath)
    {
        string extension = Path.GetExtension(filePath).ToLower();
        switch (extension)
        {
            case ".mp3": return "audio/mpeg";
            case ".ogg": return "audio/ogg";
            case ".wav": return "audio/wav";
            case ".m4a": return "audio/mp4";
            default: return "application/octet-stream";
        }
    }
    
    private string GetUniqueFilePath(string filePath)
    {
        string directory = Path.GetDirectoryName(filePath);
        
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string extension = Path.GetExtension(filePath);
        int count = 1;

        while (File.Exists(filePath))
        {
            string newFileName = $"{fileNameWithoutExtension} ({count}){extension}";
            filePath = Path.Combine(directory, newFileName);
            count++;
        }

        return filePath;
    }
    public void GenerateNewLevel()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Sound Files", "mp3", "wav", "ogg", "m4a")
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Оберіть пісню тривалістю до 10 хвилин", "", extensions, false);
        
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            string sourcePath = paths[0];
            string fileName = Path.GetFileName(sourcePath);
            string destinationPath = Path.Combine(Application.persistentDataPath, fileName);

            destinationPath = GetUniqueFilePath(destinationPath);

            File.Copy(sourcePath, destinationPath);

            StartCoroutine(SendDataToServer(destinationPath));
        }
    }
    
    public void BackToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
