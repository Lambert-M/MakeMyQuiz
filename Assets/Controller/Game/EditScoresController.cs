using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class EditScoresController : MonoBehaviour {

    private List<Button> teamsButton = new List<Button>();
    private GameObject team;
    private GameObject teamContainer1;
    private GameObject teamContainer2;
    private GameObject arrow;
    private List<PlayerModel> teamsCtrl = new List<PlayerModel>();
    private Color c;
    // Use this for initialization
    void Start () {
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
            team.GetComponentInChildren<PlayerModel>().teamnumber = (i + 1);
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
         
                if (x.name.Contains("Team"))
                {
                    x.color = c;
                }
            }
        }

        }
	
	// Update is called once per frame
	void Update () {
        Event e = Event.current;
        for (int i = 0; i < DataModel.NumberOfTeams; i++)
        {
            teamsButton[i].GetComponentInChildren<TextMeshProUGUI>().text = DataModel.GetTextScoreFromTeam(i);
        }

        if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            if (DataModel.NumberOfTeams >= 1)
            {
                DataModel.Scores[0] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            if (DataModel.NumberOfTeams >= 2)
            {
                DataModel.Scores[1] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            if (DataModel.NumberOfTeams >= 3)
            {
                DataModel.Scores[2] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)))
        {
            if (DataModel.NumberOfTeams >= 4)
            {
                DataModel.Scores[3] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)))
        {
            if (DataModel.NumberOfTeams >= 5)
            {
                DataModel.Scores[4] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)))
        {
            if (DataModel.NumberOfTeams >= 6)
            {
                DataModel.Scores[5] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)))
        {
            if (DataModel.NumberOfTeams >= 7)
            {
                DataModel.Scores[6] += 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) && (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)))
        {
            if (DataModel.NumberOfTeams >= 8)
            {
                DataModel.Scores[7] += 1;
            }
        }

        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1)))
        {
            if (DataModel.NumberOfTeams >= 1)
            {
                DataModel.Scores[0] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2)))
        {
            if (DataModel.NumberOfTeams >= 2)
            {
                DataModel.Scores[1] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3)))
        {
            if (DataModel.NumberOfTeams >= 3)
            {
                DataModel.Scores[2] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4)))
        {
            if (DataModel.NumberOfTeams >= 4)
            {
                DataModel.Scores[3] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad5)))
        {
            if (DataModel.NumberOfTeams >= 5)
            {
                DataModel.Scores[4] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha6) || Input.GetKeyDown(KeyCode.Keypad6)))
        {
            if (DataModel.NumberOfTeams >= 6)
            {
                DataModel.Scores[5] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha7) || Input.GetKeyDown(KeyCode.Keypad7)))
        {
            if (DataModel.NumberOfTeams >= 7)
            {
                DataModel.Scores[6] -= 1;
            }
        }
        else if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && (Input.GetKeyDown(KeyCode.Alpha8) || Input.GetKeyDown(KeyCode.Keypad8)))
        {
            if (DataModel.NumberOfTeams >= 8)
            {
                DataModel.Scores[7] -= 1;
            }
        }

        else if (Input.GetKeyDown(DataModel.Next))
        {
            SceneManager.LoadScene("Ending");
        }
    }
}
