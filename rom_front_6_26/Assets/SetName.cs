using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SetName : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Text>().text = StaticLoginUser.theName;
	}
}
