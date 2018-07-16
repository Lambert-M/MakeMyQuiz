using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopicData
{
    public bool IsAvailable { get; set; }
    public string Name { get; set; }
    public List<QuestionData> Questions { get; set; }

    public TopicData(string name, List<QuestionData> q)
    {
        IsAvailable = true;
        Name = name;
        Questions = q;
    }
}
