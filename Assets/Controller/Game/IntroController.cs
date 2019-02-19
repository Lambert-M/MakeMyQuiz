
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;

public class IntroController : MonoBehaviour
{
    private bool timeWritten;
    private string time;
    private static string pattern;
    private AudioSource introSource;
    private AudioClip introMusic;
    private Timer timer;
    private static TMP_InputField TimeField;
    private List<Button> teamsButton = new List<Button>();
    private GameObject teamContainer;
    private GameObject team;
    private Color c;

    // Use this for initialization
    void Start()
    {

        if (DataModel.CurrentFilename.Contains("running"))
        {
            DataModel.CurrentFilename = DataModel.CurrentFilename.Substring(0, DataModel.CurrentFilename.Length - 12) + ".json";
            DataModel.CurrentRunningFilename = DataModel.CurrentRunningFilename.Substring(0, DataModel.CurrentRunningFilename.Length - 12) + ".json";
        }

        teamContainer = GameObject.FindWithTag("teamcontainer");
        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            team = Instantiate(Resources.Load<GameObject>("Prefabs/Team"), teamContainer.transform);
            teamsButton.Add(team.GetComponentInChildren<Button>());
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
        /*
         * IntroMusic setup
         */
        introSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        introMusic = (AudioClip)Resources.Load("Sounds/"+DataModel.IntroMusicName);
        introSource.clip = introMusic;
        introSource.loop = true;
        introSource.volume = 0.4f;
        introSource.Play();

        /*
         * Initialisation of gameobjects and variables
         */
        Sprite sprite = Resources.Load<Sprite>("Images/" + DataModel.BackgroundName);
        GameObject.Find("Background").GetComponent<Image>().sprite = sprite;
        timeWritten = false;
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        TimeField = GameObject.Find("TimerInput").GetComponent<TMP_InputField>();
        pattern = @"^(\d$|\d{2}$|(\d|\d{2}):$|(\d|\d{2}):(\d|\d{2})$)";

        /*
         * Initialisation of texts
         */
        GameObject.Find("WaitingTime").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["starting_time"];
        GameObject.Find("TimerInput").transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["write_time"];
        GameObject.Find("Introduction").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["welcome_game"] + DataModel.QuizName;
        
        for (int i = 0; i < teamsButton.Count; i++)
        {
            if ( i < DataModel.NumberOfTeams)
            {
                teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
            }
            else
            {
                teamsButton[i].gameObject.SetActive(false);
            }
        }

        if (TimeField)
        {
            TimeField.onValidateInput = ValidateInput;
        }
    }

    static char ValidateInput(string text, int charIndex, char addedChar)
    {
        if (Regex.IsMatch(text+addedChar, pattern))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }
    // Update is called once per frame
    void Update()
    {
        time = GameObject.Find("TimerDisplay").GetComponent<TextMeshProUGUI>().text;
        
        // When the timer written looks something like x:xx at least ( or xx:xx ) we start converting it to time and activate the hidden timer
        if (time.Length > 4 && !timeWritten && (Input.GetKeyDown(KeyCode.Return|KeyCode.KeypadEnter)))
        {
            timeWritten = true;
            string[] minsec = time.Split(':');
            timer.timerValue = float.Parse(minsec[0]) * 60f + float.Parse(minsec[1]);
            timer.ResetTimer();
            timer.tickingDown = true;
        }

        // We desplay the correct time left in the InputField
        if (timeWritten)
        {
            float minutes = Mathf.FloorToInt(timer.GetCurrentTimeValue() / 60F);
            float seconds = Mathf.FloorToInt(timer.GetCurrentTimeValue() % 60);
            TimeField.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        }

        // When the timer hits 0, we write starting soon mand give the possibility to write another timer
        if(timer.GetCurrentTimeValue() < 0.5f && timeWritten)
        {
            TimeField.text = "STARTING SOON";
            timeWritten = false;
        }

        if (Input.GetKeyDown(DataModel.Next))
        {
            GotoTopicScene();
        }
    }

    /**
     * Used on the ArrowButton or when the user press Next
     */
    public void GotoTopicScene()
    {
        introSource.Stop();
        SceneManager.LoadScene("Topics");
    }
}