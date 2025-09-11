using UnityEngine;
using System.Collections.Generic;

public class PlayerChoiceTracker : MonoBehaviour
{
    public static PlayerChoiceTracker Instance;
    
    [Header("Player Choices")]
    public List<PlayerChoice> playerChoices = new List<PlayerChoice>();
    
    [System.Serializable]
    public class PlayerChoice
    {
        public string choiceId;
        public string choiceText;
        public string targetScene;
        public float timestamp;
        
        public PlayerChoice(string id, string text, string scene)
        {
            choiceId = id;
            choiceText = text;
            targetScene = scene;
            timestamp = Time.time;
        }
    }
    
    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadChoices();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void RecordChoice(string choiceId, string choiceText, string targetScene)
    {
        PlayerChoice newChoice = new PlayerChoice(choiceId, choiceText, targetScene);
        playerChoices.Add(newChoice);
        
        Debug.Log($"Choice recorded: {choiceText} -> {targetScene}");
        
        // Save to PlayerPrefs immediately
        SaveChoices();
    }
    
    public void SaveChoices()
    {
        // Save number of choices
        PlayerPrefs.SetInt("PlayerChoiceCount", playerChoices.Count);
        
        // Save each choice
        for (int i = 0; i < playerChoices.Count; i++)
        {
            PlayerChoice choice = playerChoices[i];
            PlayerPrefs.SetString($"Choice_{i}_ID", choice.choiceId);
            PlayerPrefs.SetString($"Choice_{i}_Text", choice.choiceText);
            PlayerPrefs.SetString($"Choice_{i}_Scene", choice.targetScene);
            PlayerPrefs.SetFloat($"Choice_{i}_Time", choice.timestamp);
        }
        
        PlayerPrefs.Save();
        Debug.Log($"Saved {playerChoices.Count} player choices");
    }
    
    public void LoadChoices()
    {
        playerChoices.Clear();
        int choiceCount = PlayerPrefs.GetInt("PlayerChoiceCount", 0);
        
        for (int i = 0; i < choiceCount; i++)
        {
            string id = PlayerPrefs.GetString($"Choice_{i}_ID", "");
            string text = PlayerPrefs.GetString($"Choice_{i}_Text", "");
            string scene = PlayerPrefs.GetString($"Choice_{i}_Scene", "");
            float time = PlayerPrefs.GetFloat($"Choice_{i}_Time", 0f);
            
            if (!string.IsNullOrEmpty(id))
            {
                PlayerChoice choice = new PlayerChoice(id, text, scene);
                choice.timestamp = time;
                playerChoices.Add(choice);
            }
        }
        
        Debug.Log($"Loaded {playerChoices.Count} player choices");
    }
    
    public bool HasMadeChoice(string choiceId)
    {
        foreach (PlayerChoice choice in playerChoices)
        {
            if (choice.choiceId == choiceId)
            {
                return true;
            }
        }
        return false;
    }
    
    public PlayerChoice GetChoice(string choiceId)
    {
        foreach (PlayerChoice choice in playerChoices)
        {
            if (choice.choiceId == choiceId)
            {
                return choice;
            }
        }
        return null;
    }
    
    public List<PlayerChoice> GetAllChoices()
    {
        return new List<PlayerChoice>(playerChoices);
    }
    
    public void ClearAllChoices()
    {
        playerChoices.Clear();
        
        // Clear from PlayerPrefs
        int choiceCount = PlayerPrefs.GetInt("PlayerChoiceCount", 0);
        for (int i = 0; i < choiceCount; i++)
        {
            PlayerPrefs.DeleteKey($"Choice_{i}_ID");
            PlayerPrefs.DeleteKey($"Choice_{i}_Text");
            PlayerPrefs.DeleteKey($"Choice_{i}_Scene");
            PlayerPrefs.DeleteKey($"Choice_{i}_Time");
        }
        PlayerPrefs.DeleteKey("PlayerChoiceCount");
        PlayerPrefs.Save();
        
        Debug.Log("Cleared all player choices");
    }
    
    // Method to print all choices for debugging
    [ContextMenu("Print All Choices")]
    public void PrintAllChoices()
    {
        Debug.Log($"Player has made {playerChoices.Count} choices:");
        for (int i = 0; i < playerChoices.Count; i++)
        {
            PlayerChoice choice = playerChoices[i];
            Debug.Log($"{i + 1}. {choice.choiceId}: '{choice.choiceText}' -> {choice.targetScene} (Time: {choice.timestamp})");
        }
    }
}