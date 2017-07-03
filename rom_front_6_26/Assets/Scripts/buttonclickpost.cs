using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using CP.ProChart;
using System.Text.RegularExpressions;
using JsonFx.Json;
using System.Net;
using System.IO;
using System.Net.Sockets;
/*
    - 구조 -

    [변수]

        (생략)

    [함수 - 코루틴]

        patientlist() - WaitForRequestPatientlist(WWW) : 
        patientadd() - (BalloonOn(bool[]) && WaitForRequestpatientadd(WWW) : 
        checkdatelist() - WaitForRequestCheckdatelist(WWW) : 
        doctorpatientlist() - WaitForRequestDoctorPatientlist(WWW) : 
        doctorcheckdatelist() - WaitForRequestDoctorCheckdatelist(WWW) : 
        kinectadd() - WaitForRequestkinectadd(WWW, int) : 
        DoctorCheckTimeList() - WaitForRequestCheckTimeList(WWW) : 
        addclick() - listWaitForRequest(WWW) : 

    [함수]
        
        listclick()
        shutDown()

    [코루틴]

        end()

    [더미 코드]

        doctorcheckdatelistgraph()
*/

public class buttonclickpost : MonoBehaviour
{

    //****************************************************************
    //이하 [변수] 단
    //****************************************************************


    #region variable
    //각 WWW에서 보낼 URL 모음.
    string patientListURL = "http://igrus.mireene.com/rom_server/patientlist.php";
    string patientAddURL = "http://igrus.mireene.com/rom_server/patientadd.php";
    string checkDateListURL = "http://igrus.mireene.com/rom_server/checkdatelist.php";
    string checkTimeListURL = "http://igrus.mireene.com/rom_server/checktimelist.php";
    string fronttoKinectURL = "http://igrus.mireene.com/rom_server/fronttokinect.php";

    public GameObject arrayPatientListGameObject;
    public GameObject arrayCheckDateListGameObject;
    public GameObject arrayDoctorCheckDateListGameObject;

    //인스펙터상에서 링크해놓는 오브젝트들입니다.
    public GameObject patientListButton; //의사와 프론트 씬에서 좌측에 나오는 환자의 목록을 구성하는 버튼입니다.
    public GameObject docterPatientListButton; //
    public GameObject checkDateListPenal; //
    public GameObject doctorCheckDateListButton; //

    //그래프 관련 변수들
    public GameObject graphPanel; //그래프를 보여줄 패널의 레퍼런스입니다.
    public GameObject textRect; //그래프에서 날짜를 표시해주는 텍스트입니다. 인스턴스로 존재해 씬 내부에 있습니다.
    public RectTransform tooltip; //그래프 패널 내부의 노드에 커서를 호버링하면 보여줄 정보를 담는 툴팁 상자입니다.
    public Text tooltipText; //바로 위 tooltip 내부의 텍스트입니다.
    public Dropdown dropDown; //그래프f 패널 내부에 존재하는 드롭다운입니다.

    private int[] DDValueToJD;
    public ScrollRect scrollRect;
    public RectTransform contentRect;
    public GameObject dot;
    public GameObject date;
    public Text angle;

    private flowChart floawchart;

    public static List<string> dateSet;
    //이상 그래프 관련 변수들

    //클릭한 객체를 알려주는 인디케이터, 이벤트시스템 대용쯤
    static public string patientid = "0";
    static public string checkdateid = "0";
    static public string[] patientInfo = { "", "", "", "" };
    //17-03-29 이미지 복원시 필요한 이름
    static public string imagename = "";

    //@TODO : 설명
    public class checktime
    {
        public string checktimeid = "0";
        public int framecount = 0;
        public string jsons = "";
    }
    public static List<checktime> checktimearr;

    //@TODO : 설명 
    private int overRow = -1;
    private int overColumn = -1;

    //프론트 씬에서 환자 검진 등록 시 각 Input Field의 타당성 체크를 표시해줄 말풍선입니다
    public Transform[] balloons = new Transform[3];
    #endregion


    //****************************************************************
    //이하 [함수 - 코루틴] 단
    //****************************************************************


    #region function-coroutine

    #region patientlist
    public void patientlist() //닥터와 프론트 씬에서 환자 목록들을 가져오는 함수-코루틴입니다. 두 씬 다 왼쪽 패널에 불러옵니다.
    {
        StartCoroutine(WaitForRequestPatientlist(new WWW(patientListURL)));
    }
    private IEnumerator WaitForRequestPatientlist(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            arrayPatientListGameObject = GameObject.Find("arraypatientlist");

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            int childs = arrayPatientListGameObject.transform.childCount;

            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayPatientListGameObject.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < dicarr.Length; i++)
            {
                GameObject tmpplb = Instantiate(patientListButton);

                Text text_name = tmpplb.transform.Find("text_patientlist_name").GetComponent<Text>();
                Text text_number = tmpplb.transform.Find("text_patientlist_number").GetComponent<Text>();
                Text text_sex = tmpplb.transform.Find("text_patientlist_sexstring").GetComponent<Text>();
                Text text_birth = tmpplb.transform.Find("text_patientlist_birth").GetComponent<Text>();

                text_name.text = dicarr[i]["name"] + "\n";
                if (dicarr[i]["sex"] == null || dicarr[i]["sex"].ToString().Trim().Equals("")) Debug.Log("No sex value");
                else text_sex.text = dicarr[i]["sex"] + "\n";
                if (dicarr[i]["birth"] == null || dicarr[i]["birth"].ToString().Trim().Equals("")) Debug.Log("No birth value");
                else text_birth.text = dicarr[i]["birth"] + "\n";
                if (dicarr[i]["number"] == null || dicarr[i]["number"].ToString().Trim().Equals("")) Debug.Log("No number value");
                else text_number.text = dicarr[i]["number"] + "\n";
                if (dicarr[i]["patientid"] == null || dicarr[i]["patientid"].ToString().Trim().Equals("")) Debug.Log("No patientid value");
                else tmpplb.name = "" + dicarr[i]["patientid"];

                tmpplb.transform.parent = arrayPatientListGameObject.transform;
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    #endregion

    #region patientadd
    public void patientadd() //한상우 : SearchScene에서 초진 환자를 등록할 때 사용
    {
        GameObject joincenterpenal = GameObject.Find("joincenterpenal");

        InputField input_name = joincenterpenal.transform.Find("input_join_name").GetComponent<InputField>();
        InputField input_number = joincenterpenal.transform.Find("input_join_number").GetComponent<InputField>();

        Toggle toggle_man = joincenterpenal.transform.Find("ToggleGroup").Find("ToggleMan").GetComponent<Toggle>();

        InputField input_birth = joincenterpenal.transform.Find("input_join_birth").GetComponent<InputField>();

        bool[] valid = { true, true, true };
        bool isOK = true; //서버에 보내도 될 만한 올바른 자료인가?

        //한상우 : validation check - 이름 공백 || 토글 공백 || 생년월일 타당성 불효시 보내지 않음

        if (input_name.text.Equals(""))
        {
            Debug.Log("이름이 공백");
            valid[0] = false;
            isOK = false;
        }
        //각각 확인 해야 하므로 if가 아닌 else if
        if (input_number.text.Equals(""))
        {
            valid[1] = false;
            isOK = false;
        }
        if (input_birth.text.Equals("") || (
            !Regex.IsMatch(input_birth.text, @"^(19|20)\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[0-1])$") &&
            !Regex.IsMatch(input_birth.text, @"^\d{2}(0[1-9]|1[012])(0[1-9]|[12][0-9]|3[0-1])$")))
        {
            Debug.Log("정확한 생년월일을 입력해주세요");
            valid[2] = false;
            isOK = false;
        }

        if (isOK) //올바른 형식의 자료일 시에는 서버에 요청하는 코루틴을 실행
        {
            Dictionary<string, string> di = new Dictionary<string, string>();

            di.Add("name", input_name.text);
            if (toggle_man.isOn) di.Add("sex", "1");
            else di.Add("sex", "2");
            di.Add("number", input_number.text);
            di.Add("birth", input_birth.text);
            WWWForm form = new WWWForm();
            foreach (KeyValuePair<String, String> post_arg in di)
            {
                form.AddField(post_arg.Key, post_arg.Value);
            }
            WWW www = new WWW(patientAddURL, form);
            StartCoroutine(WaitForRequestpatientadd(www));
        }
        else //아닐경우 말풍선 띄움
        {
            StartCoroutine(BalloonOn(valid));

        }
    }
    //말풍선을 띄웁니다.
    IEnumerator BalloonOn(bool[] valid)
    {

        for (int i = 0; i < valid.Length; i++)
        {
            if (!valid[i])
                balloons[i].gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < valid.Length; i++)
        {
            if (!valid[i])
                balloons[i].gameObject.SetActive(false);
        }
    }
    private IEnumerator WaitForRequestpatientadd(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log(www.text);
            patientlist();
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

    }
    #endregion

    #region checkdatelist
    public void checkdatelist()
    {
        patientid = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log("Event Message : " + patientid);

        GameObject obj = EventSystem.current.currentSelectedGameObject;

        patientInfo[0] = obj.transform.Find("text_patientlist_name").GetComponent<Text>().text;
        patientInfo[1] = obj.transform.Find("text_patientlist_number").GetComponent<Text>().text;
        patientInfo[2] = obj.transform.Find("text_patientlist_sexstring").GetComponent<Text>().text;
        patientInfo[3] = obj.transform.Find("text_patientlist_birth").GetComponent<Text>().text;

        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("patientid", patientid);

        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(checkDateListURL, form);
        StartCoroutine(WaitForRequestCheckdatelist(www));
    }
    private IEnumerator WaitForRequestCheckdatelist(WWW www)
    {
        yield return www;

        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("arraycheckdatelistgameobject");

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            Debug.LogError(www.text);
        }
        // check for errors
        else if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            arrayCheckDateListGameObject = GameObject.Find("arraycheckdatelistgameobject");

            Dictionary<string, object>[] dicarr = null;

            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }


            for (int i = 0; i < dicarr.Length; i++)
            {
                GameObject cdlp = Instantiate(checkDateListPenal);

                Text text_resume = cdlp.transform.Find("text_resume").GetComponent<Text>();
                text_resume.text = dicarr[i]["datetime"] + "\n";
                text_resume.text += "부위 : " + dicarr[i]["jointdirection"];
                text_resume.text += "각도 : " + dicarr[i]["maxangle"];
                cdlp.name = "" + dicarr[i]["checkdateid"];

                cdlp.transform.parent = arrayCheckDateListGameObject.transform;


            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    #endregion

    #region doctorpatientlist
    public void doctorpatientlist()
    {
        StartCoroutine(WaitForRequestDoctorPatientlist(new WWW(patientListURL)));
    }
    private IEnumerator WaitForRequestDoctorPatientlist(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            arrayPatientListGameObject = GameObject.Find("arraydoctorpatientlist");

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            int childs = arrayPatientListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayPatientListGameObject.transform.GetChild(i).gameObject);
            }


            for (int i = 0; i < dicarr.Length; i++)
            {
                GameObject tmpdplb = Instantiate(docterPatientListButton);

                Text text_name = tmpdplb.transform.Find("text_patientlist_name").GetComponent<Text>();
                Text text_number = tmpdplb.transform.Find("text_patientlist_number").GetComponent<Text>();
                Text text_sex = tmpdplb.transform.Find("text_patientlist_sexstring").GetComponent<Text>();
                Text text_birth = tmpdplb.transform.Find("text_patientlist_birth").GetComponent<Text>();
                text_name.text = dicarr[i]["name"] + "\n";
                if (int.Parse(dicarr[i]["sex"].ToString()) == 1) text_sex.text = "남";
                else text_sex.text = "여";


                text_birth.text = dicarr[i]["birth"] + "\n";
                text_number.text = dicarr[i]["number"] + "\n";
                tmpdplb.name = "" + dicarr[i]["patientid"];
                tmpdplb.transform.SetParent(arrayPatientListGameObject.transform);
            }


        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    #endregion

    #region doctorcheckdatelist
    //닥터 씬에서, 왼쪽의 목록중 하나의 버튼을 누르면 실행
    public void doctorcheckdatelist()
    {
        //이벤트 시스템을 이용하여 왼쪽의 현재 선택된 버튼을 반환
        patientid = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(patientid);

        //딕셔너리 생성과 키 값 삽입 : 이 딕셔너리는 정보를 서버로 전달하기 위해 사용됩니다
        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("patientid", patientid);

        //Form 생성후 키 값 삽입
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(checkDateListURL, form);

        //WaitForRequestDoctorCheckdatelist 코루틴 실행
        StartCoroutine(WaitForRequestDoctorCheckdatelist(www));
    }
    private IEnumerator WaitForRequestDoctorCheckdatelist(WWW www)
    {
        yield return www;

        //반환값이 비었음을 의미하므로 기존의 오른쪽 창의 항목들의 삭제만 진행해줌.
        //@TODO : 어차피 삭제는 중복된 코드인데 줄일 수 없나?
        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("arraydoctorcheckdatelistgameobject");

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            Debug.LogError(www.text);
        }
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            arrayCheckDateListGameObject = GameObject.Find("arraydoctorcheckdatelistgameobject");

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < dicarr.Length; i++)
            {
                GameObject cdlp = Instantiate(doctorCheckDateListButton);
                Text text_doctorcheckdate_datetime = cdlp.transform.Find("text_doctorcheckdate_datetime").GetComponent<Text>();
                text_doctorcheckdate_datetime.text = dicarr[i]["datetime"] + "\n";

                Text text_doctorcheckdate_jointdirection = cdlp.transform.Find("text_doctorcheckdate_jointdirection").GetComponent<Text>();
                int joint = int.Parse(dicarr[i]["jointdirection"].ToString()) / 10;

                if (joint == 9) text_doctorcheckdate_jointdirection.text = "목-로테이션";
                else if (joint == 10) text_doctorcheckdate_jointdirection.text = "목-플렉션";
                else if (joint == 1) text_doctorcheckdate_jointdirection.text = "어깨-플렉션";
                else if (joint == 2) text_doctorcheckdate_jointdirection.text = "어깨-업덕션";
                else if (joint == 3) text_doctorcheckdate_jointdirection.text = "어깨-로테이션";
                else if (joint == 4) text_doctorcheckdate_jointdirection.text = "팔-플렉션";
                else if (joint == 5) text_doctorcheckdate_jointdirection.text = "팔-프로네이션";
                else if (joint == 6) text_doctorcheckdate_jointdirection.text = "무릎-플렉션";
                else if (joint == 7) text_doctorcheckdate_jointdirection.text = "엉덩이-플렉션";
                else if (joint == 8) text_doctorcheckdate_jointdirection.text = "엉덩이-로테이션";
                else if (joint == 20) text_doctorcheckdate_jointdirection.text = "어깨 자세 측정";
                else if (joint == 21) text_doctorcheckdate_jointdirection.text = "엉덩이 자세 측정";

                Text text_doctorcheckdate_direction = cdlp.transform.Find("text_doctorcheckdate_direction").GetComponent<Text>();
                int direction = int.Parse(dicarr[i]["jointdirection"].ToString()) % 10;

                if (direction == 1) text_doctorcheckdate_direction.text = "왼쪽";
                else if (direction == 2) text_doctorcheckdate_direction.text = "오른쪽";
                else if (direction == 3) text_doctorcheckdate_direction.text = "양쪽";

                Text text_patientlist_resume = cdlp.transform.Find("text_patientlist_resume").GetComponent<Text>();
                text_patientlist_resume.text = "최대각도 = " + dicarr[i]["maxangle"];
                cdlp.name = "" + dicarr[i]["checkdateid"];




                //20170405 한상우
                cdlp.transform.SetParent(arrayCheckDateListGameObject.transform);
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    #endregion

    #region kinectadd
    //fronttokinect.php 로 전달
    //프론트 씬에서, 오른쪽의 검진 등록을 누르고 정보를 적은 후 검신 시작 버튼을 누를 때 실행
    public void kinectadd()
    {
        //딕셔너리 생성 : 이 딕셔너리는 정보를 키넥트 단으로 전달하기 위해 사용됩니다
        Dictionary<string, string> di = new Dictionary<string, string>();

        //드롭다운과 토글 그룹을 찾기 위해 먼저 부모인 submitcenterpenal를 찾습니다
        GameObject submitcenterpenal = GameObject.Find("SubmitCenterPenal");

        //드롭다운 인스턴스 링킹
        Dropdown input_dropdown = submitcenterpenal.transform.Find("Dropdown").GetComponent<Dropdown>();

        //각각 왼쪽, 오른쪽, 양쪽
        Toggle toggle_left = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleLeft").GetComponent<Toggle>();
        Toggle toggle_right = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleRight").GetComponent<Toggle>();
        Toggle toggle_both = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleBoth").GetComponent<Toggle>();


        //검진할 관절의 위차와 방향의 선언
        int jointdirectioncode = 0;

        //@TODO : value 이용해서 더 좋게 바꾸기
        if (input_dropdown.captionText.text.Equals("목-로테이션")) jointdirectioncode += 90;
        else if (input_dropdown.captionText.text.Equals("목-플렉션")) jointdirectioncode += 100;
        //인체를 내려보는 순서대로 적느라 코드순이 아니게 됨
        else if (input_dropdown.captionText.text.Equals("어깨-플렉션")) jointdirectioncode += 10;
        else if (input_dropdown.captionText.text.Equals("어깨-업덕션")) jointdirectioncode += 20;
        else if (input_dropdown.captionText.text.Equals("어깨-로테이션")) jointdirectioncode += 30;
        else if (input_dropdown.captionText.text.Equals("팔-플렉션")) jointdirectioncode += 40;
        else if (input_dropdown.captionText.text.Equals("팔-프로네이션")) jointdirectioncode += 50;
        else if (input_dropdown.captionText.text.Equals("무릎-플렉션")) jointdirectioncode += 60;
        else if (input_dropdown.captionText.text.Equals("엉덩이-플렉션")) jointdirectioncode += 70;
        else if (input_dropdown.captionText.text.Equals("엉덩이-로테이션")) jointdirectioncode += 80;
        else if (input_dropdown.captionText.text.Equals("어깨 자세 측정")) jointdirectioncode += 200;
        else if (input_dropdown.captionText.text.Equals("엉덩이 자세 측정")) jointdirectioncode += 210;

        //검진할 관절의 방향 설정 : 1 = 왼쪽, 2 = 오른쪽, 3 = 양쪽
        //@TODO : 양쪽, 씀?
        if (toggle_left.isOn) jointdirectioncode += 1;
        else if (toggle_right.isOn) jointdirectioncode += 2;
        else if (toggle_right.isOn) jointdirectioncode += 3;

        //딕셔너리에 patientid와 forcecode, jointdirection 전달
        di.Add("patientid", patientid);
        di.Add("forcecode", "1");
        di.Add("jointdirection", jointdirectioncode.ToString());
        Debug.Log(jointdirectioncode.ToString());

        //Form 생성후 키 값 삽입
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(fronttoKinectURL, form);

        //WaitForRequestkinectadd 코루틴 실행
        //@TODO 코루틴의 두번째 인자는 di에 있어서 필요없긴한데, 확인차로 넣는것, 나중에 없앨것
        StartCoroutine(WaitForRequestkinectadd(www, jointdirectioncode));
    }
    private IEnumerator WaitForRequestkinectadd(WWW www, int i)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log(patientid + " " + i.ToString());
            GameObject.Find("SubmitPenal").SetActive(false);
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }

    }
    #endregion

    #region DoctorCheckTimeList
    public void DoctorCheckTimeList()
    {
        checkdateid = EventSystem.current.currentSelectedGameObject.name;
        //Debug.Log(patientid);

        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("checkdateid", checkdateid);
        // 2017_4_23


        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(checkTimeListURL, form);
        StartCoroutine(WaitForRequestCheckTimeList(www));
    }
    private IEnumerator WaitForRequestCheckTimeList(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);
            checktimearr = new List<checktime>();
            backupanimation.joint = new Dictionary<string, Vector3>[dicarr.Length];
            for (int i = 0; i < dicarr.Length; i++)
            {
                //오래걸리므로 하나 받을때마다 코루틴을 만들어야할것으로 보임
                checktime tmp = new checktime();

                tmp.checktimeid = (string)dicarr[i]["checktimeid"];
                tmp.framecount = int.Parse((string)dicarr[i]["framecount"]);
                tmp.jsons = ((string)dicarr[i]["dicjson"]);

                string s1 = tmp.jsons;
                string[] t = s1.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, float> dicjsondic =
                                      t.ToDictionary(s => s.Split(':')[0], s => float.Parse(s.Split(':')[1]));
                backupanimation.joint[i] = new Dictionary<string, Vector3>();
                foreach (KeyValuePair<string, float> items in dicjsondic)
                {
                    if (items.Key.IndexOf("_x") != -1)
                    {
                        int endindex = items.Key.IndexOf("_x");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.joint[i].Add(jointname, new Vector3(
                            items.Value, 0, 0)); //없을경우추가시켜야함..
                                                 //value도 옮겨야함
                                                 //spheredic[items.Key.Substring(0, items.Key.IndexOf("_from_x"))].transfo;

                    }
                    else if (items.Key.IndexOf("_y") != -1)
                    {
                        int endindex = items.Key.IndexOf("_y");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.joint[i][jointname] = new Vector3(
                            backupanimation.joint[i][jointname].x,
                            items.Value,
                            backupanimation.joint[i][jointname].z);
                    }
                    else if (items.Key.IndexOf("_z") != -1)
                    {
                        int endindex = items.Key.IndexOf("_z");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.joint[i][jointname] = new Vector3(
                            backupanimation.joint[i][jointname].x,
                            backupanimation.joint[i][jointname].y,
                            items.Value);
                    }



                    //Debug.Log(items.Key + items.Value);
                }

                //TODO : checktimejsons




                //GameObject.CreatePrimitive(PrimitiveType.Cube);



                checktimearr.Add(tmp);
            }
            //objectinformation.backuppenal.SetActive(true);
            //GameObject.Find("Main Camera").GetComponent<Camera>().enabled = false;
            backupanimation.init();
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

        // 2017_4_23 
        // TODO : 게임 오브젝트 '80'이 사라져 이 코루틴을 불러오지 못함
        Debug.Log(checkdateid);
    }
    #endregion

    #region addclick
    public void addclick()
    {
        Dictionary<string, string> di = new Dictionary<string, string>();
        //ingame.score -= ingame.score % 27;
        ///di.Add("userid", ingame.userid);
        //di.Add("score", ""+ingame.score);
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        //WWW www = new WWW(addurl, form);
        // StartCoroutine(listWaitForRequest(www));
    }
    private IEnumerator listWaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
        listclick();
    }
    #endregion

    #region 그래프관련
    void Start()
    {
        floawchart = new flowChart(28, scrollRect, contentRect, dot, date, angle);
        DDValueToJD = new int[28 + 1];
        DDValueToJD[0] = 11;
        DDValueToJD[1] = 11;
        DDValueToJD[2] = 12;
        DDValueToJD[3] = 13;
        DDValueToJD[4] = 21;
        DDValueToJD[5] = 22;
        DDValueToJD[6] = 23;
        DDValueToJD[7] = 31;
        DDValueToJD[8] = 32;
        DDValueToJD[9] = 33;
        DDValueToJD[10] = 41;
        DDValueToJD[11] = 42;
        DDValueToJD[12] = 43;
        DDValueToJD[13] = 51;
        DDValueToJD[14] = 52;
        DDValueToJD[15] = 53;
        DDValueToJD[16] = 61;
        DDValueToJD[17] = 62;
        DDValueToJD[18] = 63;
        DDValueToJD[19] = 71;
        DDValueToJD[20] = 72;
        DDValueToJD[21] = 73;
        DDValueToJD[22] = 81;
        DDValueToJD[23] = 82;
        DDValueToJD[24] = 83;
        DDValueToJD[25] = 91;
        DDValueToJD[26] = 101;
        DDValueToJD[27] = 200;
        DDValueToJD[28] = 211;
    }

    public void DoctorSceneUpdatekDateListGraph()
    {
        if (!(patientid == null))
        {
            graphPanel.SetActive(true);

            // 처음으로 문제 있는 조인트로

            // dropDown.value = (joint + 1)%10; //처음 그래프를 열었을때 드롭다운이 문제있는 부분을 가리키게
            Debug.Log(patientid);

            WWWForm form = new WWWForm();
            form.AddField("patientid", patientid);
            WWW www = new WWW(checkDateListURL, form); //POST형식으로 환자의 진료 로그를 요청

            StartCoroutine(WaitForRequestDoctorSceneUpdatekDateListGraph(www));
        }
    }

    private IEnumerator WaitForRequestDoctorSceneUpdatekDateListGraph(WWW www)
    {
        yield return www;

        if (www.text.Contains("[]"))
        {
            Debug.LogError(www.text);
        }
        else if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text); //JSON 분석 딕셔너리

            floawchart.AddData(dicarr);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

    }

    public void DropDownChange() //드롭다운의 값이 바뀌었을 때
    {
        Debug.Log("DropDownChange : " + DDValueToJD[dropDown.value]);
        floawchart.MakeChart(DDValueToJD[dropDown.value]);
    }
    #endregion



    //****************************************************************
    //이하 [함수] 단
    //****************************************************************

    #endregion
    #region function
    //@TODO : 구현
    public void listclick()
    {

    }

    public void closePage(GameObject obj)
    {
        obj.SetActive(false);
    }
    #endregion


    //****************************************************************
    //이하 [코루틴] 단
    //****************************************************************


    #region coroutine
    //@TODO : 기능설명
    IEnumerator end()
    {
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(4.5f);
        Application.Quit();
        yield return new WaitForSeconds(0.1f);

    }
    #endregion

}