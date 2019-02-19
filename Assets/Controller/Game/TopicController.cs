using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class TopicController : MonoBehaviour {
    
    private string selectedButtonName;
    private int nbTopic;
    private int counter;
    private RectTransform parentTransform;
    private GridLayoutGroup grid;
    private GameObject topic;
    private GameObject team;
    private GameObject parent;
    private GameObject selectedButton;
    private GameObject teamContainer;
    private Color c;
    private AudioClip topicMusic;
    private AudioSource topicSource;
    private List<PlayerModel> teamsCtrl = new List<PlayerModel>();
    private List<Button> teamsButton = new List<Button>();
    private List<Button> topicButtons = new List<Button>();

    // Use this for initialization
    void Start() {

        /*
         *  Initialisation of gameobjects and variables
         */
        DisableTeam();

        teamContainer = GameObject.FindWithTag("teamcontainer");
        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            team = Instantiate(Resources.Load<GameObject>("Prefabs/Team"), teamContainer.transform);
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
            foreach(Image x in team.GetComponentsInChildren<Image>())
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

        parent = GameObject.Find("TopicContainer");
        parentTransform = parent.transform.GetComponent<RectTransform>();
        grid = parent.transform.GetComponent<GridLayoutGroup>();
        counter = 0;
        Sprite sprite = Resources.Load<Sprite>("Images/" + DataModel.BackgroundName);
        GameObject.Find("Background").GetComponent<Image>().sprite = sprite;

        /*
         * We create a number of buttons equal to the number of topics in the current round
         */
        foreach ( TopicData t in DataModel.CurRound().Topics )
        {
            topic = Instantiate(Resources.Load<GameObject>("Prefabs/Topic"), parent.transform);
            topic.name = "Topic" + (counter+1);
            topic.GetComponent<Button>().onClick.AddListener(() => SelectingTopic());
            counter++;
        }

        /*
         *  Adjust the size of the buttons to always fit the same space despite the number
         */
        float height = parentTransform.rect.height;
        float width = parentTransform.rect.width;
        if ( counter%2 == 0)
        {
            grid.cellSize = new Vector2(grid.cellSize.x,Mathf.Min(height / (counter / 2) - grid.spacing.y,270));
        }
        else
        {
            grid.cellSize = new Vector2(grid.cellSize.x, Mathf.Min(height / (counter / 2) - grid.spacing.y, 270));
        }

        /*
         *  Initialisation of other gameobjects and variables
         */
        counter = 0;
        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Topics"))
        {
            topicButtons.Add(go.GetComponent<Button>());
        }
        topicButtons.OrderBy(b => b.name);

        for ( int i = 0; i < DataModel.Jokers.Length; i++ )
        {
            DataModel.Jokers[i] = false;
        }

        foreach (Button b in teamsButton)
        {
            b.onClick.AddListener(() => b.gameObject.GetComponent<PlayerModel>().ActivateJoker());
        }
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

        teamsButton = teamContainer.GetComponentsInChildren<Button>().OrderBy(go => go.name).ToList();

        foreach (Button b in topicButtons)
        {
            b.onClick.AddListener(() => SelectingTopic());
        }

        /*
         *  TopicMusic setup
         */
        topicSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        topicMusic = (AudioClip)Resources.Load("Sounds/" + DataModel.TopicMusicName);
        topicSource.clip = topicMusic;
        topicSource.loop = true;
        topicSource.volume = 0.4f;
        topicSource.Play();

        DisplayText();

        // Erase topics already done
        foreach (TopicData t in DataModel.CurRound().Topics)
        {
            if (!t.IsAvailable)
            {
                foreach (Button b in topicButtons)
                {
                    if (b.GetComponentInChildren<TextMeshProUGUI>().text == t.Name)
                    {
                        b.gameObject.SetActive(false);
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update() {

        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
        }

        if ( Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) )
        {
            if ( Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1) )
            {
                DataModel.Scores[0] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                DataModel.Scores[1] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                DataModel.Scores[2] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                DataModel.Scores[3] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                DataModel.Scores[4] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                DataModel.Scores[5] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                DataModel.Scores[6] += 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                DataModel.Scores[7] += 1;
            }
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                DataModel.Scores[0] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                DataModel.Scores[1] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                DataModel.Scores[2] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                DataModel.Scores[3] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5))
            {
                DataModel.Scores[4] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6))
            {
                DataModel.Scores[5] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7))
            {
                DataModel.Scores[6] -= 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8))
            {
                DataModel.Scores[7] -= 1;
            }
        }

        // Make joker appear or disappear
        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            if (DataModel.Jokers[i])
            {
                GameObject.Find("Joker "+(i+1)).GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                GameObject.Find("Joker "+(i+1)).GetComponent<CanvasGroup>().alpha = 0;
            }
        }
    }

    /**
     * Used on topic buttons to load the questions of the chosen topic
     */
    public void SelectingTopic()
    {
        selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        selectedButtonName = selectedButton.GetComponentInChildren<TextMeshProUGUI>().text;
        DataModel.CurTopicName = selectedButtonName;
        topicSource.Stop();
        if (DataModel.CurRound().Type.StartsWith("Image"))
        {
            SceneManager.LoadScene("Images");
        }
        else
        {
            SceneManager.LoadScene("Questions");
        }
    }

    private void DisableTeam()
    {
        foreach (PlayerModel e in teamsCtrl)
        {
            e.enabled = false;
        }
    } 

    /**
     * Display the correct text on the scene
     */ 
    private void DisplayText()
    {
        GameObject.Find("Round").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["round_name"]+" "+DataModel.RoundNumber
            +" - "+ TypeName();
        counter = 0;
        foreach ( Button b in topicButtons )
        {
             b.GetComponentInChildren<TextMeshProUGUI>().text = DataModel.CurRound().Topics[counter].Name;
             counter++;
        }
    }

    /**
     * Gives the correct type of rounds despite the langage they were saved with
     */
    private string TypeName()
    {
        switch (DataModel.CurRound().Type)
        {
            case "QCM": return DataModel.TextToUse["MCQ_name"];
            case "MCQ": return DataModel.TextToUse["MCQ_name"];
            case "Blind test": return DataModel.TextToUse["blindtest_name"];
            case "Musique": return DataModel.TextToUse["blindtest_name"];
            default: return "Images";
        }
    }

    
}
