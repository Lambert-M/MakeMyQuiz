using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;

/**
 * @author David
 **/
public class Timer : MonoBehaviour
{
    public float timerValue;

    protected float currTimerValue;
    private float nextPanelErased; // when i will need to erase the next timer panel
    public bool tickingDown;

    protected GameObject timer;
    public Image timer_image;
    public Image timer_bar;

    private List<GameObject> panellist = new List<GameObject>(15);
    
    void Start()
    {
        // initialisation of the panel timer
        if (SceneManager.GetActiveScene().name != "Introduction")
        {
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("realtimer"))
            {
                panellist.Add(e);
            }
            panellist = panellist.OrderBy(go => go.name).ToList();
            nextPanelErased = 14.0f;
        }

        // initialisation of the hidden timer
        timer = GameObject.Find("Timer");
        timer_image = timer.GetComponent<Image>();

        // Set the timer fillAmount to the maximum
        timer_image.fillAmount = 1.0f;

        // Enable the countdown
        currTimerValue = timerValue;

    }

    // Update is called once per frame
    void Update()
    {
        if (currTimerValue < 0)
        {
            tickingDown = false;
        }
        if (tickingDown)
        {
            // Subtrack the time since the start of the frame to currTimerValue
            currTimerValue -= Time.deltaTime;
            // Updating timer fillAmount
            timer_image.fillAmount -= 1.0f / timerValue * Time.deltaTime;
            timer_bar.fillAmount -= 1.0f / timerValue * Time.deltaTime;

            // erase the timer panel
            if (SceneManager.GetActiveScene().name != "Introduction" && SceneManager.GetActiveScene().name != "TrueFalse")
            {
                if (currTimerValue < nextPanelErased)
                {
                    panellist.ElementAt((int)nextPanelErased).GetComponent<CanvasGroup>().alpha = 0;
                    nextPanelErased -= 1.0f;
                }
            }
        }
    }

    public void ResetTimer()
    {
        tickingDown = false;

        currTimerValue = timerValue;
        // Set the timer fillAmount to the maximum
        timer_image.fillAmount = 1.0f;
        timer_bar.fillAmount = 1.0f;

        nextPanelErased = 14.0f;
    }

    public float GetCurrentTimeValue()
    {
        return currTimerValue;
    }
}
