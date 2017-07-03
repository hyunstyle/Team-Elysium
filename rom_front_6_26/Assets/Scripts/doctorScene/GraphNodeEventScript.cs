using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GraphNodeEventScript : MonoBehaviour {

    public Text tooltipText;

    public void graphhNodeIn()
    {
        tooltipText = GameObject.Find("TooltipText").GetComponent<Text>();
        tooltipText.text = gameObject.name;
        //Debug.Log(gameObject.name);
    }
	
    public void graphNodeOut()
    {
        tooltipText.text = "";
    }

}
