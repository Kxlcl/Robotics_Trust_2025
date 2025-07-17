using System;
using System.Collections.Generic;

[Serializable]
public class SurveyData
{
    public List<Question> questions;
}

[Serializable]
public class Question
{
    public int id;
    public string text;
    public string section;
    public string type;
    public List<string> options;
}

[Serializable]
public class PlayerAnswer
{
    public int questionId;
    public string response;
}

[Serializable]
public class PlayerResponse
{
    public string playerId;
    public List<PlayerAnswer> answers;
}
