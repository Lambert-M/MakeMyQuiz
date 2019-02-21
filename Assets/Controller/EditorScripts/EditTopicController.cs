using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using TMPro;

public class EditTopicController : MonoBehaviour {

    public RectTransform topicPrefab;
    private int nbPanel;
    private int nbQuestions;

	// Use this for initialization
	void Start ()
    {
        nbPanel = 0;

        GameObject.Find("MenuButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backmain"];
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        GameObject.Find("BackToRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backround"];
        GameObject.Find("BackToRound").GetComponent<Button>().onClick.AddListener(() => BackToRoundMenu());
        GameObject.Find("AddTopic").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_addtopic"];
        GameObject.Find("AddTopic").GetComponent<Button>().onClick.AddListener(() => NewTopicPanelData());
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_topics"];
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text += "\n " + DataModel.TextToUse["round_name"] + " " + (DataModel.IroundCur + 1);
        
        LoadAllPanels();
	}
	
	// Update is called once per frame
	void Update () {
        GameObject.Find("NumberTopics").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["topic_number"] + nbPanel;
    }

    /**
     * Create a new panel (dedicated to Topic) in the UI for the scene ETopic
     **/
    public void NewTopicPanelLoad()
    {
        nbPanel++;
        topicPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/TopicSample"), GameObject.Find("TGrid").transform);
        topicPrefab.name = "TopicPanel" + nbPanel;
        topicPrefab.GetComponent<PanelModel>().PanelNumber = nbPanel;
        topicPrefab.Find("TField").Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["enter_topicname"];
        topicPrefab.Find("EditTopic").GetComponent<Button>().onClick.AddListener(() => EditTopic());
        topicPrefab.Find("EditTopic").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_edit"];
        topicPrefab.Find("DeleteTopic").GetComponent<Button>().onClick.AddListener(() => DeleteTopic());
        topicPrefab.Find("DeleteTopic").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        nbQuestions = DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Count;
        topicPrefab.Find("NumberofContainer").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["question_number"] + nbQuestions;
    }

    /**
     * Create a new topic in the DataModel
     **/
    public void NewTopicPanelData()
    {
        //Max number of topic = 16
        if (nbPanel < 16)
        {
            nbPanel++;
            topicPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/TopicSample"), GameObject.Find("TGrid").transform);
            topicPrefab.GetComponent<PanelModel>().PanelNumber = nbPanel;
            topicPrefab.name = "TopicPanel" + nbPanel;
            topicPrefab.Find("TField").Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["enter_topicname"];
            topicPrefab.Find("EditTopic").GetComponent<Button>().onClick.AddListener(() => EditTopic());
            topicPrefab.Find("EditTopic").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_edit"];
            topicPrefab.Find("DeleteTopic").GetComponent<Button>().onClick.AddListener(() => DeleteTopic());
            topicPrefab.Find("DeleteTopic").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
            DataModel.Rounds[DataModel.IroundCur].Topics.Add(new TopicData(DataModel.TextToUse["enter_topicname"], new List<QuestionData>()));
            nbQuestions = DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Count;
            topicPrefab.Find("NumberofContainer").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["question_number"] + nbQuestions;
        }
    }

    /**
     * Create in the UI a given number of panel corresponding to the number of topic in the current state of the DataModel
     * Used when starting the ETopics scene to load all the previously entered topics
     **/
    public void LoadAllPanels()
    {
        int counter = 0;
        foreach(TopicData t in DataModel.Rounds[DataModel.IroundCur].Topics)
        {
            DataModel.ItopicCur = counter;
            NewTopicPanelLoad();
            topicPrefab.Find("TField").GetComponent<TMP_InputField>().text = t.Name;
            counter++;
        }
    }

    /**
     * Lead to the right scene depending on the selected quiz type
     **/
    public void EditTopic()
    {
        GeneralSave();
        int topicnum = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
        DataModel.ItopicCur = topicnum-1; //update the index of current topic
        switch(DataModel.Rounds[DataModel.IroundCur].Type)
        {
            case "TrueFalse": SceneManager.LoadScene("ETrueFalse"); break;
            case "VraiFaux": SceneManager.LoadScene("ETrueFalse"); break;
            case "QCM": SceneManager.LoadScene("ETextQuestion"); break;
            case "MCQ": SceneManager.LoadScene("ETextQuestion"); break;
            case "Blind test": SceneManager.LoadScene("EBlindtest"); break;
            case "Musique": SceneManager.LoadScene("EBlindtest"); break;
            case "Image": SceneManager.LoadScene("EImages"); break;
            default: Debug.LogError("Type de Quiz inconnu"); break;
        }
    }

    public void GeneralSave()
    {
        PanelModel[] topic = gameObject.GetComponentsInChildren<PanelModel>();

        foreach (PanelModel ti in topic)
        {
            int topicNum = ti.GetComponent<PanelModel>().PanelNumber;
            TMP_InputField topicInput = ti.GetComponentInChildren<TMP_InputField>(); //get the topic name
            DataModel.Rounds[DataModel.IroundCur].Topics[topicNum-1].Name = topicInput.text;
        }

        DataModel.Save(DataModel.CurrentFilename);
    }

    /**
     * Remove the selected topic from the DataModel and destroy the associated panel in the UI
     * Updating the number of question panel
     **/
    public void DeleteTopic()
    {
        if (nbPanel > 1)
        {
            int nbDelete = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
            DataModel.Rounds[DataModel.IroundCur].Topics.Remove(DataModel.Rounds[DataModel.IroundCur].Topics[nbDelete-1]);

            Destroy(GameObject.Find("TopicPanel" + nbDelete));
            nbPanel--;
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("TopicPanel"))
            {
                if (e.GetComponent<PanelModel>().PanelNumber > nbDelete)
                {
                    e.GetComponent<PanelModel>().PanelNumber--;
                    e.name = "TopicPanel" + e.GetComponent<PanelModel>().PanelNumber;
                }
            }
        }
    }

    public void BackToMainMenu()
    {
        GeneralSave();
        SceneManager.LoadScene("EMenus");
    }

    public void BackToRoundMenu()
    {
        GeneralSave();
        SceneManager.LoadScene("ERounds");
    }
}
