using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;

public class LanguageController : MonoBehaviour
{ 
    public void EnglishText()
    {
        GameObject.Find("ChoseText").GetComponent<TextMeshProUGUI>().text = "Select your language";
    }

    public void FrenchText()
    {
        GameObject.Find("ChoseText").GetComponent<TextMeshProUGUI>().text = "Choisissez votre langue";
    }

    public void LoadLocalizedText(string fileName)
    {
        DataModel.TextToUse = new Dictionary<string, string>();
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);

        if (File.Exists(filePath))
        {
            string dataAsJson = File.ReadAllText(filePath);
            LocalizationData loadedData = JsonUtility.FromJson< LocalizationData>(dataAsJson);

            for (int i = 0; i < loadedData.items.Length; i++) 
            {
                DataModel.TextToUse.Add(loadedData.items[i].key, loadedData.items[i].value);
            }
            SceneManager.LoadScene("Welcome");
        }
    }
}