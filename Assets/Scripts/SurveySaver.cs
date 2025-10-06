using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class SurveyResponse
{
    public string playerId;
    public string timestamp;
    public List<Answer> answers = new List<Answer>();
}

[Serializable]
public class Answer
{
    public int questionId;
    public string response; // For MultipleSelect, store as comma-separated string
}

public class SurveySaver : MonoBehaviour
{
    private SurveyResponse currentResponse;
    private string filePath;

    private void Awake()
    {
        currentResponse = new SurveyResponse
        {
            playerId = GenerateRandomPlayerId(32),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        };
        filePath = Path.Combine(Application.persistentDataPath, $"survey_response_{currentResponse.timestamp}.json");
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

    public void SaveAnswer(int questionId, string response)
    {
        var existing = currentResponse.answers.Find(a => a.questionId == questionId);
        if (existing != null)
        {
            existing.response = response;
        }
        else
        {
            currentResponse.answers.Add(new Answer { questionId = questionId, response = response });
        }
        SaveToFile();
    }

    private void SaveToFile()
    {
        string json = JsonUtility.ToJson(currentResponse, true);
        File.WriteAllText(filePath, json);
    }

    // Optionally, call this at the end to finalize or upload
    public void FinalizeSurvey()
    {
        SaveToFile();
        Debug.Log($"Survey saved to: {filePath}");

        // Submit to database if DatabaseSubmitter is available
        if (DatabaseSubmitter.Instance != null)
        {
            DatabaseSubmitter.Instance.SubmitSurveyOnly(currentResponse.answers);
            Debug.Log("Survey data submitted to database");
        }
    }

    // Public getter for answers (used by DatabaseSubmitter)
    public List<Answer> GetAnswers()
    {
        return new List<Answer>(currentResponse.answers);
    }

    // Public getter for survey response (used by DatabaseSubmitter)
    public SurveyResponse GetCurrentResponse()
    {
        return currentResponse;
    }
}
