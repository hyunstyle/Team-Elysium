using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class buttonclickpost_searchtopatient : MonoBehaviour
{
    public GameObject arrayPatientListGameObject;
    public GameObject docterPatientListButton;

    private GameObject InputText;

    public void doctorpatientlist()
    {
        InputText = GameObject.Find("input_id");
        StartCoroutine(WaitForRequestDoctorPatientlist(new WWW(frontData.patientListURL)));
    }

    public IEnumerator WaitForRequestDoctorPatientlist(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log("WWW OK!: " + www.text);
            arrayPatientListGameObject = GameObject.Find("arraydoctorpatientlist");

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            // start. frontData로 복사
            frontData.PatientData = new Dictionary<int, Dictionary<string, object>>();
            for (int i = 0; i < dicarr.Length; i++)
            {
                frontData.PatientData.Add(int.Parse((string)dicarr[i]["patientid"]), dicarr[i]);
            }
            // end.

            int childs = arrayPatientListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayPatientListGameObject.transform.GetChild(i).gameObject);
            }

            foreach (KeyValuePair<int, Dictionary<string, object>> tmp in frontData.PatientData)
            {
                Debug.Log("Name : " + tmp.Value["name"].ToString());
                // 아무것도 입력되지 않은 채, "탐색" 버튼을 클릭 시 모든 환자 정보를 불러온다.
                // 만약 환자의 이름이 "안형빈"이면, 이중 '안', '형', '빈'만 검색해도 "안형빈"이 검색된다.
                if (!(tmp.Value["name"].ToString().Contains(InputText.GetComponent<InputField>().text.ToString().Trim())) && (InputText.GetComponent<InputField>().text.ToString() != ""))
                {
                    continue;
                }
                GameObject tmpdplb = Instantiate(docterPatientListButton);

                Text text_name = tmpdplb.transform.Find("text_patientlist_name").GetComponent<Text>();
                Text text_number = tmpdplb.transform.Find("text_patientlist_number").GetComponent<Text>();
                Text text_sex = tmpdplb.transform.Find("text_patientlist_sexstring").GetComponent<Text>();
                Text text_birth = tmpdplb.transform.Find("text_patientlist_birth").GetComponent<Text>();

                text_name.text = tmp.Value["name"] + "\n";
                if (int.Parse(tmp.Value["sex"].ToString()) == 1)
                    text_sex.text = "남";
                else
                    text_sex.text = "여";

                text_birth.text = tmp.Value["birth"] + "\n";
                text_number.text = tmp.Value["number"] + "\n";
                tmpdplb.name = "" + tmp.Key.ToString();

                tmpdplb.transform.SetParent(arrayPatientListGameObject.transform);
            }
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}
