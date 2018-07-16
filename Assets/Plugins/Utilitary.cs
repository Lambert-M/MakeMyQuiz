using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Utilitary : UnityEngine.EventSystems.UIBehaviour {
    public static void NavigationUtility()
    {
        bool atLeastOneSelected = false;
        if(EventSystem.current.currentSelectedGameObject != null)
        {
            atLeastOneSelected = true;
        }
        if(atLeastOneSelected && (Input.GetKey(KeyCode.LeftShift)||Input.GetKey(KeyCode.RightShift)) && Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable next = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnUp();

            if (next != null && next.GetComponent<InputField>() != null)
            {
                next.GetComponent<InputField>().Select();
            }
            else if(next != null && next.GetComponent<TMP_InputField>() != null)
            {
                next.GetComponent<TMP_InputField>().Select();
            }
        }
        else if (atLeastOneSelected && Input.GetKeyDown(KeyCode.Tab))
        {
            Selectable nextDown = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().FindSelectableOnDown();

            if(nextDown != null)
            {
                if (nextDown.GetComponent<InputField>() != null)
                {
                    nextDown.GetComponent<InputField>().Select();
                }
                else if (nextDown.GetComponent<TMP_InputField>() != null)
                {
                    nextDown.GetComponent<TMP_InputField>().Select();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("EMenus");
        }
    }
}
