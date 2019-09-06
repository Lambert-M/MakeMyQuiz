using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using GracesGames.SimpleFileBrowser.Scripts;
using System.IO;
using System;
using UnityEngine.SceneManagement;

/**
 * @authors : Léo ROUZIC
 */
public class EditImageController : MonoBehaviour
{
    //VARIABLES
    public RectTransform imagePrefab;
    public GameObject confirmationBox;
    public GameObject yesBox;
    public GameObject noBox;
    public CanvasGroup canvas;
    public CanvasGroup canvasGroupConfirmationBox;

    private int nbQ;
    private string[] fileExtensions = { "png", "jpg" };
    private GameObject currentObj;
    private Image image;
    private string filename;
    private string pathDestImage;
    private string confirmation;
    private int nbDelete;

    // Use this for initialization
    void Start()
    {
        confirmation = "null";
        string[] datapath = Application.dataPath.Split('/');
        string pathsrc = datapath[0] + '/';
        for (int i = 1; i < datapath.Length - 1; i++)
        {
            pathsrc += '/' + datapath[i];
        }
        pathDestImage = pathsrc + '/' + "Images";
       
        GameObject.Find("MenuButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backmain"];
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        GameObject.Find("BackToTopics").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backtopic"];
        GameObject.Find("BackToTopics").GetComponent<Button>().onClick.AddListener(() => BackToTopicMenu());
        GameObject.Find("AddQuestion").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_addquestion"];
        GameObject.Find("AddQuestion").GetComponent<Button>().onClick.AddListener(() => NewQuestionPanelData());
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_images"];
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text += "\n " + DataModel.TextToUse["topic_name"] + " " + DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Name;
        yesBox.gameObject.GetComponent<Button>().onClick.AddListener(() => ClickYes());
        noBox.gameObject.GetComponent<Button>().onClick.AddListener(() => ClickNo());
        nbQ = 0;
        LoadAllQuestionsPanel();
    }

    // Update is called once per frame
    void Update()
    {
        Utilitary.NavigationUtility();
        GameObject.Find("NumberQuestions").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["question_number"] + nbQ;
    }

    /**
     * Create a new panel (dedicated to Image) in the UI for the scene EImages
     **/
    public void NewQuestionPanelLoad()
    {
        nbQ++;
        imagePrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/ImgSample"), GameObject.Find("ImgGrid").transform);
        imagePrefab.name = "ImgSample" + nbQ;
        imagePrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        imagePrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        imagePrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        imagePrefab.Find("ValidateQuestion").Find("ImportButton").GetComponent<Button>().onClick.AddListener(() => OpenImageFileBrowser());
        imagePrefab.Find("ValidateQuestion").Find("ImportButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["import_image"];
        imagePrefab.Find("ValidateQuestion").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        for (int i = 1; i < 5; i++)
        {
            imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
    }

    /**
     * @author Léo
     * 
     * Create in the UI a given number of panel corresponding to the number of questions in the current state of the DataModel
     * Used when starting the EImages scene to load all the previously entered datas
     **/
    public void LoadAllQuestionsPanel()
    {
        foreach (ImageQuestion qd in DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions)
        {
            NewQuestionPanelLoad();
            var x = new WWW("file:///" + pathDestImage + '/'+ qd.ImagePath);
            // waiting for the image to load entirely
            while (!x.isDone)
            {

            }
            imagePrefab.Find("Preview").GetComponent<Image>().sprite = Sprite.Create(x.texture, new Rect(0, 0, x.texture.width, x.texture.height), new Vector2(0, 0));
            imagePrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
            imagePrefab.GetComponent<PanelModel>().ImportDone = true;
            imagePrefab.GetComponent<PanelModel>().Filepath = qd.ImagePath;

            for (int i = 0; i < qd.Answers.Length; i++)
            {
                imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").gameObject.GetComponentsInChildren<TMP_InputField>()[i].text = qd.Answers[i].AnswerText;
                if (qd.Answers[i].IsTrue)
                {
                    imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>()[i].GetComponentInChildren<Toggle>().isOn = true;
                }
            }
        }
    }

    /**
     * Create a new question in the DataModel and fill it with default value
     **/
    public void NewQuestionPanelData()
    {
        nbQ++;
        imagePrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/ImgSample"), GameObject.Find("ImgGrid").transform);
        // set the new panel attributes to default values
        imagePrefab.name = "ImgSample" + nbQ;
        imagePrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
        imagePrefab.GetComponent<PanelModel>().ImportDone = false;
        // updating interface (language, events while clicking on button)
        imagePrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        imagePrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        imagePrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        imagePrefab.Find("ValidateQuestion").Find("ImportButton").GetComponent<Button>().onClick.AddListener(() => OpenImageFileBrowser());
        imagePrefab.Find("ValidateQuestion").Find("ImportButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["import_image"];
        imagePrefab.Find("ValidateQuestion").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        for (int i = 1; i < 5; i++)
        {
            imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
        //add a neutral question when user press the AddQuestion button and add it to the DataModel
        AnswerData[] ans = { new AnswerData("a", true), new AnswerData("b", false), new AnswerData("c", false), new AnswerData("d", false) };
        ImageQuestion question = new ImageQuestion("new question", ans);
        imagePrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A1Field").Find("TrueA1").GetComponent<Toggle>().isOn = true;
        DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Add(question);
    }



    /**
     * Remove the selected question from the DataModel and destroy the associated panel in the UI
     * Updating the number of question panel
     **/
    IEnumerator LaunchBoxDialog()
    {
        Debug.Log(confirmation);
        while (confirmation.Equals("null"))
        {
            Debug.Log(confirmation);
            yield return null;
        }
        if (confirmation.Equals("yes"))
        {
            Debug.Log("yes");

            DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Remove(DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[nbDelete - 1]);
            Destroy(GameObject.Find("ImgSample" + nbDelete));
            nbQ--;
            // edit the number associated to each ImgSample (avoid having two non-consecutive numbers)
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("ImgSample"))
            {
                if (e.GetComponent<PanelModel>().PanelNumber > nbDelete)
                {
                    e.GetComponent<PanelModel>().PanelNumber--;
                    e.name = "ImgSample" + e.GetComponent<PanelModel>().PanelNumber;
                }
            }
            confirmation = "null";
            confirmationBox.SetActive(false);

            canvas.interactable = true;
            canvas.blocksRaycasts = true;


            StopCoroutine("LaunchBoxDialog");
        }
        else if (confirmation.Equals("no"))
        {

            canvas.interactable = true;
            canvas.blocksRaycasts = true;


            Debug.Log("no");
            confirmationBox.SetActive(false);
            confirmation = "null";
            StopCoroutine("LaunchBoxDialog");
        }
    }

    /**
    * Set the confirmation string to yes or no depending of the button activated
    **/

    public void ClickYes()
    {
        confirmation = "yes";
    }
    public void ClickNo()
    {
        confirmation = "no";
    }


    public void RemoveQuestion()
    {
        nbDelete = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
        confirmationBox.SetActive(true);

        //reduce the visibility of normal UI, and disable all interraction
        canvas.interactable = false;
        canvas.blocksRaycasts = false;


        //enable interraction with confirmation gui and make visible
        canvasGroupConfirmationBox.interactable = true;
        canvasGroupConfirmationBox.blocksRaycasts = true;


        StartCoroutine("LaunchBoxDialog");
    }

    /**
     * Open the file browser to select a picture
     **/
    public void OpenImageFileBrowser()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().gameObject;
        GameObject fileBrowserObject = Instantiate((GameObject)Resources.Load("Prefabs/FileBrowser"), transform);
        fileBrowserObject.name = "FileBrowser";
        FileBrowser fileBrowserScript = fileBrowserObject.GetComponent<FileBrowser>();
        fileBrowserScript.SetupFileBrowser(ViewMode.Landscape);
        fileBrowserScript.OpenFilePanel(fileExtensions);
        GameObject.Find("SelectFileButton").GetComponent<Button>().onClick.AddListener(() => ChargeImage());
    }


    public void SaveQuestion()
    {
        currentObj = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().gameObject;
        if (currentObj.GetComponent<PanelModel>().ImportDone)
        {
            int QuestionNumber = currentObj.GetComponentInParent<PanelModel>().PanelNumber;
            TMP_InputField[] answersInput = EventSystem.current.currentSelectedGameObject.transform.Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>(); //get the answers
            Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
            List<AnswerData> answers = new List<AnswerData>();

            for (int i = 0; i < answersInput.Length; i++)
            {
                answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of an answer             
                answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));
            }

            ImageQuestion question = new ImageQuestion(currentObj.GetComponentInParent<PanelModel>().Filepath, answers.ToArray()); //create a new ImageQuestion
            DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question in the current topic in DataModel
        }
        else
        {
            StartCoroutine(ChangeColor());
        }
    }

    IEnumerator ChangeColor()
    {
        currentObj.transform.Find("ValidateQuestion").Find("ImportButton").GetComponent<Image>().color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(0.5f);
        currentObj.transform.Find("ValidateQuestion").Find("ImportButton").GetComponent<Image>().color = new Color(1, 1, 1, 1);
    }

    public void ChargeImage()
    {
        image = currentObj.transform.Find("Preview").GetComponent<Image>();
        string path = GameObject.Find("LoadFileText").GetComponent<Text>().text;
        path = path.Replace('\\', '/');
        string[] pathsplit = path.Split('/');
        string filename = pathsplit[pathsplit.Length - 1];

        if (!File.Exists(pathDestImage + '/' + filename))
        {
            File.Copy(path, pathDestImage + '/' + filename);
        }

        var x = new WWW("file:///" + pathDestImage + '/' + filename);
        //waiting for the picture to load entirely
        while (!x.isDone)
        {

        }
        image.sprite = Sprite.Create(x.texture, new Rect(0, 0, x.texture.width, x.texture.height), new Vector2(0, 0));
        currentObj.GetComponent<PanelModel>().Filepath = filename;
        currentObj.GetComponent<PanelModel>().ImportDone = true;

    }

    public bool GeneralSave()
    {
        bool allpassed = true;
        PanelModel[] questions = gameObject.GetComponentsInChildren<PanelModel>();
        foreach (PanelModel qi in questions)
        {
            if (qi.ImportDone)
            {
                int QuestionNumber = qi.PanelNumber;

                TMP_InputField[] answersInput = qi.transform.Find("ValidateQuestion").Find("QField").Find("AnswerField").GetComponentsInChildren<TMP_InputField>(); //get the answers
                Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
                List<AnswerData> answers = new List<AnswerData>();

                for (int i = 0; i < answersInput.Length; i++)
                {
                    answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of an answer           
                    answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));
                }

                ImageQuestion question = new ImageQuestion(qi.Filepath, answers.ToArray()); //create a new ImageQuestion
                question.ImagePath = qi.Filepath;
                DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question in the current topic in DataModel
            }
            else
            {
                allpassed = false;
                currentObj = qi.gameObject;
                StartCoroutine(ChangeColor());
                break;
            }
        }

        if (allpassed)
        {
            DataModel.Save(DataModel.CurrentFilename);
        }
        return allpassed;
    }

    public void BackToMainMenu()
    {
        if (GeneralSave())
        {
            SceneManager.LoadScene("EMenus");
        }
    }

    public void BackToTopicMenu()
    {
        if(GeneralSave())
        {
           
            SceneManager.LoadScene("ETopic");
        }
    }
}
