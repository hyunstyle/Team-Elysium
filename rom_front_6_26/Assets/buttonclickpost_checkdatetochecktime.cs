using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public class buttonclickpost_checkdatetochecktime : MonoBehaviour
{
    public GameObject patientListButton; //의사와 프론트 씬에서 좌측에 나오는 환자의 목록을 구성하는 버튼입니다.
    public GameObject docterPatientListButton; //
    public GameObject checkDateListPenal; //
    public GameObject doctorCheckDateListButton; //
    public GameObject PNG;

    public void DoctorCheckTimeList()
    {
        frontData.checkdateid = EventSystem.current.currentSelectedGameObject.name;

        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("checkdateid", frontData.checkdateid);

        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(frontData.checkTimeListURL, form);
        StartCoroutine(WaitForRequestCheckTimeList(www));
    }

    public IEnumerator WaitForRequestCheckTimeList(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Console.WriteLine(www.text);
            Dictionary<string, object>[] dicarr = (Dictionary<string, object>[])jsonf.Readarray(www.text);
            frontData.checktimearr = new List<frontData.checktime>();
            backupanimation.joint = new Dictionary<string, Vector3>[dicarr.Length];
            backupanimation.jointrot = new Dictionary<string, Vector4>[dicarr.Length]; //ys0625

            for (int i = 0; i < dicarr.Length; i++)
            {
                frontData.checktime tmp = new frontData.checktime();

                tmp.checktimeid = (string)dicarr[i]["checktimeid"];
                tmp.framecount = int.Parse((string)dicarr[i]["framecount"]);
                tmp.jsons = ((string)dicarr[i]["dicjson"]);

                string s1 = tmp.jsons;
                string[] t = s1.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

                Dictionary<string, float> dicjsondic = t.ToDictionary(s => s.Split(':')[0], s => float.Parse(s.Split(':')[1]));
                backupanimation.joint[i] = new Dictionary<string, Vector3>();
                backupanimation.jointrot[i] = new Dictionary<string, Vector4>(); //ys 0625
                foreach (KeyValuePair<string, float> items in dicjsondic)
                {
                    if (items.Key.IndexOf("_x") != -1)
                    {
                        int endindex = items.Key.IndexOf("_x");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.joint[i].Add(jointname, new Vector3(
                            items.Value, 0, 0));
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
                    //ys0625
                    else if (items.Key.IndexOf("_r_x") != -1)
                    {
                        int endindex = items.Key.IndexOf("_r_x");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.jointrot[i].Add(jointname, new Vector4(
                            items.Value, 0, 0, 0));
                    }
                    else if (items.Key.IndexOf("_r_y") != -1)
                    {
                        int endindex = items.Key.IndexOf("_r_y");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.jointrot[i][jointname] = new Vector4(
                            backupanimation.jointrot[i][jointname].x,
                            items.Value,
                            backupanimation.jointrot[i][jointname].z,
                            backupanimation.jointrot[i][jointname].w);
                    }
                    else if (items.Key.IndexOf("_r_z") != -1)
                    {
                        int endindex = items.Key.IndexOf("_r_z");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.jointrot[i][jointname] = new Vector4(
                            backupanimation.jointrot[i][jointname].x,
                            backupanimation.jointrot[i][jointname].y,
                            items.Value,
                            backupanimation.jointrot[i][jointname].w);
                    }
                    else if (items.Key.IndexOf("_r_w") != -1)
                    {
                        int endindex = items.Key.IndexOf("_r_w");
                        string jointname = items.Key.Substring(0, endindex);
                        backupanimation.jointrot[i][jointname] = new Vector4(
                            backupanimation.jointrot[i][jointname].x,
                            backupanimation.jointrot[i][jointname].y,
                            backupanimation.jointrot[i][jointname].z,
                            items.Value);
                    }


                }
                frontData.checktimearr.Add(tmp);
            }

            backupanimation.init();
            backupanimation temp = new backupanimation();
            temp.PNG = PNG;
            StartCoroutine(temp.downloadPNG(frontData.checkdateid));


        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }

    }


}