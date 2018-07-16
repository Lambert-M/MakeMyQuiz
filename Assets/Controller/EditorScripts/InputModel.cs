using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputModel : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GameObject.Find("InputText").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_input"];
        GameObject.Find("Quit").GetComponent<TextMeshProUGUI>().text = DataModel.TextToUse["menu_quit"];
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
