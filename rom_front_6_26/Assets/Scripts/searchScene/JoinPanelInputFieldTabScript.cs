using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class JoinPanelInputFieldTabScript : MonoBehaviour {

    public Transform panel;
    public InputField i1;
    public InputField i2;
    public InputField i3;

    // Use this for initialization
    void Start () {
        i1 = panel.Find("joincenterpenal").Find("input_join_name").GetComponent<InputField>();
        i2 = panel.Find("joincenterpenal").Find("input_join_number").GetComponent<InputField>();
        i3 = panel.Find("joincenterpenal").Find("input_join_birth").GetComponent<InputField>();
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (EventSystem.current.currentSelectedGameObject == i1.gameObject)
                i2.Select();
            else if (EventSystem.current.currentSelectedGameObject == i2.gameObject)
                i3.Select();
            else if (EventSystem.current.currentSelectedGameObject == i3.gameObject)
                i1.Select();
        }
	}

    /*
    //회원가입(관리자 등록)의 유효성 검사.
    public bool validationCheck()
    {
        bool[] valid = { true, true, true, true };
        bool isOK = true;

        if (!(SignUpName.text.Length >= 2))
        {
            Debug.Log("네임 2");
            valid[0] = false;
            isOK = false;
        }
        if (!(SignUpID.text.Length >= 4))
        {
            Debug.Log("아이디 4");
            valid[1] = false;
            isOK = false;
        }
        if (!(SignUpPassword.text.Length >= 8))
        {
            Debug.Log("패스워드 8");
            valid[2] = false;
            isOK = false;
        }
        if (!Regex.IsMatch(SignUpEmail.text,
            "[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?"))
        {
            Debug.Log("이메일 문제");
            valid[3] = false;
            isOK = false;
        }

        if (!isOK)
        {
            StartCoroutine(BalloonOn(valid));
        }

        return isOK;
    }

    //말풍선을 띄웁니다.
    IEnumerator BalloonOn(bool[] valid)
    {

        for (int i = 0; i < valid.Length; i++)
        {
            if (!valid[i])
                Balloons[i].SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < valid.Length; i++)
        {
            if (!valid[i])
                Balloons[i].SetActive(false);
        }
    }
    */
}
