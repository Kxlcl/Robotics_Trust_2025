using System.Collections.Generic;

[System.Serializable]
public class SurveyData
{
    public List<Question> questions;
}

[System.Serializable]
public class Question
{
    public int id;
    public string text;
    public string section;
    public string type;
    public List<string> options;
}
