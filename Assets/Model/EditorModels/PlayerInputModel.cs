using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * @author Léo
 * */

public class PlayerInputModel : MonoBehaviour
{
    public int teamnumber;
    private Image imagebutton;
    private float time;
    private bool buttonpressed;
    private bool timerfinish;
    // Use this for initialization
    void Start()
    {
        buttonpressed = false;
        timerfinish = false;
        time = 0f;
        imagebutton = GameObject.Find("Team" + teamnumber).transform.Find("ButtonDisplay").GetComponentInChildren<Image>();
        if (teamnumber > DataModel.NumberOfTeams)
        {
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (!buttonpressed)
        {
            if (Input.GetButtonDown("team" + teamnumber + "A"))
            {
                imagebutton.color = new Color(0, 0, 1, 1);
                buttonpressed = true;
            }
            else if (Input.GetButtonDown("team" + teamnumber + "B"))
            {
                imagebutton.color = new Color(1, 0.56f, 0, 1);
                buttonpressed = true;
            }
            else if (Input.GetButtonDown("team" + teamnumber + "C"))
            {
                imagebutton.color = new Color(0.017f, 1, 0, 1);
                buttonpressed = true;
            }
            else if (Input.GetButtonDown("team" + teamnumber + "D"))
            {
                imagebutton.color = new Color(1, 1, 0, 1);
                buttonpressed = true;
            }
            else if (Input.GetButtonDown("team" + teamnumber + "buzz"))
            {
                switch (teamnumber)
                {
                    case 1:
                        imagebutton.color = new Color(1, 0, 0, 1);
                        break;
                    case 2:
                        imagebutton.color = new Color(0, 0, 1, 1);
                        break;
                    case 3:
                        imagebutton.color = new Color(0.78f, 0, 1, 1);
                        break;
                    case 4:
                        imagebutton.color = new Color(0.017f, 1, 0, 1);
                        break;
                    case 5:
                        imagebutton.color = new Color(1, 0.56f, 0, 1);
                        break;
                    case 6:
                        imagebutton.color = new Color(0, 0.85f, 1, 1);
                        break;
                    case 7:
                        imagebutton.color = new Color(1, 0, 0.68f, 1);
                        break;
                    case 8:
                        imagebutton.color = new Color(1, 1, 0, 1);
                        break;
                }
                buttonpressed = true;
            }
        }
        else if (!timerfinish && buttonpressed)
        {       
            
                if (time > 0.4f)
                {
                    timerfinish = true;
                }        
            time += Time.deltaTime;               
        }
        else if (timerfinish)
        {
            buttonpressed = false;
            timerfinish = false;
            time = 0f;
            imagebutton.color = new Color(1, 1, 1, 1);
        }
    }
    public void QuitInput()
    {
        SceneManager.LoadScene("EMenus");
    }
}
