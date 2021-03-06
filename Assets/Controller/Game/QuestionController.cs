﻿using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class QuestionController : MonoBehaviour
{

    //Variables
    public bool[] teams_can_buzz;
    public int number_team_buzz;
    public bool buzz_event;
    private bool buzz_answer_confirm;
    private bool isAnyThemeLeftInCurRound;
    private bool pauseActivated;
    private bool isNextAvailable;
    private bool musicQuestionIsPlaying;
    private bool resetvol;
    private bool isBuzzActivate;
    private int visibleCharacterCount;
    private int numberOfCharacters;
    private int numberOfQuestions;
    private int actualQuestion;
    private string localpath;
    private float question_length_to_time; // A AJOUTER DANS LE DATA MODEL POUR AJUSTER LES PLAYER MODEL ENSUITE
    //Objects
    private AnswerData currAnswer;
    private Timer timerctrl;
    private TextMeshProUGUI questionText;
    private AudioSource musicSource;
    private AudioClip music;
    private GameObject ans1, ans2, ans3, ans4;
    private GameObject arrow;
    private GameObject team;
    private GameObject teamContainer1;
    private GameObject teamContainer2;
    private Color c;
    public AudioSource[] sfx_buzzers;
    public AudioSource sfx_buzzer_win;
    public AudioSource sfx_buzzer_defeat;
    //Arrays
    private AnswerData[] answers;
    //Lists
    private List<Button> teamsButton = new List<Button>();
    private List<GameObject> teamlist = new List<GameObject>();
    private List<GameObject> answerList = new List<GameObject>();
    private List<GameObject> timerPanelList = new List<GameObject>();
    private List<QuestionData> questions = new List<QuestionData>();
    private List<PlayerModel> teamsCtrl = new List<PlayerModel>();

    /*
     * This method initialize everything.
     */
    void Start()
    {
        ans1 = GameObject.Find("Answer 1");
        ans2 = GameObject.Find("Answer 2");
        ans3 = GameObject.Find("Answer 3");
        ans4 = GameObject.Find("Answer 4");
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
            team.GetComponentInChildren<PlayerModel>().answer3 = ans3.GetComponent<TextMeshProUGUI>();
            team.GetComponentInChildren<PlayerModel>().answer4 = ans4.GetComponent<TextMeshProUGUI>();
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

        teams_can_buzz = new bool[teamsCtrl.Count];
        buzz_event = false;
        buzz_answer_confirm = false;
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
        
        timerctrl = GameObject.Find("Timer").GetComponent<Timer>();
        
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("realtimer"))
        {
            timerPanelList.Add(e);
        }
        timerPanelList = timerPanelList.OrderBy(go => go.name).ToList();

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
 
        isBuzzActivate = DataModel.CurRound().IsBuzzRound;
        RunningQuestions();
    }
    
    void Update()
    {
        if (timerctrl.GetCurrentTimeValue() < 0.1f && Input.GetKeyDown(DataModel.Next))
        {
            GoToNextQuestion();
        }

        if (EveryoneAnswered() && Time.timeScale == 1f)
        {
            Time.timeScale = 2f;
        }
        
        if (isBuzzActivate && buzz_event && !teamsCtrl[number_team_buzz - 1].buzzed)
        {
            DisableTeam();
            
            if (!pauseActivated)
            {
                DisapearAllTeamsButOne(number_team_buzz);
                Pause();
                LaunchSoundBuzzer(number_team_buzz);
            }
            if (Input.GetKeyDown(KeyCode.Y))
            {
                Pause();
                CancelInvoke();
                sfx_buzzer_win.Play();
                buzz_answer_confirm = true;
                DataModel.Scores[number_team_buzz - 1] += 5;
                if (DataModel.Jokers[number_team_buzz-1])
                {
                    DataModel.Scores[number_team_buzz - 1] += 2;
                }
                buzz_event = false;
                isNextAvailable = true;
                ReappearAllTeams();
                arrow.GetComponent<CanvasGroup>().alpha = 1;
                GameObject.Find("ArrowButton").GetComponent<Button>().interactable = true;
                UpdateScoreTeams(number_team_buzz);
                DisplayQuestion();
                DisplayGoodAnswer();
                musicSource.Stop();

            }
            else if (Input.GetKeyDown(KeyCode.N))
            {
                sfx_buzzer_defeat.Play();
                StartCoroutine(WaitForRealSeconds(1.57f));
                ReappearAllTeams();
                teamsCtrl[number_team_buzz - 1].gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
                buzz_answer_confirm = true;
                teamsCtrl[number_team_buzz - 1].SetHasAnswered(true);
                teamsCtrl[number_team_buzz - 1].buzzed = true;
                buzz_event = false;
                EnableTeam();
            }
        }
        else 
        {
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
    
            if (musicQuestionIsPlaying)
            {
                // Make the sound fade in
                MusicQuestion musicQ = (MusicQuestion)DataModel.CurQuestion();
                if (!resetvol && musicQ.Fade)
                {
                    musicSource.volume = 0;
                    resetvol = true;
                }

                if (musicQ.Fade && musicQuestionIsPlaying)
                {
                    if (musicSource.volume < musicQ.Volume)
                    {

                        musicSource.volume = musicSource.volume + (Time.deltaTime / 8);
                    }
                }

                if (timerctrl.GetCurrentTimeValue() < 2f)
                {
                    if (musicSource.volume > 0)
                    {
                        musicSource.volume -= musicQ.Volume * Time.deltaTime / 2f;
                    }
                }
            }
        }
    }


    public IEnumerator WaitForRealSeconds(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        Pause();
    }

  
    private void RunningQuestions()
    {

        if (isBuzzActivate)
        {
            buzz_event = false;
            EnableTeam();
            EnableAllBuzzers();
            ReappearAllTeams();
            ResetTeamsAnswered();
        }
        else
        {
            buzz_event = false;
            EnableTeam();
            ReappearAllTeams();
            ResetTeamsAnswered();
            DisableAllBuzzers();
        }
     
        // Make required objects to disappear at the start of question
        GameObject.Find("ArrowButton").GetComponent<Button>().interactable = false;

        arrow.GetComponent<CanvasGroup>().alpha = 0;
        foreach (GameObject e in timerPanelList)
        {
            e.GetComponent<CanvasGroup>().alpha = 0;
        }
        foreach (GameObject p in answerList)
        {
            p.GetComponent<CanvasGroup>().alpha = 0;
        }

        isNextAvailable = false;

        GameObject.Find("QuestionCounter").GetComponent<TextMeshProUGUI>().text = "Question " + actualQuestion + " / " + numberOfQuestions;

        // Either this is a MusicQuestion and we musicQuestionIsPlaying its music or we musicQuestionIsPlaying the basic question music
        if (DataModel.CurQuestion() is MusicQuestion)
        {
            MusicQuestion musicQ = (MusicQuestion)DataModel.CurQuestion();
            var x = new WWW("file:///"+ localpath +'/'+ musicQ.MusicPath);
            while (!x.isDone)
            {

            }
            musicSource.clip = x.GetAudioClip();
            musicSource.time = musicQ.StartTrack;
            if (!musicQ.Fade)
            {
                musicSource.volume = musicQ.Volume;
            }
            question_length_to_time = 2.5f;
        }
        else
        {
            int music_index = Random.Range(1, 4);
            music = Resources.Load<AudioClip>("Sounds/" + DataModel.QuestionMusicName+music_index);
            musicSource.clip = music;
            musicSource.volume = 0.75f;
          
        }
        
        foreach (PlayerModel e in teamsCtrl)
        {
            ChangeTeamColor(0, e);
            e.buzzed = false;
        }

        if (DataModel.CurQuestion() is MusicQuestion)
        {
            musicQuestionIsPlaying = true;
        }
        musicSource.Play();

        // After 10 seconds, the timer and answers appears, 7 seconds after that a false answer disappears, again 4 seconds after and at 25 sec teams can'musicQ answer
        // anymore. Finally at 28 seconds, the true answer is revealed and points are given

        if (DataModel.CurQuestion() is TextQuestion)
        {

            TextQuestion texteQ = (TextQuestion)DataModel.CurQuestion();
            questionText.text = texteQ.Question;

            //formule de merde a changer
            question_length_to_time = questionText.text.Length * 0.07f;
        }
        else
        {
            questionText.text = DataModel.TextToUse["music_display"] + actualQuestion;
        }
        answers = questions.First().Answers;
        GameObject.Find("Answer 1").GetComponent<TextMeshProUGUI>().text = answers[0].AnswerText;
        GameObject.Find("Answer 2").GetComponent<TextMeshProUGUI>().text = answers[1].AnswerText;
        GameObject.Find("Answer 3").GetComponent<TextMeshProUGUI>().text = answers[2].AnswerText;
        GameObject.Find("Answer 4").GetComponent<TextMeshProUGUI>().text = answers[3].AnswerText;
        StartCoroutine(DisplayText());

        Invoke("RevealAnswers", 3.0f + question_length_to_time);
        Invoke("EliminateFalseAnswer", 10.0f + question_length_to_time);
        Invoke("EliminateFalseAnswer", 14.0f + question_length_to_time);
        Invoke("DisableTeam", 18.0f + question_length_to_time);
        Invoke("FinalAnswerPhase", 21.0f + question_length_to_time);
    }

    /**
     * Launch the buzzer sound of the team in parameter 
     */
     
    private void LaunchSoundBuzzer(int number_team)
    {
        sfx_buzzers[number_team - 1].Play();
    }

    /**
     * Disapear all teams in the scene but the one in parameter 
     */
    private void DisapearAllTeamsButOne(int number_team)
    {
        foreach (PlayerModel e in teamsCtrl)
        {
            if (!teamsCtrl[number_team - 1].Equals(e))
            {
                if (DataModel.Jokers[e.teamnumber - 1])
                {
                    GameObject.Find("Joker " + (e.teamnumber)).GetComponent<CanvasGroup>().alpha = 0f;
                }
                e.gameObject.GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }

    /**
     * Reappear all teams in the scene
     */
    private void ReappearAllTeams()
    {
        foreach (PlayerModel e in teamsCtrl)
        {

            if (e.buzzed)
            {
                if (DataModel.Jokers[e.teamnumber - 1])
                {
                    GameObject.Find("Joker " + (e.teamnumber)).GetComponent<CanvasGroup>().alpha = 0.5f;
                }
                e.gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;

            }
            else
            {
                if (DataModel.Jokers[e.teamnumber - 1])
                {
                    GameObject.Find("Joker " + (e.teamnumber)).GetComponent<CanvasGroup>().alpha = 1f;
                }
                e.gameObject.GetComponent<CanvasGroup>().alpha = 1;
            }

        }
    }
    /**
     * When this method is called, timer and answers appears and players are able to answer
     */
    private void RevealAnswers()
    {
        StopCoroutine(DisplayText());
        DisableAllBuzzers();
        foreach (GameObject p in answerList)
        {
            p.GetComponent<CanvasGroup>().alpha = 1;
        }
        foreach (GameObject e in timerPanelList)
        {
            e.GetComponent<CanvasGroup>().alpha = 1;
        }
        timerctrl.tickingDown = true;
     
    }
    
    /**
     * Eliminate a random false answer of the scene
     */
    private void EliminateFalseAnswer()
    {
        int randomIndex = UnityEngine.Random.Range(0, 4);
        while (answers[randomIndex].IsTrue || answerList[randomIndex].GetComponent<CanvasGroup>().alpha == 0)
        {
            randomIndex = UnityEngine.Random.Range(0, 4);
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
        if (DataModel.CurQuestion() is MusicQuestion)
        {
            musicSource.Pause();
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
            e.buzzed = false;
        }
    }

    private void DisableAllBuzzers()
    {
         for ( int i = 0; i < teams_can_buzz.Length; i++)
        {
            teams_can_buzz[i] = false;
        }
    }

    private void EnableAllBuzzers()
    {

        for (int i = 0; i < teams_can_buzz.Length; i++)
        {
            teams_can_buzz[i] = true;
        }
        foreach (PlayerModel e in teamsCtrl)
        {
            e.buzzed = false;
        }
    }

    /**
     * Update score in the data model of the team in parameter
     */
   private void UpdateScoreTeams(int number_team)
    {
          teamsCtrl[number_team-1].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(number_team - 1);
    }
    /**
     * Display the question in the scene
     */
    private void DisplayQuestion()
    {
        questionText.maxVisibleCharacters = questionText.textInfo.characterCount;
    }
    /**
     * Display the good answer panel in the scene
     */ 
    private void DisplayGoodAnswer()
    {
        answers = questions.First().Answers;
        GameObject.Find("Answer 1").GetComponent<TextMeshProUGUI>().text = answers[0].AnswerText;
        GameObject.Find("Answer 2").GetComponent<TextMeshProUGUI>().text = answers[1].AnswerText;
        GameObject.Find("Answer 3").GetComponent<TextMeshProUGUI>().text = answers[2].AnswerText;
        GameObject.Find("Answer 4").GetComponent<TextMeshProUGUI>().text = answers[3].AnswerText;
        for(int i = 0; i< answers.Length; i++)
        {
            if (answers[i].IsTrue)
            {
                answerList[i].GetComponent<CanvasGroup>().alpha = 1;
            }
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
        Debug.Log("là je repart normalement");
        visibleCharacterCount = 0;
        if (DataModel.CurQuestion() is TextQuestion)
        {
                
            TextQuestion texteQ = (TextQuestion)DataModel.CurQuestion();
            questionText.text = texteQ.Question;

            //formule de merde a changer
            question_length_to_time = questionText.text.Length * 0.07f;
        }
        else
        {
            questionText.text = DataModel.TextToUse["music_display"] + actualQuestion;
        }
        questionText.maxVisibleCharacters = visibleCharacterCount;
        numberOfCharacters = questionText.text.Length;
        while (visibleCharacterCount <= numberOfCharacters)
        {
            visibleCharacterCount++;
            questionText.maxVisibleCharacters = visibleCharacterCount;
            yield return new WaitForSeconds(0.07f);
        }
        yield return null;
    }

    public bool EveryoneAnswered()
    {
        bool res = true;
        foreach(PlayerModel p in teamsCtrl)
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
    private void TransitionCorrectBuzz()
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

    public void GoToNextQuestion()
    {
      
        if (Time.timeScale == 2f)
        {
            Time.timeScale = 1f;
        }
        if (isNextAvailable)
        {
            musicSource.Stop();
            musicQuestionIsPlaying = false;
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
                    if (!EqualityExist())
                    {
                        DataModel.Save(DataModel.CurrentRunningFilename);
                        SceneManager.LoadScene("Ending");
                    }
                    else
                    {
                        DataModel.Save(DataModel.CurrentRunningFilename);
                        SceneManager.LoadScene("EditScores");
                    }
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
                //display next question without answers
                foreach (GameObject go in answerList)
                {
                    go.GetComponent<CanvasGroup>().alpha = 0;
                }

                timerctrl.ResetTimer();

                DataModel.Save(DataModel.CurrentRunningFilename);
                RunningQuestions();
            }
        }
    }
    /**
     * 
     */
    public bool EqualityExist()
    {
        int count = 0;
        foreach (PlayerModel p in teamsCtrl)
        {
            if (DataModel.BestScore() == DataModel.Scores[p.teamnumber - 1])
            {
                count++;
            }
        }

        if (count >= 2)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}