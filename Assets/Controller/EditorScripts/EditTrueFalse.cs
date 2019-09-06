using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class EditTrueFalse : MonoBehaviour
{
    //VARIABLES
    private RectTransform questionPrefab;
    public GameObject confirmationBox;
    public GameObject yesBox;
    public GameObject noBox;
    public CanvasGroup canvas;
    public CanvasGroup canvasGroupConfirmationBox;

    private int nbQ;
    private string confirmation;
    private int nbDelete;

    // Use this for initialization
    void Start()
    {
        confirmation = "null";
        GameObject.Find("MenuButton").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backmain"];
        GameObject.Find("MenuButton").GetComponent<Button>().onClick.AddListener(() => BackToMainMenu());
        GameObject.Find("BackToTopics").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_backtopic"];
        GameObject.Find("BackToTopics").GetComponent<Button>().onClick.AddListener(() => BackToTopicMenu());
        GameObject.Find("AddQuestion").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_addquestion"];
        GameObject.Find("AddQuestion").GetComponent<Button>().onClick.AddListener(() => NewQuestionPanelData());
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_questions"];
        GameObject.Find("Title").GetComponent<TextMeshProUGUI>().text += "\n " + DataModel.TextToUse["topic_name"] + " " + DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Name;
        yesBox.gameObject.GetComponent<Button>().onClick.AddListener(() => ClickYes());
        noBox.gameObject.GetComponent<Button>().onClick.AddListener(() => ClickNo());
        nbQ = 0;
        if (DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions != null)
        {
            LoadAllQuestionsPanel();
        }

    }

    // Update is called once per frame
    void Update()
    {
        Utilitary.NavigationUtility();
        GameObject.Find("NumberQuestions").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["question_number"] + nbQ;
    }

    /**
     * Create a new panel (dedicated to TextQuestion) in the UI for the scene EQuestions
     **/
    public void NewQuestionPanelLoad()
    {
        nbQ++;
        questionPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/TFSample"), GameObject.Find("QGrid").transform);
        questionPrefab.name = "TFSample" + nbQ;
        questionPrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
        questionPrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        questionPrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        questionPrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        questionPrefab.Find("ValidateQuestion").Find("Validate").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        questionPrefab.Find("ValidateQuestion").Find("QField").Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["enter_questiontext"];
        for (int i = 1; i < 3; i++)
        {
            questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
    }

    /**
     * Create a new question in the DataModel and fill it with default value
     **/
    public void NewQuestionPanelData()
    {
        nbQ++;
        questionPrefab = Instantiate(Resources.Load<RectTransform>("Prefabs/TFSample"), GameObject.Find("QGrid").transform);
        questionPrefab.name = "TFSample" + nbQ;
        questionPrefab.GetComponent<PanelModel>().PanelNumber = nbQ;
        questionPrefab.Find("Delete").GetComponent<Button>().onClick.AddListener(() => RemoveQuestion());
        questionPrefab.Find("Delete").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["menu_remove"];
        questionPrefab.Find("ValidateQuestion").GetComponent<Button>().onClick.AddListener(() => SaveQuestion());
        questionPrefab.Find("ValidateQuestion").Find("Validate").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_validate"];
        questionPrefab.Find("ValidateQuestion").Find("QField").Find("Text Area").Find("Placeholder").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["enter_questiontext"];
        for (int i = 1; i < 3; i++)
        {
            questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("Text Area").GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["enter_answertext"];
            questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + i + "Field").Find("TrueA" + i).GetComponentInChildren<TextMeshProUGUI>().text = DataModel.TextToUse["correct_answer"];
        }
        //add a neutral question when user press the AddQuestion button and add it to the DataModel
        AnswerData[] ans = { new AnswerData("a", false), new AnswerData("b", false) };
        TrueFalseQuestion question = new TrueFalseQuestion("question", ans);
        DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Add(question);
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

    /**
     * Remove the selected question from the DataModel and destroy the associated panel in the UI
     * Updating the number of question panel
     **/

    IEnumerator LaunchBoxDialog()
    {
        while (confirmation.Equals("null"))
        {
            yield return null;
        }
        if (confirmation.Equals("yes"))
        {
            DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions.Remove(DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[nbDelete - 1]);
            Destroy(GameObject.Find("TFSample" + nbDelete));
            nbQ--;
            foreach (GameObject e in GameObject.FindGameObjectsWithTag("QSample"))
            {
                if (e.GetComponent<PanelModel>().PanelNumber > nbDelete)
                {
                    e.GetComponent<PanelModel>().PanelNumber--;
                    e.name = "TFSample" + e.GetComponent<PanelModel>().PanelNumber;
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

            confirmationBox.SetActive(false);
            confirmation = "null";
            StopCoroutine("LaunchBoxDialog");
        }
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
     * @author Léo
     * 
     * Create in the UI a given number of panel corresponding to the number of questions in the current state of the DataModel
     * Used when starting the EBlindtest scene to load all the previously entered datas
     **/
    public void LoadAllQuestionsPanel()
    {
        foreach (TrueFalseQuestion qd in DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions)
        {
            NewQuestionPanelLoad();
            questionPrefab.gameObject.GetComponentInChildren<TMP_InputField>().text = qd.Question;

            for (int i = 0; i < qd.Answers.Length; i++)
            {
                questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A"+(i+1)+"Field").gameObject.GetComponent<TMP_InputField>().text = qd.Answers[i].AnswerText;
                if (qd.Answers[i].IsTrue)
                {
                    questionPrefab.Find("ValidateQuestion").Find("QField").Find("AnswerField").Find("A" + (i + 1) + "Field").gameObject.GetComponent<TMP_InputField>().GetComponentInChildren<Toggle>().isOn = true;
                }
            }
        }
    }

    public void GeneralSave()
    {
        PanelModel[] questions = gameObject.GetComponentsInChildren<PanelModel>();
        foreach (PanelModel qi in questions)
        {
            int QuestionNumber = qi.GetComponent<PanelModel>().PanelNumber;
            TMP_InputField questionInput = qi.GetComponentInChildren<TMP_InputField>(); //get the question
            TMP_InputField[] answersInput = questionInput.GetComponentsInChildren<TMP_InputField>(); //get the answers
            Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
            List<AnswerData> answers = new List<AnswerData>();

            for (int i = 1; i < answersInput.Length; i++)
            {
                answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of the answer             
                answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));
            }

            TrueFalseQuestion question = new TrueFalseQuestion(questionInput.text, answers.ToArray()); //create a new TextQuestion

            DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question to the current topic in the DataModel
        }

        DataModel.Save(DataModel.CurrentFilename);
    }

    public void SaveQuestion()
    {

        int QuestionNumber = EventSystem.current.currentSelectedGameObject.GetComponentInParent<PanelModel>().PanelNumber;
        TMP_InputField questionInput = EventSystem.current.currentSelectedGameObject.GetComponentInChildren<TMP_InputField>(); //get the question
        TMP_InputField[] answersInput = questionInput.GetComponentsInChildren<TMP_InputField>(); //get the answers
        Toggle[] answersIsCorrect = new Toggle[answersInput.Length];
        List<AnswerData> answers = new List<AnswerData>();

        for (int i = 1; i < answersInput.Length; i++)
        {
            answersIsCorrect[i] = answersInput[i].GetComponentInChildren<Toggle>(); //get the correct statement of the answer              
            answers.Add(new AnswerData(answersInput[i].text, answersIsCorrect[i].isOn));

        }

        TrueFalseQuestion question = new TrueFalseQuestion(questionInput.text, answers.ToArray()); //create a new TextQuestion
        DataModel.Rounds[DataModel.IroundCur].Topics[DataModel.ItopicCur].Questions[QuestionNumber - 1] = question;//add the question to the current topic in the DataModel
    }

    public void BackToMainMenu()
    {
        GeneralSave();
        SceneManager.LoadScene("EMenus");
    }

    public void BackToTopicMenu()
    {
        GeneralSave();
        SceneManager.LoadScene("ETopic");
    }
}
