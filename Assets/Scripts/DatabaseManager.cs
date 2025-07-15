using UnityEngine;
using System;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    [System.Serializable]
    public class AnswerEntry
    {
        public int questionId;
        public string response;
    }

    [System.Serializable]
    public class SurveyResponse
    {
        public string playerId;
        public string timestamp;
        public AnswerEntry[] answers;
    }

    public void SaveSurveyResponse(string playerId, AnswerEntry[] answers)
    {
        SurveyResponse response = new SurveyResponse
        {
            playerId = playerId,
            timestamp = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ"),
            answers = answers
        };

        string json = JsonUtility.ToJson(response, true);
        string filename = $"survey_{playerId}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
        string path = Path.Combine(Application.persistentDataPath, filename);

        File.WriteAllText(path, json);
        Debug.Log("✅ Survey saved to: " + path);
    }
}
