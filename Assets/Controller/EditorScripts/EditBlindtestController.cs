using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using GracesGames.SimpleFileBrowser.Scripts;
using System.IO;
using System;
using UnityEngine.SceneManagement;

/**
 * @authors : Léo ROUZIC
 */
public class EditBlindtestController : MonoBehaviour
{
    //VARIABLES
    public RectTransform btPrefab;
    private int nbQ;

    private bool play;
    private readonly string[] fileExtensions = { "ogg", "wav" } ;
    private bool resetVol;
    private GameObject currentObj;
    private AudioSource music;
    private string filename;
    private string pathDestSound;
    private bool musicStop;

    // Use this for initialization
    void Start()
    {
        string[] datapath = Application.dataPath.Split('/');
        string pathsrc = datapath[0] + '/';
        for (int i = 1; i < datapath.Length - 1; i++)
        {
            pathsrc += '/' + datapath[i];
        }
        pathDestSound = pathsrc + '/' + "Sounds";

        GameObject.Find("MenuButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backmain"];
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        GameObject.Find("BackToTopics").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backtopic"];
        GameObject.Find("BackToTopics").GetComponent<Button>().onClick.AddListener(() => BackToTopicMenu());
        GameObject.Find("AddQuestion").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_addquestion"];
        GameObject.Find("AddQuestion").GetComponent<Button>().onClick.AddListener(() => NewQuestionPanelData());
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_blindtest"];
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text += "\n " + DataModel.TextToUse["topic_name"] + " " + DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Name;
        nbQ = 0;
        musicStop = true;
        LoadAllQuestionsPanel();
       

    }

    // Update is called once per frame
    void Update()
    {
        Utilitary.NavigationUtility();
        GameObject.Find("NumberQuestions").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["question_number"] + nbQ;
        if (play)
        {
            if (!resetVol && currentObj.transform.Find("ValidateQuestion").Find("FadeIn").GetComponent<Toggle>().isOn)
            {
                music.volume = 0;
                resetVol = true;
            }

            if (currentObj.transform.Find("ValidateQuestion").Find("FadeIn").GetComponentInChildren<Toggle>().isOn && play)
            {
                if (music.volume < currentObj.GetComponent<PanelModel>().Volume)
                {
                    music.volume = music.volume + (Time.deltaTime / 10);
                }
            }
            DisplayTime();
        }
      
    }

    /**
     * @author Léo, Maxime
     * 
     * Create a new panel (dedicated to Blindtest) in the UI for the scene EBlindtest
     **/
    public void NewQuestionPanelLoad()
    {
        nbQ++;
        btPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/BTSample"), GameObject.Find("BTGrid").transform);
        btPrefab.name = "BTSample" + nbQ;
        btPrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
        btPrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        btPrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        btPrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        btPrefab.Find("ValidateQuestion").Find("Validate").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        btPrefab.Find("Play").GetComponent<Button>().onClick.AddListener(() => Play());
        btPrefab.Find("Stop").GetComponent<Button>().onClick.AddListener(() => Stop());
        btPrefab.Find("Pause").GetComponent<Button>().onClick.AddListener(() => Pause());
        btPrefab.Find("ValidateQuestion").Find("ImportButton").GetComponent<Button>().onClick.AddListener(() => OpenMusicFileBrowser());
        btPrefab.Find("ValidateQuestion").Find("ImportButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["import_music"];
        btPrefab.Find("ValidateQuestion").Find("FadeIn").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["music_fadein"];
        btPrefab.Find("ValidateQuestion").Find("TimeController").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["music_starttime"];

        for (int i = 1; i < 5; i++)
        {
            btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
    }

    /**
     * @author Léo
     * 
     * Create in the UI a given number of panel corresponding to the number of questions in the current state of the DataModel
     * Used when starting the EBlindtest scene to load all the previously entered datas
     **/
    public void LoadAllQuestionsPanel()
    {
        foreach (MusicQuestion qd in DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions)
        {
            NewQuestionPanelLoad();
            var x = new WWW("file:///" + pathDestSound + '/' + qd.MusicPath);
            // waiting for the audio to load entirely
            while (!x.isDone)
            {

            }
            btPrefab.GetComponent<AudioSource>().clip = x.GetAudioClip();
            btPrefab.GetComponent<PanelModel>().Volume = qd.Volume;
            btPrefab.GetComponent<PanelModel>().StartTime = qd.StartTrack;
            btPrefab.GetComponent<PanelModel>().ImportDone = true;
            btPrefab.GetComponent<PanelModel>().Filepath = qd.MusicPath;   
            btPrefab.Find("TrackName").GetComponentInChildren<TextMeshProUGUI>().text = qd.MusicPath.Split('/')[qd.MusicPath.Split('/').Length-1];
            btPrefab.Find("ValidateQuestion").Find("FadeIn").GetComponent<Toggle>().isOn = qd.Fade;
            music = btPrefab.GetComponent<AudioSource>();
            music.time = qd.StartTrack;
            music.volume = qd.Volume;
            btPrefab.Find("ValidateQuestion").Find("TimeController").GetComponent<Slider>().value = qd.StartTrack / btPrefab.GetComponent<AudioSource>().clip.length;
            btPrefab.Find("ValidateQuestion").Find("TimeController").GetComponent<Slider>().onValueChanged.AddListener((y) => ChangeTime(y));
            btPrefab.Find("ValidateQuestion").Find("VolumeController").GetComponent<Slider>().value = qd.Volume;
            btPrefab.Find("ValidateQuestion").Find("VolumeController").GetComponent<Slider>().onValueChanged.AddListener((y) => ChangeVolume(y));
            btPrefab.Find("ValidateQuestion").Find("VolumeController").Find("VolumeDisplay").GetComponent<TextMeshProUGUI>().text = "Volume : " + Math.Round(qd.Volume, 2) * 100 + "%";

            currentObj = btPrefab.gameObject;
            DisplayTime();

            for (int i = 0; i < qd.Answers.Length; i++)
            {
                btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").gameObject.GetComponentsInChildren<TMP_InputField>()[i].text = qd.Answers[i].AnswerText;
                if (qd.Answers[i].IsTrue)
                {
                    btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>()[i].GetComponentInChildren<Toggle>().isOn = true;
                }
            }
        }
    }

    /**
     * Create a new question in the DataModel and fill it with default value
     **/
    public void NewQuestionPanelData()
    {
        nbQ++;
        btPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/BTSample"), GameObject.Find("BTGrid").transform);
        // set the new panel attributes to default values
        btPrefab.name = "BTSample" + nbQ;
        btPrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
        btPrefab.GetComponent<PanelModel>().Volume = 1f;
        btPrefab.GetComponent<PanelModel>().StartTime = 0f;
        btPrefab.GetComponent<PanelModel>().ImportDone = false;
        // updating interface (language, events while clicking on button)
        btPrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        btPrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        btPrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        btPrefab.Find("ValidateQuestion").Find("Validate").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        btPrefab.Find("Play").GetComponent<Button>().onClick.AddListener(() => Play());
        btPrefab.Find("Stop").GetComponent<Button>().onClick.AddListener(() => Stop());
        btPrefab.Find("Pause").GetComponent<Button>().onClick.AddListener(() => Pause());
        btPrefab.Find("ValidateQuestion").Find("ImportButton").GetComponent<Button>().onClick.AddListener(() => OpenMusicFileBrowser());
        btPrefab.Find("ValidateQuestion").Find("ImportButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["import_music"];
        btPrefab.Find("ValidateQuestion").Find("FadeIn").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["music_fadein"];
        btPrefab.Find("ValidateQuestion").Find("TimeController").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["music_starttime"];
        btPrefab.Find("ValidateQuestion").Find("VolumeController").GetComponent<Slider>().onValueChanged.AddListener((x) => ChangeVolume(x));
        btPrefab.Find("ValidateQuestion").Find("TimeController").GetComponent<Slider>().onValueChanged.AddListener((x) => ChangeTime(x));
        for (int i = 1; i < 5; i++)
        {
            btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
        //add a neutral question when user press the AddQuestion button and add it to the DataModel
        AnswerData[] ans = { new AnswerData("a", true), new AnswerData("b", false), new AnswerData("c", false), new AnswerData("d", false) };
        MusicQuestion question = new MusicQuestion("new question", ans);
        btPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A1Field").Find("TrueA1").GetComponent<Toggle>().isOn = true;
        DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Add(question);
    }

    /**
     * Remove the selected question from the DataModel and destroy the associated panel in the UI
     * Updating the number of question panel
     **/
    public void RemoveQuestion()
    {
        int nbDelete = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
        DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Remove(DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[nbDelete - 1]);
        Destroy(GameObject.Find("BTSample" + nbDelete));
        nbQ--;
        // edit the number associated to each BTSample (avoid having two non-consecutive numbers)
        foreach (GameObject e in GameObject.FindGameObjectsWithTag("BTSample"))
        {
            if (e.GetComponent<PanelModel>().PanelNumber > nbDelete)
            {
                e.GetComponent<PanelModel>().PanelNumber--;
                e.name = "BTSample" + e.GetComponent<PanelModel>().PanelNumber;
            }
        }
    }

    public void ChangeTime(float x)
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        currentObj.GetComponentInParent<PanelModel>().StartTime = x*music.clip.length;
        music.time = music.clip.length * x;
        DisplayTime();
    }

    public void ChangeVolume(float x)
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        currentObj.GetComponentInParent<PanelModel>().Volume = x;
        EventSystem.current.currentSelectedGameObject.transform.Find("VolumeDisplay").GetComponent<TextMeshProUGUI>().text = "Volume : " + Math.Round(x,2)*100 + "%";
        music.volume = x;
   
    }
    
    public void DisplayTime()
    {
        int minutescur;
        int secondscur;

        if (!musicStop)
        {
            minutescur = Mathf.FloorToInt(music.time / 60F);
            secondscur = Mathf.FloorToInt(music.time % 60);

        }
        else
        {
            minutescur = Mathf.FloorToInt(currentObj.GetComponentInParent<PanelModel>().StartTime / 60F);
            secondscur = Mathf.FloorToInt(currentObj.GetComponentInParent<PanelModel>().StartTime % 60);
        }
        int minutes = Mathf.FloorToInt(music.clip.length / 60F);
        int seconds = Mathf.FloorToInt(music.clip.length % 60);
        string currtime = string.Format("{0:0}:{1:00}", minutescur, secondscur);
        string niceTime = string.Format("{0:0}:{1:00}", minutes, seconds);
        currentObj.transform.Find("ValidateQuestion").Find("TimeDisplayer").GetComponent<TextMeshProUGUI>().text = currtime + " / " + niceTime;

    }

    /**
     * Open the file browser to select a music
     **/
    public void OpenMusicFileBrowser()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        GameObject fileBrowserObject = Instantiate((GameObject)Resources.Load("Prefabs/FileBrowser"), transform);
        fileBrowserObject.name = "FileBrowser";
        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        fileBrowserScript.SetupFileBrowser(ViewMode.Landscape);
        fileBrowserScript.OpenFilePanel(fileExtensions);
        GameObject.Find("SelectFileButton").GetComponent<Button>().onClick.AddListener(() => ChargeMusic());
    }

    public void Play()
    {
        if (!play)
        {
            currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
            music = currentObj.GetComponentInParent<AudioSource>();
            music.Play();
            play = true;
            musicStop = false;
        }
    }

    public void Pause()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        music.Pause();
        play = false;
    }

    public void Stop()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        music.Stop();
        play = false;
        resetVol = false;
        musicStop = true;
        DisplayTime();
    }

    public void SaveQuestion()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<AudioSource>().gameObject;
        if (currentObj.GetComponent<PanelModel>().ImportDone)
        { 
            int QuestionNumber = currentObj.GetComponentInParent<PanelModel>().PanelNumber;
            TMP_InputField[] answersInput = EventSystem.current.currentSelectedGameObject.transform.Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>(); //get the answers
            Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
            List<AnswerData> answers = new List<AnswerData>();
            for (int i = 0; i < answersInput.Length; i++)
            {
                answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of an answer             
                answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));
            }
            MusicQuestion question = new MusicQuestion(currentObj.GetComponentInParent<PanelModel>().Filepath, answers.ToArray()); //create a new MusicQuestion
            question.Fade = EventSystem.current.currentSelectedGameObject.transform.Find("FadeIn").GetComponentInChildren<Toggle>().isOn;
            question.StartTrack = currentObj.GetComponentInParent<PanelModel>().StartTime;
            question.Volume = currentObj.GetComponentInParent<PanelModel>().Volume;
            DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question in the current topic in DataModel
        }
        else
        {
            StartCoroutine(ChangeColor());
        }
    }

    IEnumerator ChangeColor ()
    {
        currentObj.transform.Find("ValidateQuestion").Find("ImportButton").GetComponent<Image>().color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);
        currentObj.transform.Find("ValidateQuestion").Find("ImportButton").GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void ChargeMusic()
    {
        music = currentObj.GetComponent<AudioSource>();
        string path = GameObject.Find("LoadFileText").GetComponent<Text>().text;
        path = path.Replace('\\', '/');
        string[] pathsplit = path.Split('/');
        string filename = pathsplit[pathsplit.Length - 1];

        if (!File.Exists(pathDestSound + '/' + filename))
        {
            File.Copy(path, pathDestSound + '/' + filename);
        }
      
        var x = new WWW("file:///" + pathDestSound + '/' + filename);
        //waiting for the audio to load entirely
        while (!x.isDone)
        {
           
        }
        music.clip = x.GetAudioClip();
        currentObj.GetComponent<PanelModel>().Filepath = filename;
        currentObj.GetComponent<PanelModel>().ImportDone = true;
        currentObj.transform.Find("TrackName").GetComponentInChildren<TextMeshProUGUI>().text = filename;
    }
    
    public bool GeneralSave()
    {
        bool allpassed = true;
        PanelModel[] questions = gameObject.GetComponentsInChildren<PanelModel>();
        foreach (PanelModel qi in questions)
        {
            if (qi.ImportDone)
            {
                int QuestionNumber = qi.PanelNumber;
                
                TMP_InputField[] answersInput = qi.transform.Find("ValidateQuestion").Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>(); //get the answers
                Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
                List<AnswerData> answers = new List<AnswerData>();

                for (int i = 0; i < answersInput.Length; i++)
                {
                    answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of an answer             
                    answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));            
                }
                MusicQuestion question = new MusicQuestion(qi.Filepath, answers.ToArray()); //create a new MusicQuestion
                question.MusicPath = qi.Filepath;
                question.Fade = qi.transform.Find("ValidateQuestion").Find("FadeIn").GetComponent<Toggle>().isOn;
                question.StartTrack = qi.StartTime;
                question.Volume = qi.Volume;
                DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question in the current topic in DataModel
            }
            else
            {
                allpassed = false;
                currentObj = qi.gameObject;
                StartCoroutine(ChangeColor());
                break;
            }
        }

        if (allpassed)
        {
            DataModel.Save(DataModel.CurrentFilename);
        }
        return allpassed;
    }
    
    public void BackToMainMenu()
    {
        if (GeneralSave())
        {
            SceneManager.LoadScene("EMenus");
        }
    }

    public void BackToTopicMenu()
    {
        if (GeneralSave())
        {
            SceneManager.LoadScene("ETopic");
        }
    }
}
