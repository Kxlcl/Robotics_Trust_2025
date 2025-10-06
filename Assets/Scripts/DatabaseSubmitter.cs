using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSessionData
{
    public string sessionId;
    public string playerId;
    public string timestamp;
    public List<PlayerChoiceTracker.PlayerChoice> choices;
    public List<Answer> surveyAnswers;
    public float finalTimeRemaining;
    public string completionStatus; // "completed", "timeout", "game_over"
}

public class DatabaseSubmitter : MonoBehaviour
{
    [Header("API Settings")]
    [Tooltip("URL of your backend API endpoint")]
    public string apiUrl = "http://localhost:3000/api/submit-session";

    [Header("Debug")]
    public bool logResponses = true;

    private static DatabaseSubmitter _instance;
    public static DatabaseSubmitter Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("DatabaseSubmitter");
                _instance = go.AddComponent<DatabaseSubmitter>();
                DontDestroyOnLoad(go);
            }
            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Submit game session data including choices and survey responses to MongoDB
    /// </summary>
    public void SubmitSessionData(string completionStatus = "completed")
    {
        StartCoroutine(SubmitSessionDataCoroutine(completionStatus));
    }

    private IEnumerator SubmitSessionDataCoroutine(string completionStatus)
    {
        // Gather all data
        GameSessionData sessionData = new GameSessionData
        {
            sessionId = System.Guid.NewGuid().ToString(),
            playerId = GetOrCreatePlayerId(),
            timestamp = System.DateTime.UtcNow.ToString("o"), // ISO 8601 format
            choices = new List<PlayerChoiceTracker.PlayerChoice>(),
            surveyAnswers = new List<Answer>(),
            finalTimeRemaining = GlobalTimer.Instance != null ? GlobalTimer.Instance.timeRemaining : 0f,
            completionStatus = completionStatus
        };

        // Get player choices
        if (PlayerChoiceTracker.Instance != null)
        {
            sessionData.choices = PlayerChoiceTracker.Instance.GetAllChoices();
            Debug.Log($"DatabaseSubmitter: Collected {sessionData.choices.Count} player choices");
        }
        else
        {
            Debug.LogWarning("DatabaseSubmitter: PlayerChoiceTracker.Instance is null");
        }

        // Get survey answers (if available)
        SurveySaver surveySaver = FindObjectOfType<SurveySaver>();
        if (surveySaver != null)
        {
            // Note: You may need to add a public getter to SurveySaver to access answers
            Debug.Log("DatabaseSubmitter: Survey data found");
        }

        // Convert to JSON
        string jsonData = JsonUtility.ToJson(sessionData, true);

        if (logResponses)
        {
            Debug.Log($"DatabaseSubmitter: Submitting data:\n{jsonData}");
        }

        // Send to backend
        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"DatabaseSubmitter: Successfully submitted data to MongoDB. Response: {request.downloadHandler.text}");

                if (logResponses)
                {
                    Debug.Log($"Response Code: {request.responseCode}");
                    Debug.Log($"Response Body: {request.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"DatabaseSubmitter: Failed to submit data. Error: {request.error}");
                Debug.LogError($"Response Code: {request.responseCode}");
                Debug.LogError($"Response: {request.downloadHandler.text}");

                // Fallback: Save locally if submission fails
                SaveLocalBackup(jsonData);
            }
        }
    }

    /// <summary>
    /// Submit survey answers separately (can be called from survey completion)
    /// </summary>
    public void SubmitSurveyOnly(List<Answer> surveyAnswers)
    {
        StartCoroutine(SubmitSurveyOnlyCoroutine(surveyAnswers));
    }

    private IEnumerator SubmitSurveyOnlyCoroutine(List<Answer> surveyAnswers)
    {
        GameSessionData sessionData = new GameSessionData
        {
            sessionId = System.Guid.NewGuid().ToString(),
            playerId = GetOrCreatePlayerId(),
            timestamp = System.DateTime.UtcNow.ToString("o"),
            surveyAnswers = surveyAnswers,
            completionStatus = "survey_only"
        };

        string jsonData = JsonUtility.ToJson(sessionData, true);

        using (UnityWebRequest request = new UnityWebRequest(apiUrl, "POST"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("DatabaseSubmitter: Successfully submitted survey data");
            }
            else
            {
                Debug.LogError($"DatabaseSubmitter: Failed to submit survey. Error: {request.error}");
                SaveLocalBackup(jsonData);
            }
        }
    }

    private string currentPlayerId = null;

    private string GetOrCreatePlayerId()
    {
        // Generate a new random 32-character ID each game session
        if (string.IsNullOrEmpty(currentPlayerId))
        {
            currentPlayerId = GenerateRandomPlayerId(32);
            Debug.Log($"DatabaseSubmitter: Generated new PlayerId: {currentPlayerId}");
        }

        return currentPlayerId;
    }

    /// <summary>
    /// Generates a random alphanumeric ID of specified length
    /// Uses uppercase letters (A-Z), lowercase letters (a-z), and numbers (0-9)
    /// </summary>
    private string GenerateRandomPlayerId(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        System.Text.StringBuilder result = new System.Text.StringBuilder(length);
        System.Random random = new System.Random();

        for (int i = 0; i < length; i++)
        {
            result.Append(chars[random.Next(chars.Length)]);
        }

        return result.ToString();
    }

    private void SaveLocalBackup(string jsonData)
    {
        try
        {
            string backupPath = System.IO.Path.Combine(
                Application.persistentDataPath,
                $"backup_session_{System.DateTime.Now:yyyyMMdd_HHmmss}.json"
            );

            System.IO.File.WriteAllText(backupPath, jsonData);
            Debug.Log($"DatabaseSubmitter: Saved backup to: {backupPath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"DatabaseSubmitter: Failed to save backup. Error: {e.Message}");
        }
    }

    /// <summary>
    /// Call this when the game ends or player completes the experience
    /// </summary>
    public void OnGameComplete()
    {
        SubmitSessionData("completed");
    }

    /// <summary>
    /// Call this when timer runs out
    /// </summary>
    public void OnTimeout()
    {
        SubmitSessionData("timeout");
    }

    /// <summary>
    /// Call this when player gets game over
    /// </summary>
    public void OnGameOver()
    {
        SubmitSessionData("game_over");
    }
}
