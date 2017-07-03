using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

class flowChart : MonoBehaviour{

    private ScrollRect scrollView;
    private RectTransform content;
    private GameObject dot;
    private GameObject dateText;
    private Text maxAngle;

    private List<GameObject> dots;
    private List<GameObject> dateTexts;

    
    public float dotScaler;

    private Dictionary<int, float> MaxData;
    private Dictionary<int, List<float>> Data;
    private Dictionary<int, List<string>> DateData;
   

    public flowChart(int NumberOfJointDirection, ScrollRect scroll, RectTransform cont, GameObject dot, GameObject date , Text angle)
    {
        scrollView = scroll;
        content = cont;
        this.dot = dot;
        maxAngle = angle;
        dateText = date;

        Data = new Dictionary<int, List<float>>();
        MaxData = new Dictionary<int, float>();
        DateData = new Dictionary<int, List<string>>();
        dots = new List<GameObject>();
        dateTexts = new List<GameObject>();

        Data.Add(11, new List<float>()); MaxData.Add(11, 0); DateData.Add(11, new List<string>());
        Data.Add(12, new List<float>()); MaxData.Add(12, 0); DateData.Add(12, new List<string>());
        Data.Add(13, new List<float>()); MaxData.Add(13, 0); DateData.Add(13, new List<string>());
        Data.Add(21, new List<float>()); MaxData.Add(21, 0); DateData.Add(21, new List<string>());
        Data.Add(22, new List<float>()); MaxData.Add(22, 0); DateData.Add(22, new List<string>());
        Data.Add(23, new List<float>()); MaxData.Add(23, 0); DateData.Add(23, new List<string>()); 
        Data.Add(31, new List<float>()); MaxData.Add(31, 0); DateData.Add(31, new List<string>());
        Data.Add(32, new List<float>()); MaxData.Add(32, 0); DateData.Add(32, new List<string>()); 
        Data.Add(33, new List<float>()); MaxData.Add(33, 0); DateData.Add(33, new List<string>());
        Data.Add(41, new List<float>()); MaxData.Add(41, 0); DateData.Add(41, new List<string>());
        Data.Add(42, new List<float>()); MaxData.Add(42, 0); DateData.Add(42, new List<string>());
        Data.Add(43, new List<float>()); MaxData.Add(43, 0); DateData.Add(43, new List<string>());
        Data.Add(51, new List<float>()); MaxData.Add(51, 0); DateData.Add(51, new List<string>());
        Data.Add(52, new List<float>()); MaxData.Add(52, 0); DateData.Add(52, new List<string>());
        Data.Add(53, new List<float>()); MaxData.Add(53, 0); DateData.Add(53, new List<string>());
        Data.Add(61, new List<float>()); MaxData.Add(61, 0); DateData.Add(61, new List<string>());
        Data.Add(62, new List<float>()); MaxData.Add(62, 0); DateData.Add(62, new List<string>());
        Data.Add(63, new List<float>()); MaxData.Add(63, 0); DateData.Add(63, new List<string>());
        Data.Add(71, new List<float>()); MaxData.Add(71, 0); DateData.Add(71, new List<string>());
        Data.Add(72, new List<float>()); MaxData.Add(72, 0); DateData.Add(72, new List<string>());
        Data.Add(73, new List<float>()); MaxData.Add(73, 0); DateData.Add(73, new List<string>());
        Data.Add(81, new List<float>()); MaxData.Add(81, 0); DateData.Add(81, new List<string>());
        Data.Add(82, new List<float>()); MaxData.Add(82, 0); DateData.Add(82, new List<string>());
        Data.Add(83, new List<float>()); MaxData.Add(83, 0); DateData.Add(83, new List<string>());
        Data.Add(91, new List<float>()); MaxData.Add(91, 0); DateData.Add(91, new List<string>());
        Data.Add(101, new List<float>()); MaxData.Add(101, 0); DateData.Add(101, new List<string>());
        Data.Add(201, new List<float>()); MaxData.Add(201, 0); DateData.Add(201, new List<string>());
    }

    public void AddData(Dictionary<string, object>[] dic)
    {
            for (int i = 0; i < dic.Length; i++)
            {
                Data[int.Parse(dic[i]["jointdirection"] + "")].Add(float.Parse(dic[i]["maxangle"] + ""));
                DateData[int.Parse(dic[i]["jointdirection"] + "")].Add(dic[i]["datetime"].ToString().Substring(2, 8));

                Debug.Log(dic[i]["jointdirection"] + "     " + dic[i]["maxangle"] + "");
                if (i == 0 || int.Parse(dic[i]["jointdirection"] + "") != int.Parse(dic[i - 1]["jointdirection"] + ""))
                {
                    Debug.Log("Max");
                    MaxData[int.Parse(dic[i]["jointdirection"] + "")] = float.Parse(dic[i]["maxangle"] + "");
                }
                else
                {
                    if (MaxData[int.Parse(dic[i]["jointdirection"] + "")] < float.Parse(dic[i]["maxangle"] + ""))
                    {
                        MaxData[int.Parse(dic[i]["jointdirection"] + "")] = float.Parse(dic[i]["maxangle"] + "");
                    }
                }
                //Debug.Log(" M " + MaxData[int.Parse(dic[i]["jointdirection"] + "")]);
            }
            Debug.Log("끝");
    }

    public void MakeChart(int jointDirection)
    {
        foreach(GameObject d in dots)
            Destroy(d);
        foreach (GameObject d in dateTexts)
            Destroy(d);

        dots.Clear();
        dateTexts.Clear();

        Debug.Log(MaxData[11]);
        maxAngle.text = "";
        //dateText.GetComponent<Text>().text = "";
        for(int i = 9; i >= 0; i--)
        {
            maxAngle.text += Math.Round(MaxData[jointDirection] * i / 9f, 2).ToString() + "\n\n";
        }
        dotScaler = content.rect.height / MaxData[jointDirection] * 0.76f; //점의 세로위치 비율 조정
        content.GetComponent<RectTransform>().sizeDelta = new Vector2(Data[jointDirection].Count * 100f + 40 ,content.GetComponent<RectTransform>().sizeDelta.y);
        for(int i = 0; i < Data[jointDirection].Count; i++)
        {
            dots.Add((GameObject)Instantiate(dot, content.transform));
            dateTexts.Add((GameObject)Instantiate(dateText, content.transform));

            //2017.3.26 상우
            dots[i].name = "Value : " + Data[jointDirection][i];
            dots[i].name += "\nData : " + DateData[jointDirection][i];

            //Debug.Log(dots[i].name);

            dots[i].transform.localPosition = new Vector3(i * 100f + 70f, Data[jointDirection][i] * dotScaler + 57f);
            dateTexts[i].transform.localPosition = new Vector3(i * 100f + 70f, 10f);
            dateTexts[i].GetComponent<Text>().text = DateData[jointDirection][i];

            if(i != 0)
            {
                dots[i].transform.GetChild(0).GetComponent<RectTransform>().sizeDelta += Vector2.up *
                Vector2.Distance(dots[i].transform.localPosition, dots[i - 1].transform.localPosition);

                dots[i].transform.GetChild(0).transform.Rotate(Vector3.forward, Vector3.Angle(dots[i - 1].transform.localPosition - dots[i].transform.localPosition, Vector2.up));
            }
        }

        for(int i = 0; i < dots.Count; i++)
        {
            //dots[i].transform.localPosition = new Vector3(i * 95f + 70f, Data[jointDirection][i] * dotScaler + 57f);
        }


    }
}
