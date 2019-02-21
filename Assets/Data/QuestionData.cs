using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class QuestionData {
    public AnswerData[] Answers { get; set; }
}

public class MusicQuestion : QuestionData
{
    public string MusicPath { get; set; }
    public float StartTrack { get; set; }
    public float Volume { get; set; }
    public bool Fade { get; set; }

    public MusicQuestion(string name, AnswerData[] answers)
    {
        MusicPath = name;
        StartTrack = 0.0f;
        Volume = 1.0f;
        Answers = answers;
    }

}

public class ImageQuestion : QuestionData
{
    public string ImagePath { get; set; }

    public ImageQuestion (string path, AnswerData[] answers)
    {
        ImagePath = path;
        Answers = answers;
    }
}

public class TextQuestion : QuestionData
{
    public string Question { get; set; }

    public TextQuestion (string question, AnswerData[] answers)
    {
        Question = question;
        Answers = answers;
    }
}

public class TrueFalseQuestion : QuestionData
{
    public string Question { get; set; }

    public TrueFalseQuestion(string question, AnswerData[] answers)
    {
        Question = question;
        Answers = answers;
    }
}