using System;
using System.Collections.Generic;

[Serializable]
public class ResponseEntry
{
    public int questionId;
    public string response;
}

[Serializable]
public class SurveyResponse
{
    public string playerId;
    public List<ResponseEntry> responses = new List<ResponseEntry>();
}
