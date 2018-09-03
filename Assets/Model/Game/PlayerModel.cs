using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/**
 * @author Léo
 * */

public class PlayerModel : MonoBehaviour
{
    protected string answer;
    public bool buzzed;
    private bool hasAnswered;
    private int ans = -1;
    public int teamnumber;
    private int roundPoint;
    private Timer t;
    public TextMeshProUGUI answer1;
    public TextMeshProUGUI answer2;
    public TextMeshProUGUI answer3;
    public TextMeshProUGUI answer4;
    public GameObject answerPanel;

    // Use this for initialization
    void Start()
    {
        roundPoint = 0;
        answer = "";
        hasAnswered = false;
        buzzed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if ( SceneManager.GetActiveScene().name == "Questions" || SceneManager.GetActiveScene().name == "Images"  ) {
            
            t = GameObject.Find("Timer").GetComponent<Timer>();
            Image img = answerPanel.GetComponent<Image>();
            if(!hasAnswered){

                ans = -1;

            if (t.GetCurrentTimeValue() >= 12 && t.GetCurrentTimeValue() < 15)
                {
                    roundPoint = 4;
                }

            else if (t.GetCurrentTimeValue() >= 8 && t.GetCurrentTimeValue() < 12)
                {
                    roundPoint = 3;
                }

            else if (t.GetCurrentTimeValue() >= 4 && t.GetCurrentTimeValue() < 8)
                {
                    roundPoint = 2;
                }

            else if (t.GetCurrentTimeValue() >= 0 && t.GetCurrentTimeValue() < 4)
                {
                    roundPoint = 1;
                }

                if (Input.GetButtonDown("team" + teamnumber + "A") && GameObject.Find("Answer Panel 1").GetComponent<CanvasGroup>().alpha == 1)
                {
                    ans = 1;
                    img.color = new Color(0, 0, 0, 1);
                    answer = answer1.text;
                    hasAnswered = true;
                }
                else if (Input.GetButtonDown("team" + teamnumber + "B") && GameObject.Find("Answer Panel 2").GetComponent<CanvasGroup>().alpha == 1)
                {
                    ans = 2;
                    img.color = new Color(0, 0, 0, 1);
                    answer = answer2.text;
                    hasAnswered = true;
                }
                else if (Input.GetButtonDown("team" + teamnumber + "C") && GameObject.Find("Answer Panel 3").GetComponent<CanvasGroup>().alpha == 1)
                {
                    ans = 3;
                    img.color = new Color(0, 0, 0, 1);
                    hasAnswered = true;
                    answer = answer3.text;

                }
                else if (Input.GetButtonDown("team" + teamnumber + "D") && GameObject.Find("Answer Panel 4").GetComponent<CanvasGroup>().alpha == 1)
                {
                    ans = 4;
                    img.color = new Color(0, 0, 0, 1);
                    hasAnswered = true;
                    answer = answer4.text; ;
                }
                else if (Input.GetButtonDown("team" + teamnumber + "buzz"))
                {
                    buzzed = true;
                    hasAnswered = true;
                }
            }
        }
    }

    public string GetAnswer()
    {
        if (hasAnswered)
        {
            return answer;
        }
        return "";
    }

    public void ActivateJoker()
    {
        DataModel.Jokers[teamnumber - 1] = !DataModel.Jokers[teamnumber - 1];
    }

        public bool GetHasAnswered()
    {
      return hasAnswered;
    }

    public void SetHasAnswered (bool b)
    {
      hasAnswered = b;
    }

    public int GetNumberAnswer(){
      return ans;
    }
    
 
    public int GetCurrentRoundPoints()
    {
      return roundPoint;
    }
    

}
