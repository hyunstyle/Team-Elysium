using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 0625 유상현
// 대기 환자 업데이트 스크립트
public class waitingListUpdateScript : MonoBehaviour
{

    public float checkTime = 0.0f;

    private void Update()
    {
        checkTime += Time.deltaTime;
        if (checkTime > 5.0f)
        {
            updateWaitingPatientList();
            checkTime = 0.0f;
        }

    }

    private GameObject arrayCheckDateListGameObject;

    public GameObject waitingListPanel;
    public GameObject currentPatient;
    private Text text_waiting;

    public void updateWaitingPatientList()
    {
        WWW www = new WWW(frontData.registeredListURL);
        StartCoroutine(updateWaitingList(www));
        //Debug.Log("코루틴 시작");
    }

    public IEnumerator updateWaitingList(WWW www)
    {
        yield return www;

        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("WaitPanel/Scroll List/arraycheckdatelistgameobject");

            int childs = arrayCheckDateListGameObject.transform.childCount;
          

            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject); 
            }

            currentPatient.GetComponent<Text>().text = "";
            Debug.Log("데이터가 없습니다.");
        }
        // check for errors
        else if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            arrayCheckDateListGameObject = GameObject.Find("WaitPanel/Scroll List/arraycheckdatelistgameobject");

            Dictionary<string, object>[] dicarr = null;

            dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);

            int childs = arrayCheckDateListGameObject.transform.childCount;
            for (int i = childs - 1; i >= 0; i--)
            {
                Destroy(arrayCheckDateListGameObject.transform.GetChild(i).gameObject);
            }

            //frontData.dictionaryLength = dicarr.Length;
            //frontData.clickedPatientInfo = new string[dicarr.Length];
            for (int i = 0; i < dicarr.Length; i++)
            {
                if (i == 0)
                {
                    currentPatient.GetComponent<Text>().text = "" + dicarr[i]["name"];
                    continue;
                }

                GameObject cdlp = Instantiate(waitingListPanel);

                text_waiting = cdlp.transform.Find("Text").GetComponent<Text>();
                text_waiting.text = "No : " + i + "     ";
                text_waiting.text += "이름 : " + dicarr[i]["name"] + "\n";

                //frontData.clickedPatientInfo[i] = dicarr[i]["name"] + ":" + dicarr[i]["patientid"];
                // Debug.Log("데이터들 - " + frontData.clickedPatientInfo[i]);
                int jointDir = Convert.ToInt32(dicarr[i]["jointdirection"]);
                switch (jointDir)
                {
                    case 91:
                        text_waiting.text += "측정부위 : 목-로테이션(좌)\n";
                        break;
                    case 92:
                        text_waiting.text += "측정부위 : 목-로테이션(우)\n";
                        break;
                    case 101:
                        text_waiting.text += "측정부위 : 목-플렉션(좌)\n";
                        break;
                    case 102:
                        text_waiting.text += "측정부위 : 목-플렉션(우)\n";
                        break;
                    case 11:
                        text_waiting.text += "측정부위 : 어깨-플렉션(좌)\n";
                        break;
                    case 12:
                        text_waiting.text += "측정부위 : 어깨-플렉션(우)\n";
                        break;
                    case 21:
                        text_waiting.text += "측정부위 : 어깨-업덕션(좌)\n";
                        break;
                    case 22:
                        text_waiting.text += "측정부위 : 어깨-업덕션(우)\n";
                        break;
                    case 31:
                        text_waiting.text += "측정부위 : 어깨-로테이션(좌)\n";
                        break;
                    case 32:
                        text_waiting.text += "측정부위 : 어깨-로테이션(우)\n";
                        break;
                    case 41:
                        text_waiting.text += "측정부위 : 팔-플렉션(좌)\n";
                        break;
                    case 42:
                        text_waiting.text += "측정부위 : 팔-플렉션(우)\n";
                        break;
                    case 51:
                        text_waiting.text += "측정부위 : 팔-프로네이션(좌)\n";
                        break;
                    case 52:
                        text_waiting.text += "측정부위 : 팔-프로네이션(우)\n";
                        break;
                    case 61:
                        text_waiting.text += "측정부위 : 무릎-플렉션(좌)\n";
                        break;
                    case 62:
                        text_waiting.text += "측정부위 : 무릎-플렉션(우)\n";
                        break;
                    case 71:
                        text_waiting.text += "측정부위 : 엉덩이-플렉션(좌)\n";
                        break;
                    case 72:
                        text_waiting.text += "측정부위 : 엉덩이-플렉션(우)\n";
                        break;
                    case 81:
                        text_waiting.text += "측정부위 : 엉덩이-로테이션(좌)\n";
                        break;
                    case 82:
                        text_waiting.text += "측정부위 : 엉덩이-로테이션(우)\n";
                        break;
                    case 201:
                        text_waiting.text += "측정부위 : 어깨 자세 측정(좌)\n";
                        break;
                    case 202:
                        text_waiting.text += "측정부위 : 어깨 자세 측정(우)\n";
                        break;
                    case 211:
                        text_waiting.text += "측정부위 : 엉덩이 자세 측정(좌)\n";
                        break;
                    case 212:
                        text_waiting.text += "측정부위 : 엉덩이 자세 측정(우)\n";
                        break;
                    default:
                        break;

                }

                cdlp.name = "" + i;
                  
                cdlp.transform.parent = arrayCheckDateListGameObject.transform;
            }
            Debug.Log("받아오기 완료");
            
            
            //currentPatient.GetComponent<Text>().text = "" + dicarr[minPatientNumber]["name"];

            //currentPatientInfo = "이름 : " + dicarr[minPatientNumber]["name"] + "\n" + 
           
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
}// 0625 유상현 end
