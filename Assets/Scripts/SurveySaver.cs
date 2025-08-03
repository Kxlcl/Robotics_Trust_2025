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
            playerId = System.Guid.NewGuid().ToString(),
            timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")
        };
        filePath = Path.Combine(Application.persistentDataPath, $"survey_response_{currentResponse.timestamp}.json");
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
    }
}
