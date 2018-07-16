using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnswerData {
    public string AnswerText { get; set; }
    public bool IsTrue { get; set; }

    public AnswerData(string text, bool correct)
    {
        AnswerText = text;
        IsTrue = correct;
    }
}
