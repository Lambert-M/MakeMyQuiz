using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using System.Linq;

public class WelcomeController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("game_title").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["game_title"];
        GameObject.Find("welcome").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["welcome"];
    }
	
	// Update is called once per frame
	void Update () {
		if ( Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.Mouse0) )
        {
            SceneManager.LoadScene("EMenus");
        }
	}
}
