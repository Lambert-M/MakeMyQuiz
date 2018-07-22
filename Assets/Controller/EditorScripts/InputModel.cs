using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

public class InputModel : MonoBehaviour {

    TMP_InputField teamNumber;
    private static string pattern;

    // Use this for initialization
    void Start () {

        GameObject.Find("InputText").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_input"];
        GameObject.Find("Nbteams").GetComponent<TMP_InputField>().gameObject.transform.Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["placeholder_input"];
        
        pattern = @"^\d$";
        teamNumber = GameObject.Find("Nbteams").GetComponent<TMP_InputField>();

        if (teamNumber)
        {
            teamNumber.onValidateInput = ValidateInput;
        }
    }

    static char ValidateInput(string text, int charIndex, char addedChar)
    {
        if (Regex.IsMatch(text + addedChar, pattern))
        {
            return addedChar;
        }
        else
        {
            return '\0';
        }
    }

    // Update is called once per frame
    void Update () {

        if (Regex.IsMatch(teamNumber.text, pattern) && int.Parse(teamNumber.text) < 9 && int.Parse(teamNumber.text) > 0 && (Input.GetKeyDown(KeyCode.Return | KeyCode.KeypadEnter)))
        {
            DataModel.NumberOfTeams = int.Parse(teamNumber.text);
            SceneManager.LoadScene("Introduction");
        }
    }
}
