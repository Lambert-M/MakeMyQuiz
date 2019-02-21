using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TrueFalseController : MonoBehaviour
{
    //Variables
    private bool isAnyThemeLeftInCurRound;
    private bool pauseActivated;
    private bool isNextAvailable;
    private bool resetvol;
    private bool goingToNextQuestion;
    private bool first;
    private int visibleCharacterCount;
    private int numberOfCharacters;
    private int numberOfQuestions;
    private int actualQuestion;
    private string localpath;
    private float question_length_to_time; 
    //Objects
    private AnswerData currAnswer;
    private TextMeshProUGUI questionText;
    private AudioSource musicSource;
    private AudioClip music;
    private GameObject ans1, ans2;
    private GameObject arrow;
    private GameObject team;
    private GameObject teamContainer1;
    private GameObject teamContainer2;
    private Color c;
    //Arrays
    private AnswerData[] answers;
    //Lists
    private List<Button> teamsButton = new List<Button>();
    private List<GameObject> teamlist = new List<GameObject>();
    private List<GameObject> answerList = new List<GameObject>();
    private List<QuestionData> questions = new List<QuestionData>();
    private List<PlayerModel> teamsCtrl = new List<PlayerModel>();

    /*
     * This method initialize everything.
     */
    void Start()
    {
        ans1 = GameObject.Find("Answer 1");
        ans2 = GameObject.Find("Answer 2");
        teamContainer1 = GameObject.Find("TeamContainer1");
        teamContainer2 = GameObject.Find("TeamContainer2");
        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            if (i % 2 == 0)
            {
                team = Instantiate(Resources.Load<GameObject>("Prefabs/Team"), teamContainer1.transform);
            }
            else
            {
                team = Instantiate(Resources.Load<GameObject>("Prefabs/Team"), teamContainer2.transform);
            }
            teamsButton.Add(team.GetComponentInChildren<Button>());
            teamsCtrl.Add(team.GetComponentInChildren<PlayerModel>());
            switch (i)
            {
                case 0: c = Color.red; break;
                case 1: c = Color.blue; break;
                case 2: c = new Color(0.78f, 0f, 1f, 1f); break;
                case 3: c = Color.green; break;
                case 4: c = new Color(1f, 0.56f, 0f, 1f); break;
                case 5: c = new Color(0f, 0.85f, 1f, 1f); break;
                case 6: c = Color.magenta; break;
                case 7: c = Color.yellow; break;
            }
            ColorBlock cb = team.GetComponentInChildren<Button>().colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.pressedColor = Color.white;
            team.GetComponentInChildren<Button>().colors = cb;
            team.GetComponentInChildren<PlayerModel>().teamnumber = (i + 1);
            team.GetComponentInChildren<PlayerModel>().answer1 = ans1.GetComponent<TextMeshProUGUI>();
            team.GetComponentInChildren<PlayerModel>().answer2 = ans2.GetComponent<TextMeshProUGUI>();
            team.GetComponentInChildren<PlayerModel>().sfx_answer = GameObject.Find("answer_soundeffecr").GetComponent<AudioSource>();
            foreach (Image x in team.GetComponentsInChildren<Image>())
            {
                if (x.name.Contains("Joker"))
                {
                    x.name = "Joker " + (i + 1);
                }
                if (x.name.Contains("Team"))
                {
                    x.color = c;
                }
            }
        }
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("answerimage"))
        {
            go.GetComponent<CanvasGroup>().alpha = 1;
        }

        /*
         * Initialisation of gameobjects and variables
         */
        string[] datapath = Application.dataPath.Split('/');
        string pathsrc = datapath[0] + '/';
        for (int i = 1; i < datapath.Length - 1; i++)
        {
            pathsrc += '/' + datapath[i];
        }
        localpath = pathsrc + "/Sounds";
        
        goingToNextQuestion = false;
        pauseActivated = false;
        actualQuestion = 1;
        questions = DataModel.CurTopic().Questions;
        numberOfQuestions = questions.Count;

        arrow = GameObject.Find("ArrowButton");
        questionText = GameObject.Find("Question").GetComponent<TextMeshProUGUI>();
        musicSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        Sprite sprite = Resources.Load<Sprite>("Images/" + DataModel.BackgroundName);
        GameObject.Find("Background").GetComponent<Image>().sprite = sprite;

        for (int i = 0; i < teamsButton.Count; i++)
        {
            if (i < DataModel.NumberOfTeams)
            {
                teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
            }
            else
            {
                teamsButton[i].gameObject.SetActive(false);
            }
        }

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("team"))
        {
            teamlist.Add(go);
        }
        teamlist = teamlist.OrderBy(go => go.name).ToList();

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("answer"))
        {
            answerList.Add(go);
        }
        answerList = answerList.OrderBy(go => go.name).ToList();

        /*
         * Afficher les Jokers qui ont ete actives lors de la scene Topics
         */
        for (int i = 0; i < DataModel.Jokers.Length; i++)
        {
            if (DataModel.Jokers[i])
            {
                GameObject.Find("Joker " + (i + 1)).GetComponent<CanvasGroup>().alpha = 1;
            }
        }
        
        RunningQuestions();
    }

    void Update()
    {

        if (EveryoneAnswered() && Time.timeScale == 1f)
        {
            Time.timeScale = 2f;
        }
        // Pause/Resume the game
        if (Input.GetKeyUp(DataModel.Pause))
        {
            Pause();
        }

        //go to next question
        if (Input.GetKeyDown(DataModel.Next))
        {
            GoToNextQuestion();
        }

        if(!first && EveryoneAnswered())
        {
            first = true;
            StartCoroutine(WaitForRealSeconds(1.0f));
            FinalAnswerPhase();
        }
    }
   
    private void RunningQuestions()
    {
        first = false;
        goingToNextQuestion = false;
        isNextAvailable = false;

        EnableTeam();
        ResetTeamsAnswered();
        foreach(GameObject go in answerList)
        {
            go.GetComponent<CanvasGroup>().alpha = 1;
        }
        // Make required objects to disappear at the start of question
        GameObject.Find("ArrowButton").GetComponent<Button>().interactable = false;

        GameObject.Find("QuestionCounter").GetComponent<TextMeshProUGUI>().text = "Question " + actualQuestion + " / " + numberOfQuestions;
        int music_index = Random.Range(1, 4);
        music = Resources.Load<AudioClip>("Sounds/" + DataModel.QuestionMusicName+music_index);
        musicSource.clip = music;
        StartCoroutine(DisplayText());

        foreach (PlayerModel e in teamsCtrl)
        {
            ChangeTeamColor(0, e);
        }

        musicSource.Play();
    }

    public IEnumerator WaitForRealSeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    }
    /**
     * Eliminate a random false answer of the scene
     */
    private void EliminateFalseAnswer()
    {
        int randomIndex = UnityEngine.Random.Range(0, 2);
        while (answers[randomIndex].IsTrue || answerList[randomIndex].GetComponent<CanvasGroup>().alpha == 0)
        {
            randomIndex = UnityEngine.Random.Range(0, 2);
        }
        answerList[randomIndex].GetComponent<CanvasGroup>().alpha = 0;
    }

    /**
     * Eliminate the last false answer and gives the points to the teams that were right and display the answer's color on the team's answer panel
     */
    private void FinalAnswerPhase()
    {
        EliminateFalseAnswer();
        foreach (PlayerModel e in teamsCtrl)
        {
            //change the team's answer button to the color of the one they chose
            ChangeTeamColor(e.GetNumberAnswer(), e);

            for (int i = 0; i < questions.First().Answers.Length; i++)
            {
                currAnswer = questions.First().Answers[i];
                //Check if PlayerControler answered and gave the good answer
                if (e.GetAnswer().Equals(currAnswer.AnswerText) && currAnswer.IsTrue)
                {
                    DataModel.AddScoreToTeam(e.GetCurrentRoundPoints(), teamsCtrl.IndexOf(e));
                }
            }
        }

        for (int i = 0; i < teamsButton.Count; i++)
        {
            teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
        }
        GameObject.Find("ArrowButton").GetComponent<Button>().interactable = true;
        arrow.GetComponent<CanvasGroup>().alpha = 1;
        isNextAvailable = true;
    }

    private void DisableTeam()
    {
        foreach (PlayerModel e in teamsCtrl)
        {
            e.enabled = false;
        }
    }

    private void EnableTeam()
    {
        foreach (PlayerModel e in teamsCtrl)
        {
            e.enabled = true;
        }
    }

    private void ResetTeamsAnswered()
    {
        foreach (PlayerModel e in teamsCtrl)
        {
            e.SetHasAnswered(false);
        }
    }

    /**
     * Change the color of a PlayerControler e based on the integer i which stands for
     * the answer the team chooses. Their color change to the corresponding answer's color.
     */
    private void ChangeTeamColor(int i, PlayerModel e)
    {
        Image img = e.answerPanel.GetComponent<Image>();
        switch (i)
        {
            case 1: //Color of answer number 1
                img.color = new Color(0, 0, 1, 1);
                break;
            case 2: //Color of answer number 2
                img.color = new Color(1, 0.7093226F, 0, 1);
                break;
            case 3: //Color of answer number 3
                img.color = new Color(0.07693553F, 1, 0, 1);
                break;
            case 4: //Color of answer number 4
                img.color = new Color(0.9725245F, 1, 0, 1);
                break;
            default: //White, used when waiting for an answer
                img.color = new Color(1, 1, 1, 1);
                break;
        }
    }//end of changeTeamColor

    /*
     * This method is called once. It is used to display the questions character by character to
     * make it look dynamic. It also displays the answers' text
     */
    private IEnumerator DisplayText()
    {
        visibleCharacterCount = 0;
        while (!goingToNextQuestion)
        {
            answers = questions.First().Answers;
            GameObject.Find("Answer 1").GetComponent<TextMeshProUGUI>().text = answers[0].AnswerText;
            GameObject.Find("Answer 2").GetComponent<TextMeshProUGUI>().text = answers[1].AnswerText;
            TrueFalseQuestion texteQ = (TrueFalseQuestion)DataModel.CurQuestion();
            questionText.text = texteQ.Question;
                
            question_length_to_time = questionText.text.Length * 0.035f; //salut lut comme le biscuit

            yield return null;
        }
    }

    public bool EveryoneAnswered()
    {
        bool res = true;
        foreach (PlayerModel p in teamsCtrl)
        {
            if (!p.GetHasAnswered())
            {
                res = false;
            }
        }
        return res;
    }

    public void Pause()
    {
        if (!pauseActivated)
        {
            //game paused
            musicSource.Pause();
            Time.timeScale = 0f;
            //disable every controller
            //dispay "pause activated" message
            pauseActivated = true;
        }
        else
        {
            //game already in pause i.e. resume game
            Time.timeScale = 1f;
            musicSource.Play();
            //enable every controller
            //display "resume game" message
            pauseActivated = false;
        }
    }

    public void GoToNextQuestion()
    {
        if (Time.timeScale == 2f)
        {
            Time.timeScale = 1f;
        }
        goingToNextQuestion = true;
        if (isNextAvailable)
        {
            musicSource.Stop();
            questions.Remove(DataModel.CurQuestion());
            actualQuestion++;
            if (!questions.Any())
            {
                DataModel.CurTopic().IsAvailable = false;
                isAnyThemeLeftInCurRound = false;
                for (int i = 0; i < DataModel.CurRound().Topics.Count; i++)
                {
                    if (DataModel.CurRound().Topics[i].IsAvailable)
                    {
                        isAnyThemeLeftInCurRound = true;
                    }
                }
                // if there are no more questions or topics, go to next round
                if (!isAnyThemeLeftInCurRound)
                {
                    DataModel.Rounds.Remove(DataModel.CurRound());
                    DataModel.RoundNumber++;
                }
                if (!DataModel.Rounds.Any())
                {
                    DataModel.Save(DataModel.CurrentRunningFilename);
                    SceneManager.LoadScene("Ending");
                }
                //if it is the last question, return to Topics
                else
                {
                    DataModel.Save(DataModel.CurrentRunningFilename);
                    SceneManager.LoadScene("Topics");
                }
            }
            else
            {
                arrow.GetComponent<CanvasGroup>().alpha = 1;
                DataModel.Save(DataModel.CurrentRunningFilename);
                RunningQuestions();
            }
        }
    }
}