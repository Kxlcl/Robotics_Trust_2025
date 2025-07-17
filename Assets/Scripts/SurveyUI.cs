using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class SurveyUI : MonoBehaviour
{
    [Header("Setup")]
    public TextAsset surveyJson;
    public GameObject questionPrefab;
    public Transform contentParent;
    public GameObject nextButton;
    public GameObject prevButton;
    public GameObject submitButton;

    private SurveyData surveyData;
    private List<PlayerAnswer> answers = new List<PlayerAnswer>();
    private List<GameObject> instantiatedQuestions = new List<GameObject>();
    private int currentQuestionIndex = 0;

    void Start()
    {
        if (surveyJson == null)
        {
            Debug.LogError("Survey JSON is not assigned in the Inspector.");
            return;
        }

        surveyData = JsonUtility.FromJson<SurveyData>(surveyJson.text);

        // Pre-fill answers list with blank responses
        foreach (var question in surveyData.questions)
        {
            answers.Add(new PlayerAnswer
            {
                questionId = question.id,
                response = ""
            });
        }

        ShowQuestion(currentQuestionIndex);
    }

    void ShowQuestion(int index)
    {
        Debug.Log($"Showing question at index {index}");

        if (surveyData == null || surveyData.questions == null || surveyData.questions.Count == 0)
        {
            Debug.LogError("Survey data is missing or empty.");
            return;
        }

        // Clear previous
        foreach (var go in instantiatedQuestions)
            Destroy(go);
        instantiatedQuestions.Clear();

        var question = surveyData.questions[index];
        GameObject instance = Instantiate(questionPrefab, contentParent);
        instantiatedQuestions.Add(instance);

        Debug.Log("Instantiated question prefab.");

        // Update these lines if your hierarchy changed
        TextMeshProUGUI questionText = instance.transform.Find("Question_Text")?.GetComponent<TextMeshProUGUI>();
        TMP_InputField inputField = instance.transform.Find("Question_Answer/Input/InputField (TMP)")?.GetComponent<TMP_InputField>();

        if (questionText == null || inputField == null)
        {
            Debug.LogError("Failed to find required components on the prefab.");
            return;
        }

        questionText.text = question.text;
        inputField.text = answers[index].response;

        inputField.onValueChanged.AddListener(value =>
        {
            answers[index].response = value;
        });

        prevButton.SetActive(index > 0);
        nextButton.SetActive(index < surveyData.questions.Count - 1);
        submitButton.SetActive(index == surveyData.questions.Count - 1);
    }


    public void NextQuestion()
    {
        if (currentQuestionIndex < surveyData.questions.Count - 1)
        {
            currentQuestionIndex++;
            ShowQuestion(currentQuestionIndex);
        }
    }

    public void PreviousQuestion()
    {
        if (currentQuestionIndex > 0)
        {
            currentQuestionIndex--;
            ShowQuestion(currentQuestionIndex);
        }
    }

    public void SubmitResponses()
    {
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
