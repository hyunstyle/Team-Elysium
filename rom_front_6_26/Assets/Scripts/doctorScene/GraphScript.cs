using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CP.ProChart;

public class GraphScript : MonoBehaviour {

    //그래프 관련 변수들
    string checkDateListURL = "http://igrus.mireene.com/rom_server/checkdatelist.php";

    public GameObject graphPanel; //그래프를 보여줄 패널의 레퍼런스입니다.
    public GameObject textRect; //그래프에서 날짜를 표시해주는 텍스트입니다. 인스턴스로 존재해 씬 내부에 있습니다.
    public RectTransform tooltip; //그래프 패널 내부의 노드에 커서를 호버링하면 보여줄 정보를 담는 툴팁 상자입니다.
    public Text tooltipText; //바로 위 tooltip 내부의 텍스트입니다.
    public Dropdown dropDown; //그래프 패널 내부에 존재하는 드롭다운입니다.

    public LineChart lineChart;

    private int[] DDValueToJD;
    public ScrollRect scrollRect;
    public RectTransform contentRect;
    public GameObject dot;
    public GameObject date;
    public Text angle;

    private flowChart floawchart;

    public static List<string> dateSet;
    //이상 그래프 관련 변수들

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

        dropDown.onValueChanged.AddListener(delegate { DropDownChange(); });
    }

    public void DoctorSceneUpdatekDateListGraph()
    {
        if (!(buttonclickpost.patientid == null))
        {
            graphPanel.SetActive(true);

            // 처음으로 문제 있는 조인트로

            // dropDown.value = (joint + 1)%10; //처음 그래프를 열었을때 드롭다운이 문제있는 부분을 가리키게
            Debug.Log(buttonclickpost.patientid);

            WWWForm form = new WWWForm();
            form.AddField("patientid", buttonclickpost.patientid);
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
        Debug.Log(DDValueToJD[dropDown.value]);
        floawchart.MakeChart(DDValueToJD[dropDown.value]);
    }
}
