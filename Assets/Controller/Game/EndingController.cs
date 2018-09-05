using TMPro;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class EndingController : MonoBehaviour {

    private int counter;
    private int indice;
    private float timeToWait;
    private AudioSource outroSource;
    private AudioClip outroMusic;
    private Vector3 temp;
    private List<Button> teamsButton = new List<Button>();
    private IEnumerator coroutine;

    // Use this for initialization
    void Start()
    {
        GameObject.Find("ArrowButton").GetComponent<Button>().interactable = false;

        /*
         *  Initialisation of gameobjects and variables
         */
        timeToWait = 5f / (float)DataModel.BestScore();
        
        // vector calculated to make teams never go out of screen during DisplayScore
        float heightScale = 350f / (float)Screen.height;
        float scoreScale = 280f / (float)DataModel.BestScore();
        temp = new Vector3(0, scoreScale/heightScale, 0); 
        counter = 0;
        Sprite sprite = Resources.Load<Sprite>("Images/" + DataModel.BackgroundName);
        GameObject.Find("Background").GetComponent<Image>().sprite = sprite;
        
        teamsButton = GameObject.FindWithTag("teamcontainer").GetComponentsInChildren<Button>().OrderBy(go => go.name).ToList();

        for (int i = 0; i < teamsButton.Count; i++)
        {
            if (i < DataModel.NumberOfTeams)
            {
                teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
            }
            else
            {
                teamsButton[i].GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        indice = 0;
        for (int i = 0; i < DataModel.Scores.Length; i++)
        {
            if (DataModel.Scores[indice] < DataModel.Scores[i])
            {
                indice = i;
            }
        }

        List<int> premiers = new List<int>();

        for (int i = 0; i < DataModel.Scores.Length; i++)
        {
            if (DataModel.Scores[i] == DataModel.Scores[indice])
            {
                premiers.Add(i);
            }
        }

        string teamsThatWon = "";
        foreach ( int i in premiers)
        {
            if ( teamsThatWon.Any() )
            {
                teamsThatWon += " "+DataModel.TextToUse["and"]+" ";
            }
            teamsThatWon += TeamColor(i);
        }

        GameObject.Find("FinalText").GetComponent<TextMeshProUGUI>().text = teamsThatWon;
        if ( premiers.Count == 1)
        {
            GameObject.Find("FinalText").GetComponent<TextMeshProUGUI>().text += " "+DataModel.TextToUse["oneteam_won"];
        }
        else
        {
            GameObject.Find("FinalText").GetComponent<TextMeshProUGUI>().text += " " + DataModel.TextToUse["draw"];
        }

        counter = 0;
        foreach (Button b in teamsButton)
        {
            if (b.GetComponent<CanvasGroup>().alpha == 1)
            {
                coroutine = DisplayScore(counter);
                StartCoroutine(coroutine);
            }
            counter++;

        }

        /*
         *  OutroMusic setup
         */
        outroSource = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();
        outroMusic = (AudioClip)Resources.Load("Sounds/" + DataModel.OutroMusicName);
        outroSource.clip = outroMusic;
        outroSource.loop = true;
        outroSource.volume = 0.4f;
        outroSource.Play();

    }

    void Update()
    {

    }

    public void GoToMainMenu()
    {
        outroSource.Stop();
        SceneManager.LoadScene("EMenus");
    }

    /**
     * Tells which color is the team checking its index
     */
    private string TeamColor(int team)
    {
        switch (team)
        {
            case 0: return DataModel.TextToUse["red_team"];
            case 1: return DataModel.TextToUse["blue_team"];
            case 2: return DataModel.TextToUse["purple_team"];
            case 3: return DataModel.TextToUse["green_team"];
            case 4: return DataModel.TextToUse["orange_team"];
            case 5: return DataModel.TextToUse["cyan_team"];
            case 6: return DataModel.TextToUse["pink_team"];
            default: return DataModel.TextToUse["yellow_team"];
        }
    }

    /**
     * Make the teams go the upper the better their score is
     */
    private IEnumerator DisplayScore(int teamNumber)
    {
        for (int i = 0; i < DataModel.Scores[teamNumber]; i++)
        {
            teamsButton[teamNumber].transform.position += temp;
            yield return new WaitForSeconds(timeToWait);
        }
        if ( teamNumber == indice)
        {
            GameObject.Find("FinalText").GetComponent<CanvasGroup>().alpha = 1;
            GameObject.Find("ArrowButton").GetComponent<Button>().interactable = true;
            GameObject.Find("ArrowButton").GetComponent<CanvasGroup>().alpha = 1;
        }
    }
}
