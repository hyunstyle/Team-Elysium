using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//0626 유상현
// 측정 후 대기 환자 관련 스크립트
public class afterMeasurementListScript : MonoBehaviour
{
    public float checkTime = 0.0f;

    private void Update()
    {
        checkTime += Time.deltaTime;
        if (checkTime > 5.0f)
        {
            updateAfterMeasurementList();
            checkTime = 0.0f;
        }

    }

    private GameObject arrayCheckDateListGameObject;

    public GameObject waitingListPanel;
    private Text text_afterMeasurement;

    public void updateAfterMeasurementList()
    {
        WWW www = new WWW(frontData.kinectsc2URL);
        StartCoroutine(updateWaitingList(www));
    }

    public IEnumerator updateWaitingList(WWW www)
    {
        yield return www;

        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("WaitPanel2/Scroll List/arraycheckdatelistgameobject");

            int childs = arrayCheckDateListGameObject.transform.childCount;

            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            //Debug.LogError("비었음");
        }
        // check for errors
        else if (www.error == null)
        {
            //Debug.Log("WWW Ok!: " + www.text);
            arrayCheckDateListGameObject = GameObject.Find("WaitPanel2/Scroll List/arraycheckdatelistgameobject");

            Dictionary<string, object>[] dicarr = null;

            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);
            //Debug.Log("length : " + dicarr.Length);

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            frontData.dictionaryLength = dicarr.Length;
            frontData.clickedPatientInfo = new string[dicarr.Length];
            for (int i = 0; i < dicarr.Length; i++)
            {
                GameObject cdlp = Instantiate(waitingListPanel);

                text_afterMeasurement = cdlp.transform.Find("Text").GetComponent<Text>();
                text_afterMeasurement.text = "No : " + (i + 1) + "     ";
                text_afterMeasurement.text += "이름 : " + dicarr[i]["name"] + "\n";

                //Debug.Log(text_afterMeasurement.text);
                frontData.clickedPatientInfo[i] = dicarr[i]["name"] + ":" + dicarr[i]["patientid"];

                int jointDir = Convert.ToInt32(dicarr[i]["jointdirection"]);
                switch (jointDir)
                {
                    case 91:
                        text_afterMeasurement.text += "측정부위 : 목-로테이션(좌)\n";
                        break;
                    case 92:
                        text_afterMeasurement.text += "측정부위 : 목-로테이션(우)\n";
                        break;
                    case 101:
                        text_afterMeasurement.text += "측정부위 : 목-플렉션(좌)\n";
                        break;
                    case 102:
                        text_afterMeasurement.text += "측정부위 : 목-플렉션(우)\n";
                        break;
                    case 11:
                        text_afterMeasurement.text += "측정부위 : 어깨-플렉션(좌)\n";
                        break;
                    case 12:
                        text_afterMeasurement.text += "측정부위 : 어깨-플렉션(우)\n";
                        break;
                    case 21:
                        text_afterMeasurement.text += "측정부위 : 어깨-업덕션(좌)\n";
                        break;
                    case 22:
                        text_afterMeasurement.text += "측정부위 : 어깨-업덕션(우)\n";
                        break;
                    case 31:
                        text_afterMeasurement.text += "측정부위 : 어깨-로테이션(좌)\n";
                        break;
                    case 32:
                        text_afterMeasurement.text += "측정부위 : 어깨-로테이션(우)\n";
                        break;
                    case 41:
                        text_afterMeasurement.text += "측정부위 : 팔-플렉션(좌)\n";
                        break;
                    case 42:
                        text_afterMeasurement.text += "측정부위 : 팔-플렉션(우)\n";
                        break;
                    case 51:
                        text_afterMeasurement.text += "측정부위 : 팔-프로네이션(좌)\n";
                        break;
                    case 52:
                        text_afterMeasurement.text += "측정부위 : 팔-프로네이션(우)\n";
                        break;
                    case 61:
                        text_afterMeasurement.text += "측정부위 : 무릎-플렉션(좌)\n";
                        break;
                    case 62:
                        text_afterMeasurement.text += "측정부위 : 무릎-플렉션(우)\n";
                        break;
                    case 71:
                        text_afterMeasurement.text += "측정부위 : 엉덩이-플렉션(좌)\n";
                        break;
                    case 72:
                        text_afterMeasurement.text += "측정부위 : 엉덩이-플렉션(우)\n";
                        break;
                    case 81:
                        text_afterMeasurement.text += "측정부위 : 엉덩이-로테이션(좌)\n";
                        break;
                    case 82:
                        text_afterMeasurement.text += "측정부위 : 엉덩이-로테이션(우)\n";
                        break;
                    case 201:
                        text_afterMeasurement.text += "측정부위 : 어깨 자세 측정(좌)\n";
                        break;
                    case 202:
                        text_afterMeasurement.text += "측정부위 : 어깨 자세 측정(우)\n";
                        break;
                    case 211:
                        text_afterMeasurement.text += "측정부위 : 엉덩이 자세 측정(좌)\n";
                        break;
                    case 212:
                        text_afterMeasurement.text += "측정부위 : 엉덩이 자세 측정(우)\n";
                        break;
                    default:
                        break;

                }

                cdlp.name = "" + i;
                cdlp.transform.parent = arrayCheckDateListGameObject.transform;

            }
           // Debug.Log("받아오기 완료2");
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
} // 0626 유상현
