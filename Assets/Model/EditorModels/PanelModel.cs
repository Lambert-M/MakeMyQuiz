using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelModel : MonoBehaviour {

    public int PanelNumber { get; set; }
    public string Filepath { get; set; }
    public float StartTime { get; set; }
    public float Volume { get; set; }
   
    public bool ImportDone { get; set; }

}
