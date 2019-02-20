using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using SimpleJSON;

public class DataModel : MonoBehaviour
{
    public static Dictionary<string, string> TextToUse;
    public static string QuestionMusicName { get; set; }
    public static string TopicMusicName { get; set; }
    public static string OutroMusicName { get; set; }
    public static string IntroMusicName { get; set; }
    public static string BackgroundName { get; set; }
    public static string CurTopicName { get; set; }
    public static string QuizName { get; set; }
    public static int? JokerConfig { get; set; }
    public static int RoundNumber { get; set; }
    public static KeyCode Next { get; set; }
    public static KeyCode Pause { get; set; }
    public static KeyCode Speed { get; set; }
    public static List<RoundData> Rounds { get; set; }
    public static string CurrentFilename { get; set; }
    public static string CurrentRunningFilename { get; set; }

    //Scores attributes
    public static int NumberOfTeams { get; set; }
    public static int[] Scores { get; set; }
    public static bool[] Jokers { get; set; }

    // indexes
    public static int IroundCur { get; set; }
    public static int ItopicCur { get; set; }
    public static int IquestionCur { get; set; }

   
    void Start()
    {
        Next = KeyCode.RightArrow;
        Pause = KeyCode.Space;
        Speed = KeyCode.UpArrow;

        QuizName = "Quiz du fun";
        CurTopicName = "";
        BackgroundName = "wallpaper1";
        IntroMusicName = "intro1";
        OutroMusicName = "outro1";
        QuestionMusicName = "question1";
        TopicMusicName = "topic1";

        RoundNumber = 1;
        IroundCur = 0;
        ItopicCur = 0;
        IquestionCur = 0;
        
        Rounds = new List<RoundData>();
        
        NumberOfTeams = 8;
        DontDestroyOnLoad(gameObject);

        Scores = new int[NumberOfTeams];
        Jokers = new bool[NumberOfTeams];

        string[] pathsrc = Application.dataPath.Split('/');
        string pathfinal = pathsrc[0] + "/";
        for(int i = 1; i< pathsrc.Length-1; i++)
        {
            pathfinal += '/' + pathsrc[i];
        }
        Directory.CreateDirectory(pathfinal + '/' + "Sounds");
        Directory.CreateDirectory(pathfinal + '/' + "Images");
        Directory.CreateDirectory(pathfinal + '/' + "Saves");

        DontDestroyOnLoad(gameObject);
        SceneManager.LoadScene("Language");
    }

    private void Update()
    {
        if (Input.GetKeyDown(Speed))
        {
            if (Time.timeScale > 1f)
            {
                Time.timeScale = 1f;
            }
            else
            {
                Time.timeScale = 10f;
            }
        }
    }

    public static RoundData CurRound()
    {
        return Rounds.First();
    }

    public static TopicData CurTopic()
    {
        return CurRound().Topics[Rounds.First().SearchTopic(CurTopicName)];
    }

    public static QuestionData CurQuestion()
    {
        return CurTopic().Questions.First();
    }
    
    /**
 * @author David
 * 
 * Saving the current state of DataModel in the data.json file
 **/
    public static void Save(string filepath)
    {
        JSONObject dataJson = new JSONObject();
        dataJson.Add("Quizname", QuizName);
        dataJson.Add("CurTopicName", CurTopicName);
        dataJson.Add("RoundNumber", RoundNumber);
        // indexes
        dataJson.Add("IroundCur", IroundCur);
        dataJson.Add("ItopicCur", ItopicCur);
        dataJson.Add("IquestionCur", IquestionCur);

        for (int i = 0; i < Scores.Length; i++)
        {
            dataJson.Add("Score" + i, Scores[i]);
            dataJson.Add("Joker" + i, Jokers[i]);
        }

        JSONArray roundsJsonArray = new JSONArray();
        foreach (RoundData rd in Rounds)
        {
            JSONObject roundJson = new JSONObject();
            roundJson.Add("Type", rd.Type);

            JSONArray topicsJsonArray = new JSONArray();
            foreach (TopicData td in rd.Topics)
            {
                JSONObject topicJson = new JSONObject();

                topicJson.Add("Name", td.Name);
                topicJson.Add("IsAvailable", td.IsAvailable);

                JSONArray questionsJsonArray = new JSONArray();
                foreach (QuestionData qd in td.Questions)
                {
                    JSONObject questionJson = new JSONObject();
                    
                    switch (qd.GetType().ToString())
                    {
                        case "MusicQuestion":
                            MusicQuestion mq = (MusicQuestion)qd;
                            questionJson.Add("MusicPath", mq.MusicPath);
                            questionJson.Add("StartTrack", mq.StartTrack);
                            questionJson.Add("Volume", mq.Volume);
                            questionJson.Add("Fade", mq.Fade);
                            break;
                        case "ImageQuestion":
                            ImageQuestion iq = (ImageQuestion)qd;
                            questionJson.Add("ImagePath", iq.ImagePath);
                            break;
                        case "TextQuestion":
                            TextQuestion tq = (TextQuestion)qd;
                            questionJson.Add("Question", tq.Question);
                            break;
                        default: Debug.LogError("Type de question non-reconnu"); break;
                    }

                    JSONArray answersJsonArray = new JSONArray();
                    foreach (AnswerData ad in qd.Answers)
                    {
                        JSONObject answerJson = new JSONObject();

                        answerJson.Add("AnswerText", ad.AnswerText);
                        answerJson.Add("IsTrue", ad.IsTrue);

                        answersJsonArray.Add(answerJson);
                    }
                    questionJson.Add("Answers", answersJsonArray);

                    questionsJsonArray.Add(questionJson);
                }
                topicJson.Add("Questions", questionsJsonArray);

                topicsJsonArray.Add(topicJson);
            }

            roundJson.Add("Topics", topicsJsonArray);

            roundsJsonArray.Add(roundJson);
        }

        dataJson.Add("Rounds", roundsJsonArray);

        File.WriteAllText(filepath, dataJson.ToString());

    }

    public void Load(string filepath)
    {
        string path = filepath;
        string jsonString = File.ReadAllText(path);
        JSONObject dataJson = (JSONObject)JSON.Parse(jsonString);

        // Set values
        QuizName = dataJson["Quizname"].Value;
        CurTopicName = dataJson["CurTopicName"].Value;
        RoundNumber = dataJson["RoundNumber"].AsInt;
        //indexes
        IroundCur = dataJson["IroundCur"].AsInt;
        ItopicCur = dataJson["ItopicCur"].AsInt;
        IquestionCur = dataJson["IquestionCur"].AsInt;
        
        Scores = new int[NumberOfTeams];
        Jokers = new bool[NumberOfTeams];

        for (int i = 0; i < Scores.Length; i++)
        {
            Scores[i] = dataJson[("Score" + i)].AsInt;
            Jokers[i] = dataJson[("Joker" + i)].AsBool;
        }

        // Rounds
        List<RoundData> rdList = new List<RoundData>();
        for(int i = 0; i < dataJson["Rounds"].AsArray.Count; i++)
        {
            List<TopicData> tdList = new List<TopicData>();
            for(int j = 0; j < dataJson["Rounds"].AsArray[i]["Topics"].AsArray.Count; j ++)
            {
                List<QuestionData> qdList = new List<QuestionData>();
                for(int k = 0; k < dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray.Count; k++)
                {
                    AnswerData[] adTab = new AnswerData[4];
                    for (int l = 0; l < 4; l++)
                    {
                        AnswerData ad = new AnswerData(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Answers"].AsArray[l]["AnswerText"].Value,
                            dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Answers"].AsArray[l]["IsTrue"].AsBool);

                        adTab[l] = ad;
                    }

                    switch (dataJson["Rounds"].AsArray[i]["Type"].Value)
                    {
                        case "Blind test":
                            MusicQuestion mq = new MusicQuestion(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["MusicPath"].Value,
                                adTab);
                            mq.StartTrack = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["StartTrack"].AsFloat;
                            mq.Volume = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Volume"].AsFloat;
                            mq.Fade = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Fade"].AsBool;
                            qdList.Add((QuestionData)mq);
                            break;
                        case "Musique":
                            MusicQuestion mq2 = new MusicQuestion(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["MusicPath"].Value,
                                adTab);
                            mq2.StartTrack = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["StartTrack"].AsFloat;
                            mq2.Volume = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Volume"].AsFloat;
                            mq2.Fade = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Fade"].AsBool;
                            qdList.Add((QuestionData)mq2);
                            break;
                        case "Image":
                            ImageQuestion iq = new ImageQuestion(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["ImagePath"].Value,
                                adTab);
                            qdList.Add((QuestionData)iq);
                            break;
                        case "QCM":
                            TextQuestion tq = new TextQuestion(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Question"].Value,
                                adTab);
                            qdList.Add((QuestionData)tq);
                            break;
                        case "MCQ":
                            TextQuestion tq2 = new TextQuestion(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Questions"].AsArray[k]["Question"].Value,
                                adTab);
                            qdList.Add((QuestionData)tq2);
                            break;
                        default:
                            Debug.LogError("Type de question non-reconnu");
                            break;
                    }                    
                }
                TopicData td = new TopicData(dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["Name"].Value,
                    qdList);
                td.IsAvailable = dataJson["Rounds"].AsArray[i]["Topics"].AsArray[j]["IsAvailable"].AsBool;

                tdList.Add(td);
            }

            RoundData rd = new RoundData(dataJson["Rounds"].AsArray[i]["Type"].Value, tdList);

            rdList.Add(rd);
        }
        Rounds = rdList;
    }

    public static void DestroyAllData()
    {
        Rounds.Clear();
        QuizName = "Quiz du fun";
        CurTopicName = "";
        BackgroundName = "wallpaper1";
        IntroMusicName = "intro1";
        OutroMusicName = "outro1";
        QuestionMusicName = "question1";
        TopicMusicName = "topic1";

        RoundNumber = 1;
        IroundCur = 0;
        ItopicCur = 0;
        IquestionCur = 0;

        Rounds = new List<RoundData>();
        

    }

    public static void AddScoreToTeam(int score, int team)
    {
        if (Jokers[team])
        {
            Scores[team] += score + 2;
        }
        else
        {
            Scores[team] += score;
        }
    }

    public static string GetTextScoreFromTeam(int team)
    {
        string score = "";
        if ( Scores[team] >= 0)
        {
            if ((Scores[team] / 100) < 1)
            {
                score += "0";
            }
            if ((Scores[team] / 10) < 1)
            {
                score += "0";

            }
        }
        return score + Scores[team].ToString();
    }

    public static int BestScore()
    {
        int best = -1;
        for (int i = 0; i < Scores.Length; i++)
        {
            if (Scores[i] > best)
            {
                best = Scores[i];
            }
        }
        return best;
    }
}