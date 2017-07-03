using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JsonFx.Json;

public class LoginScript : MonoBehaviour {

    public GameObject signUpPanel;

    public InputField LoginID;
    public InputField LoginPassword;

    public InputField SignUpName;
    public InputField SignUpID;
    public InputField SignUpPassword;
    public Toggle SignUpToggleDoc;
    public InputField SignUpEmail;

    public GameObject MessagePage;

    public Text msgtxt;

    public GameObject[] Balloons; //유효성 검사에 실패 했을 시 뜰 말풍선들입니다.

    //회원 가입 페이지를 엽니다.
    public void openSignUpPage()
    {
        signUpPanel.SetActive(true);
    }

    //회원 가입 페이지를 닫습니다.
    public void closeSignUpPage()
    {
        signUpPanel.SetActive(false);
    }

    public void openPage(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void closePage(GameObject obj)
    {
        obj.SetActive(false);
    }

    private void setTextMessage(string msg)
    {
        msgtxt.text = msg;
    }

    //각 InputField의 값들의 유효성을 검색하고 유효성이 만족되면 PushSignUp 코루틴을 실행합니다.
    public void requestSignUp()
    {
        WWWForm wwwF = new WWWForm();

        if(validationCheck())
        {
            wwwF.AddField("name", SignUpName.text);
            wwwF.AddField("userid", SignUpID.text);
            wwwF.AddField("password", SignUpPassword.text);
            wwwF.AddField("grade", (SignUpToggleDoc.isOn ? 1.ToString() : 2.ToString()));
            wwwF.AddField("email", SignUpEmail.text);

            WWW www = new WWW("http://igrus.mireene.com/rom_server/register.php", wwwF);

            LoginID.text = SignUpID.text;
            LoginPassword.text = SignUpPassword.text;

            StartCoroutine(pushSignUp(www));
        }
    }

    public IEnumerator pushSignUp(WWW www)
    {
        yield return www;

        closeSignUpPage();
        setTextMessage("성공적으로 등록이 되었습니다!");
        openPage(MessagePage);

        Dictionary<string, object> jDic;

        Debug.Log(www.text);

        jDic = (Dictionary<string, object>) JsonReader.Deserialize(www.text);

        Debug.Log(jDic);
    }

    //ID와 PW InputField의 값을 얻은 후 PushLogin 코루틴을 실행합니다.
    public void requestLogin()
    {
        WWWForm wwwF = new WWWForm();

        wwwF.AddField("userid", LoginID.text);
        wwwF.AddField("password", LoginPassword.text);


        Debug.Log(LoginID.text);
        Debug.Log(LoginPassword.text);

        WWW www = new WWW("http://igrus.mireene.com/rom_server/login.php", wwwF);

        StartCoroutine(pushLogin(www));
    }

    public IEnumerator pushLogin(WWW www)
    {
        yield return www;

        Dictionary<string, object> jDic;

        if(www.text.Contains("wrong"))
        {
            setTextMessage("잘못된 아이디 또는 비밀번호를 입력하셨습니다!");
        }
        else if (www.text.Contains("missing"))
        {
            setTextMessage("전송에 예기치 못한 값이 입력 되었습니다. 입력창을 지우신후 다시 입력해주세요.");
        }
        else if (www.text.Equals(""))
        {
            setTextMessage("로그인에 실패했습니다. 인터넷 연결을 확인해 주세요.");
        }
        else
        {
            jDic = (Dictionary<string, object>)JsonReader.Deserialize(www.text);

            StaticLoginUser.UID = (string)jDic["uid"];
            StaticLoginUser.theName = (string)jDic["name"];
            StaticLoginUser.grade = (int)jDic["grade"];
            StaticLoginUser.userid = (string)jDic["userid"];
            StaticLoginUser.email = (string)jDic["email"];
           

            Debug.Log(jDic["grade"].GetType());

            if (jDic["grade"].Equals(1))
            {
                setTextMessage("<b>의사</b> 로 접속합니다.");
                openPage(MessagePage);
                SceneManager.LoadScene("doctorScene");

            }
            else if (jDic["grade"].Equals(2))
            {
                setTextMessage("<b>프론트</b> 로 접속합니다.");
                openPage(MessagePage);
                SceneManager.LoadScene("searchScene");
            }
        }
        Debug.Log(www.text);

        openPage(MessagePage);
    }

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

        for(int i = 0; i < valid.Length; i++)
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
}
