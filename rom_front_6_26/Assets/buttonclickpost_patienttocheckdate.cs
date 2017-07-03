using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;

public class buttonclickpost_patienttocheckdate : MonoBehaviour
{
    public GameObject patientListButton;
    public GameObject docterPatientListButton;
    public GameObject checkDateListPenal;
    public GameObject doctorCheckDateListButton;

    public GameObject arrayCheckDateListGameObject;

    public void doctorcheckdatelist()
    {
        frontData.patientid = EventSystem.current.currentSelectedGameObject.name;
        // 이벤트 시스템을 이용하여 왼쪽의 현재 선택된 버튼을 반환한다.
        Debug.Log(frontData.patientid);

        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("patientid", frontData.patientid);
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }

        WWW www = new WWW(frontData.checkDateListURL, form);

        StartCoroutine(WaitForRequestDoctorCheckdatelist(www));
    }

    public IEnumerator WaitForRequestDoctorCheckdatelist(WWW www)
    {
        yield return www;
        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("arraydoctorcheckdatelistgameobject");

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }
        }
        if (www.error == null)
        {
            arrayCheckDateListGameObject = GameObject.Find("arraydoctorcheckdatelistgameobject");

            Dictionary<string, object>[] dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);
            frontData.CheckDateData = new Dictionary<int, Dictionary<string, object>>();
            for(int i=0; i<dicarr.Length;i++)
            {
                frontData.CheckDateData.Add(int.Parse((string)dicarr[i]["checkdateid"]), dicarr[i]);
            }
            
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

                

                cdlp.transform.SetParent(arrayCheckDateListGameObject.transform);
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}