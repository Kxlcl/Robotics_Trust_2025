using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.IO;

public class SurveyUI : MonoBehaviour
{
    public TextAsset surveyJson; // Assign your survey.json file in Inspector
    public GameObject questionPrefab; // Assign a prefab with TMP text and input field
    public Transform contentParent; // ScrollView/Content container

    private List<TMP_InputField> inputFields = new List<TMP_InputField>();

    void Start()
    {
        SurveyData surveyData = JsonUtility.FromJson<SurveyData>(surveyJson.text);
        foreach (var question in surveyData.questions)
        {
            GameObject instance = Instantiate(questionPrefab, contentParent);
            instance.transform.Find("QuestionText").GetComponent<TextMeshProUGUI>().text = question.text;
            TMP_InputField inputField = instance.transform.Find("AnswerField").GetComponent<TMP_InputField>();
            inputFields.Add(inputField);
        }
    }

    public void SubmitResponses()
    {
        List<string> responses = new List<string>();
        foreach (var input in inputFields)
        {
            responses.Add(input.text);
        }

        string json = JsonUtility.ToJson(new SurveyResponseWrapper { responses = responses }, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, "responses.json"), json);
        Debug.Log("Responses saved to: " + Application.persistentDataPath);
    }
}

[System.Serializable]
public class SurveyResponseWrapper
{
    public List<string> responses;
}
