using UnityEngine;
using UnityEngine.UI;

public class DecisionUI : MonoBehaviour
{
    public Text questionText;
    public InputField nameField;
    public InputField ageField;
    public Button startButton;
    public Button optionAButton;
    public Button optionBButton;
    public DatabaseManager database;

    private string currentQuestion = "Do you trust the robot?";

    void Start()
    {
        questionText.text = "Enter your info to start.";

        startButton.onClick.AddListener(StartSurvey);
        optionAButton.onClick.AddListener(() => OnAnswer("Yes"));
        optionBButton.onClick.AddListener(() => OnAnswer("No"));

        optionAButton.gameObject.SetActive(false);
        optionBButton.gameObject.SetActive(false);
    }

    void StartSurvey()
    {
        database.SavePlayerInfo(nameField.text, int.Parse(ageField.text));
        questionText.text = currentQuestion;

        startButton.gameObject.SetActive(false);
        optionAButton.gameObject.SetActive(true);
        optionBButton.gameObject.SetActive(true);
    }

    void OnAnswer(string answer)
    {
        database.SaveDecision(currentQuestion, answer);
        questionText.text = "Thank you!";
        optionAButton.interactable = false;
        optionBButton.interactable = false;
    }
}
