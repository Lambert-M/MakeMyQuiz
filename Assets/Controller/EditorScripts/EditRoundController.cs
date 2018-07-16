using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

public class EditRoundController : MonoBehaviour
{
    //VARIABLES
    public RectTransform round;
    private int nbPanel;
    private int nbTopics;

    // Use this for initialization
    void Start()
    {
        GameObject.Find("MenuButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backmain"];
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        GameObject.Find("AddRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_addround"];
        GameObject.Find("AddRound").GetComponent<Button>().onClick.AddListener(() => AddRound());
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_rounds"];

        nbPanel = 0;
        GameObject.Find("NumberRound").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["round_number"] + nbPanel;
        LoadAllPanels();
    }

    /**
     * @author : Christophe SAHID, Léo ROUZIC
     * Used by LoadAllPanels
     */
    public void AddRoundLoad()
    {
        nbPanel++;
        //load the prefab RoundSample and use RGrid as its parent
        round = Instantiate(Resources.Load<RectTransform>("Prefabs/RoundSample"), GameObject.Find("RGrid").transform);
        round.name = "RoundSample" + nbPanel;
        round.GetComponent<PanelModel>().PanelNumber = nbPanel;
        //link DeleteRound button and deleteRound() method
        round.Find("DeleteRound").GetComponent<Button>().onClick.AddListener(() => DeleteRound());
        round.Find("DeleteRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        //link the EditRound button and editRound() method
        round.Find("EditRound").GetComponent<Button>().onClick.AddListener(() => ERound());
        round.Find("EditRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_edit"];
        round.Find("Qtype").GetComponent<TMP_Dropdown>().options[1].text = DataModel.TextToUse["blindtest_name"];
        round.Find("Qtype").GetComponent<TMP_Dropdown>().options[2].text = DataModel.TextToUse["MCQ_name"];
        round.Find("Qtype").GetComponent<TMP_Dropdown>().interactable = false;
        //update texts
        GameObject.Find("NumberRound").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["round_number"] + nbPanel;
        nbTopics = DataModel.Rounds[round.GetComponent<PanelModel>().PanelNumber - 1].Topics.Count;
        round.Find("NumberofContainer").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["topic_number"] + nbTopics;
    }

    /**
     * @author : Léo ROUZIC, Christophe SAHID
     * Used by the AddRound button 
     */
    public void AddRound()
    {
        nbPanel++;
        //load the prefab RoundSample and use RGrid as its parent
        round = Instantiate(Resources.Load<RectTransform>("Prefabs/RoundSample"), GameObject.Find("RGrid").transform);
        round.name = "RoundSample" + nbPanel;
        round.GetComponent<PanelModel>().PanelNumber = nbPanel;
        //link DeleteRound button and deleteRound() method
        round.Find("DeleteRound").GetComponent<Button>().onClick.AddListener(() => DeleteRound());
        round.Find("DeleteRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        //link the EditRound button and editRound() method
        round.Find("EditRound").GetComponent<Button>().onClick.AddListener(() => ERound());
        round.Find("EditRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_edit"];
        //update texts
        GameObject.Find("NumberRound").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["round_number"] + nbPanel;
        round.Find("Qtype").GetComponent<TMP_Dropdown>().options[1].text = DataModel.TextToUse["blindtest_name"];
        round.Find("Qtype").GetComponent<TMP_Dropdown>().options[2].text = DataModel.TextToUse["MCQ_name"];
        // add round in DataModel with one topic
        List<TopicData> topiccommon = new List<TopicData>();
        topiccommon.Add(new TopicData("", new List<QuestionData>()));
        DataModel.Rounds.Add(new RoundData("Image", topiccommon));
        nbTopics = DataModel.Rounds[round.GetComponent<PanelModel>().PanelNumber - 1].Topics.Count;
        round.Find("NumberofContainer").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["topic_number"] + nbTopics;
    }

    /**
     * @author : Christophe SAHID, Léo ROUZIC
     * Removes the selected round
     */
    public void DeleteRound()
    {
        //get the index of the round to delete
        int nbDelete = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
        DataModel.Rounds.Remove(DataModel.Rounds[nbDelete - 1]);

        nbPanel--;
        Destroy(GameObject.Find("RoundSample" + nbDelete));
        GameObject.Find("NumberRound").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["round_number"] + nbPanel;
        // edit the number associated to each RoundSample (avoid having two non-consecutive numbers)
        foreach (GameObject g in GameObject.FindGameObjectsWithTag("RoundPanels"))
        {
            if (g.GetComponent<PanelModel>().PanelNumber >= nbDelete)
            {
                g.GetComponent<PanelModel>().PanelNumber--;
                g.name = "RoundSample" + g.GetComponent<PanelModel>().PanelNumber;
            }
        }
    }

    /**
     * @author : Léo ROUZIC
     * Used by the Edit round button
     */
    public void ERound()
    {
        GeneralSave();
        int indexRound = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;//get the Round index
        DataModel.IroundCur = indexRound-1;
        SceneManager.LoadScene("ETopic");
    }

    public void GeneralSave()
    {
        PanelModel[] rounds = gameObject.GetComponentsInChildren<PanelModel>();

        foreach (PanelModel r in rounds)
        {
            int roundNum = r.GetComponent<PanelModel>().PanelNumber;
            TMP_Dropdown type = r.GetComponentInChildren<TMP_Dropdown>(); //get the round type
            DataModel.Rounds[roundNum-1].Type = type.options[type.value].text; //update round type
        }

        DataModel.Save(DataModel.CurrentFilename);
    }
    
    public void BackToMainMenu()
    {
        GeneralSave();
        SceneManager.LoadScene("EMenus");
    }

    /**
     * @author Léo
     * 
     * Create in the UI a given number of panel corresponding to the number of round in the current state of the DataModel
     * Used when starting the ERounds scene to load all the previously entered rounds
     **/
    public void LoadAllPanels()
    {
        foreach (RoundData r in DataModel.Rounds)
        {
            AddRoundLoad();
            switch (r.Type)
            {
                case "QCM":
                    round.GetComponentInChildren<TMP_Dropdown>().value = 2;
                    break;
                case "MCQ":
                    round.GetComponentInChildren<TMP_Dropdown>().value = 2;
                    break;
                case "Blind test":
                    round.GetComponentInChildren<TMP_Dropdown>().value = 1;
                    break;
                case "Musique":
                    round.GetComponentInChildren<TMP_Dropdown>().value = 1;
                    break;
                case "Image":
                    round.GetComponentInChildren<TMP_Dropdown>().value = 0;
                    break;
            }
        }
    }
}
