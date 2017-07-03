using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SignUpInputFieldTabScript : MonoBehaviour {

    public Transform panel;
    public InputField i1;
    public InputField i2;
    public InputField i3;
    public InputField i4;

    // Use this for initialization
    void Start()
    {
        i1 = panel.Find("input_sign_name").GetComponent<InputField>();
        i2 = panel.Find("input_sign_id").GetComponent<InputField>();
        i3 = panel.Find("input_sign_password").GetComponent<InputField>();
        i4 = panel.Find("input_sign_mail").GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject == i1.gameObject)
                i2.Select();
            else if (EventSystem.current.currentSelectedGameObject == i2.gameObject)
                i3.Select();
            else if (EventSystem.current.currentSelectedGameObject == i3.gameObject)
                i4.Select();
            else if (EventSystem.current.currentSelectedGameObject == i4.gameObject)
                i1.Select();
        }
    }
}
