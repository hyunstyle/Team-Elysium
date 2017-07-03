using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartPenalInputFieldTabScript : MonoBehaviour
{
    public LoginScript sc;
    public Transform panel;
    public InputField i1;
    public InputField i2;

    // Use this for initialization
    void Start()
    {
        i1 = panel.Find("input_ID").GetComponent<InputField>();
        i2 = panel.Find("input_PW").GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject == i1.gameObject)
                i2.Select();
            else if (EventSystem.current.currentSelectedGameObject == i2.gameObject)
                i1.Select();
        }
        else if (Input.GetKeyDown("return"))
        {
            Debug.Log("Enter");
            sc.requestLogin();
        }
    }
}
