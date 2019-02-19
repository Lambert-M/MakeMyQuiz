
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;

public class IntroController : MonoBehaviour
{
    private bool timeWritten;
    private string time;
    private static string pattern;
    private AudioSource introSource;
    private AudioClip introMusic;
    private Timer timer;
    private static TMP_InputField TimeField;
    private Button[] teamsButton;

    // Use this for initialization
    void Start()
    {

        if (DataModel.CurrentFilename.Contains("running"))
        {
            DataModel.CurrentFilename = DataModel.CurrentFilename.Substring(0, DataModel.CurrentFilename.Length - 12) + ".json";
            DataModel.CurrentRunningFilename = DataModel.CurrentRunningFilename.Substring(0, DataModel.CurrentRunningFilename.Length - 12) + ".json";
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
        teamsButton = GameObject.FindWithTag("teamcontainer").GetComponentsInChildren<Button>();
        timer = GameObject.Find("Timer").GetComponent<Timer>();
        TimeField = GameObject.Find("TimerInput").GetComponent<TMP_InputField>();
        pattern = @"^(\d$|\d{2}$|(\d|\d{2}):$|(\d|\d{2}):(\d|\d{2})$)";

        /*
         * Initialisation of texts
         */
        GameObject.Find("WaitingTime").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["starting_time"];
        GameObject.Find("TimerInput").transform.Find("Text Area").transform.Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["write_time"];
        GameObject.Find("Introduction").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["welcome_game"] + DataModel.QuizName;


        for (int i = 0; i < teamsButton.Length; i++)
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