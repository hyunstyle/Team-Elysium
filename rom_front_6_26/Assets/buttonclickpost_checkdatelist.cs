using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System;
public class buttonclickpost_checkdatelist : MonoBehaviour {
    public GameObject arrayCheckDateListGameObject;
    public GameObject checkDateListPenal;

    public void checkdatelist()
    {
        frontData.patientid = EventSystem.current.currentSelectedGameObject.name;
        Debug.Log(frontData.patientid);

        GameObject obj = EventSystem.current.currentSelectedGameObject;

        frontData.patientInfo[0] = obj.transform.Find("text_patientlist_name").GetComponent<Text>().text;
        frontData.patientInfo[1] = obj.transform.Find("text_patientlist_number").GetComponent<Text>().text;
        frontData.patientInfo[2] = obj.transform.Find("text_patientlist_sexstring").GetComponent<Text>().text;
        frontData.patientInfo[3] = obj.transform.Find("text_patientlist_birth").GetComponent<Text>().text;

        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("patientid", frontData.patientid);

        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(frontData.checkDateListURL, form);
        StartCoroutine(WaitForRequestCheckdatelist(www));
    }

    public IEnumerator WaitForRequestCheckdatelist(WWW www)
    {
        yield return www;

        if (www.text.Contains("[]"))
        {
            arrayCheckDateListGameObject = GameObject.Find("RightPenel/Scroll List/arraycheckdatelistgameobject");

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
            arrayCheckDateListGameObject = GameObject.Find("RightPenel/Scroll List/arraycheckdatelistgameobject");

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
}
