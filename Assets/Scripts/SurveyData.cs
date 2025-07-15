using UnityEngine;

[CreateAssetMenu(fileName = "SurveyData", menuName = "Survey/Question Set")]
public class SurveyData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] questions;
}
