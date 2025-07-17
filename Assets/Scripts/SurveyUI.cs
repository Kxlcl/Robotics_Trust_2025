using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class SurveyUI : MonoBehaviour
{
    [Header("Setup")]
    public TextAsset surveyJson; // Drag survey_questions.json here
    public GameObject questionPrefab; // Drag your prefab with Text + InputField
    public Transform contentParent;   // ScrollView/Content GameObject

    private SurveyData surveyData;
    private List<TMP_InputField> inputFields = new List<TMP_InputField>();

    void Start()
    {
        if (surveyJson == null)
        {
            Debug.LogError("Survey JSON is not assigned in the Inspector.");
            return;
        }

        // Parse JSON into surveyData
        surveyData = JsonUtility.FromJson<SurveyData>(surveyJson.text);
        GenerateQuestions();
    }

    void GenerateQuestions()
    {
        foreach (var question in surveyData.questions)
        {
            GameObject instance = Instantiate(questionPrefab, contentParent);

            TextMeshProUGUI questionText = instance.transform.Find("Question_Text").GetComponent<TextMeshProUGUI>();
            TMP_InputField inputField = instance.transform.Find("Question_Answer/Input").GetComponent<TMP_InputField>();

            questionText.text = question.text;
            inputFields.Add(inputField);
        }
    }

    public void SubmitResponses()
    {
        List<PlayerAnswer> answers = new List<PlayerAnswer>();

        for (int i = 0; i < surveyData.questions.Count; i++)
        {
            answers.Add(new PlayerAnswer
            {
                questionId = surveyData.questions[i].id,
                response = inputFields[i].text
            });
        }

        PlayerResponse fullResponse = new PlayerResponse
        {
            playerId = "Player_" + System.DateTime.Now.Ticks,
            answers = answers
        };

        string json = JsonUtility.ToJson(fullResponse, true);
        string path = Path.Combine(Application.persistentDataPath, "player_response.json");
        File.WriteAllText(path, json);

        Debug.Log("Survey saved to: " + path);
    }
}
