using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 0626 유상현
// doctor scene, 측정 후 대기 환자 진료 완료 관련 스크립트
public class buttonclickpost_deleteStandbyPatient : MonoBehaviour
{
    public string clickedPatientInfo; // 리스트 중에서 클릭한 환장즤 정보를 받아옵니다.
    public static bool isClicked;
    public static string postURL;
    public static string patientID;

    private void Start()
    {
        isClicked = false;
    }
    private void Update()
    {
        if(isClicked)
        {
            StartCoroutine(deleteSelectedPatient(postURL, patientID));
            isClicked = false;
        }
    }

    // 리스트에서 각 환자 클릭시 시행하는 함수
    public void clickedPatient()
    {
        clickedPatientInfo = this.GetComponentInChildren<Text>().text;
        Debug.Log(clickedPatientInfo); // 클릭한 환자의 정보를 로그로 띄움

        frontData.clickedData = clickedPatientInfo; // 버튼 클릭 후 함수가 종료되므로 static 필드인 frontdata 스크립트에 저장해둡니다.
    }

    // 해당 클릭된 환자의 정보를 바탕으로 실제 제거 작업을 시행합니다.
    public void completeButton() 
    {
        if(frontData.clickedData != "")
        {
            string[] splitData = frontData.clickedData.Split(':');

            // splitData에는 No, 이름, 측정부위가 각각 :으로 구분되어 있음.
            // 따라서 splitData의 길이는 4이고, No : xx    이름 : OOO    측정부위 : DDDD 의 형식이므로,
            // splitData[2]에는 'OOO\n  측정부위' 가 저장됨.

            string[] nameSplit = splitData[2].Split('\n');
            // 'OOO\n  측정부위' 를 '\n' 으로 나누어서 결과적으로
            // nameSplit[0].Trim()을 실행 시 순수 환자의 이름을 얻을 수 있다.


            string[] infoSplit;

            for (int i = 0; i<frontData.dictionaryLength; i++)
            {
                infoSplit = frontData.clickedPatientInfo[i].Split(':');
                if (nameSplit[0].Trim() == infoSplit[0].Trim())
                {
                    Debug.Log("name : " + infoSplit[0].Trim() + "patientid : " + infoSplit[1].Trim());

                    postURL = frontData.kinectsc2deleteURL;
                    patientID = infoSplit[1].Trim();
                    isClicked = true;

                    break;
                }
            }
        }
    }


    // 삭제 post 쿼리
    IEnumerator deleteSelectedPatient(string URL, string patientid)
    {
        WWWForm form = new WWWForm();

        form.AddField("patientid", patientid); //POST의 BODY부분의 필드에 kinectid 값을 입력

        WWW www = new WWW(URL, form); //WWW를 이용해 URL로 form을 전송

        yield return www;
        //www에서 요청의 결과를 받아옴

        if (www.error == null)
        {
            Debug.Log("Delete done");
            patientID = null;

        }
        else
        {
            Debug.LogError("ERROR : " + www.error);
        }

    }
}// 0626 유상현
