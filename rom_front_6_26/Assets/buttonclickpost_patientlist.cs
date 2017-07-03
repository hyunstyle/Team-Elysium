using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class buttonclickpost_patientlist : MonoBehaviour {
    public GameObject arrayPatientListGameObject;
    public GameObject arrayCheckDateListGameObject;
    public GameObject arrayDoctorCheckDateListGameObject;
    public GameObject patientListButton;

    private GameObject InputText;

    public void patientlist()
    {
        InputText = GameObject.Find("input_id");
        StartCoroutine(WaitForRequestPatientlist(new WWW(frontData.patientListURL)));
    }

    public IEnumerator WaitForRequestPatientlist(WWW www)
    {
        yield return www;

        if(www.error == null)
        {
            arrayPatientListGameObject = GameObject.Find("arraypatientlist");
            Dictionary<string, object>[] dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            // start. frontData로 복사
            frontData.PatientData = new Dictionary<int, Dictionary<string, object>>();
            for (int i = 0; i < dicarr.Length; i++)
            {
                frontData.PatientData.Add(int.Parse((string)dicarr[i]["patientid"]), dicarr[i]);
            }
            // end.
            
            int childs = arrayPatientListGameObject.transform.childCount;
            
            for(int i=childs-1;i>=0;i--)
            {
                Destroy(arrayPatientListGameObject.transform.GetChild(i).gameObject);
            } 

            for(int i=0; i<dicarr.Length;i++)
            {
                // 아무것도 입력되지 않은 채, "탐색" 버튼을 클릭 시 모든 환자 정보를 불러온다.
                // 만약 환자의 이름이 "안형빈"이면, 이중 '안', '형', '빈'만 검색해도 "안형빈"이 검색된다.
                if (!(dicarr[i]["name"].ToString().Contains(InputText.GetComponent<InputField>().text.ToString().Trim())) && (InputText.GetComponent<InputField>().text.ToString() != ""))
                {
                    continue;
                }
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
}
