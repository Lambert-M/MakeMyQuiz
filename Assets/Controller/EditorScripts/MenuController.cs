
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using SimpleJSON;
using GracesGames.SimpleFileBrowser.Scripts;

/**
 * @author : Léo ROUZIC
 * */
public class MenuController : MonoBehaviour {

    //VARIABLES
    private RectTransform buttonPrefab;
    private string path;
    
    // Use this for initialization
    void Start()
    {
        DataModel.DestroyAllData();

        RectTransform menuButtonRef = Resources.Load<RectTransform>("Prefabs/MenuButton");
        GameObject.Find("Main panel").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["game_title"];
        //Play button
        buttonPrefab = Instantiate(menuButtonRef, GameObject.Find("Main Panel").transform);
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => PlayQuiz());
        buttonPrefab.GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_play"];

        //Create button
        buttonPrefab = Instantiate(menuButtonRef, GameObject.Find("Main Panel").transform);
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => CreateQuiz());
        buttonPrefab.GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_create"];

        //Edit button
        buttonPrefab = Instantiate(menuButtonRef, GameObject.Find("Main Panel").transform);
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => EditQuiz());
        buttonPrefab.GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_edit"];

        //Exit button
        buttonPrefab = Instantiate(menuButtonRef, GameObject.Find("Main Panel").transform);
        buttonPrefab.GetComponent<Button>().onClick.AddListener(() => CloseApplication());
        buttonPrefab.GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_quit"];
    }
    
    public void PlayQuiz()
    {
        string[] srcPath = Application.dataPath.Split('/');
        string finalPath = srcPath[0] + "/";
        for (int i = 1; i < srcPath.Length - 1; i++)
        {
            finalPath += '/' + srcPath[i];
        }

        string savePath = finalPath + "/Saves/";
        
        string[] ext = { "json" };
        GameObject fileBrowser = (GameObject)Instantiate(Resources.Load("Prefabs/FileBrowser"));
        fileBrowser.GetComponent<FileBrowser>().SetupFileBrowser(ViewMode.Landscape, savePath);
        fileBrowser.GetComponent<FileBrowser>().OpenFilePanel(ext);

        GameObject.Find("SelectFileButton").GetComponent<Button>().onClick.AddListener(() => SelectToPlay());
    }

    public void EditQuiz()
    {
        string[] srcPath = Application.dataPath.Split('/');
        string finalPath = srcPath[0] + "/";
        for (int i = 1; i < srcPath.Length - 1; i++)
        {
            finalPath += '/' + srcPath[i];
        }

        string savePath = finalPath + "/Saves/";

        string[] ext = { "json" };
        GameObject fileBrowser = (GameObject)Instantiate(Resources.Load("Prefabs/FileBrowser"));
        fileBrowser.GetComponent<FileBrowser>().SetupFileBrowser(ViewMode.Landscape,savePath);
        fileBrowser.GetComponent<FileBrowser>().OpenFilePanel(ext);
       
        GameObject.Find("SelectFileButton").GetComponent<Button>().onClick.AddListener(() => SelectToLoad());
    }

    public void CreateQuiz()
    {
        string[] srcPath = Application.dataPath.Split('/');
        string finalPath = srcPath[0] + "/";
        for (int i = 1; i < srcPath.Length - 1; i++)
        {
            finalPath += '/' + srcPath[i];
        }

        string savePath = finalPath + "/Saves/";
        
        DataModel.DestroyAllData();

        string[] ext = { "json" };
        GameObject fileBrowser = (GameObject)Instantiate(Resources.Load("Prefabs/FileBrowser"));
        fileBrowser.GetComponent<FileBrowser>().SetupFileBrowser(ViewMode.Landscape,savePath);
        fileBrowser.GetComponent<FileBrowser>().SaveFilePanel("MyQuiz", ext);

        GameObject.Find("SelectFileButton").GetComponent<Button>().onClick.AddListener(() => SelectToCreate());
    }
    

    public void CloseApplication()
    {
        Application.Quit();
    }

    void SelectToLoad()
    {
        string filename = GameObject.Find("LoadFileText").GetComponent<Text>().text;
        
        //Formatting path
        filename.Replace('\\', '/');
        DataModel.DestroyAllData();
        DataModel.CurrentFilename = filename;
        DataModel.CurrentRunningFilename = filename+"running";
        GameObject.Find("DataModel").GetComponent<DataModel>().Load(DataModel.CurrentFilename);

        SceneManager.LoadScene("ERounds");
    }

    void SelectToCreate()
    {
        string filename = GameObject.Find("SaveFileText").transform.Find("Text").GetComponent<Text>().text;
        DataModel.QuizName = filename.Split('/').Last().Split('.').First();

        string[] srcPath = Application.dataPath.Split('/');
        string finalPath = srcPath[0] + "/";
        for (int i = 1; i < srcPath.Length - 1; i++)
        {
            finalPath += '/' + srcPath[i];
        }
       
        string savePath = finalPath + "/Saves/" + filename;

        DataModel.CurrentFilename = savePath;
        if (!File.Exists(savePath))
        {
            FileStream x = File.Create(savePath);
            x.Close();
            DataModel.Save(DataModel.CurrentFilename);
            SceneManager.LoadScene("ERounds");
        }
        else
        {
            GameObject.Find("CloseFileBrowserButton").GetComponent<Button>().Select();
        }
        DataModel.CurrentFilename = savePath;
    }

    void SelectToPlay()
    {
        string filename = GameObject.Find("LoadFileText").GetComponent<Text>().text;

        //Formatting path
        filename.Replace('\\', '/');
        DataModel.DestroyAllData();
        DataModel.CurrentFilename = filename;
        DataModel.CurrentRunningFilename = filename.Substring(0, (filename.Length - 5)) + "running.json";
        GameObject.Find("DataModel").GetComponent<DataModel>().Load(DataModel.CurrentFilename);
        SceneManager.LoadScene("InputScene");
    }
}
