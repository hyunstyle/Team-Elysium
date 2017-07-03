
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;


public class getpost : MonoBehaviour
{
    //get post test
    

    string listurl = "http://igrus00.mireene.com/choiranking/boardlist.php";
    string addurl = "http://igrus00.mireene.com/choiranking/boardadd.php";
    bool cg = false;
    void Start()
    {
        //addclick();
        //listclick();
        //WWW results = GET("http://api.androidhive.info/contacts/");
        //Debug.Log(results.text);
    }

    public void listclick()
    {
        WWW www = new WWW(listurl);
        StartCoroutine(WaitForRequest(www));
    }
    public void addclick()
    {
        Dictionary<string, string> di = new Dictionary<string, string>();
        di.Add("userid", "temp"+UnityEngine.Random.Range(0,1000));
        di.Add("score", ""+ UnityEngine.Random.Range(100,10000));
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(addurl, form);
        StartCoroutine(listWaitForRequest(www));
    }

    public WWW GET(string url)
    {

        WWW www = new WWW(url);
        StartCoroutine(WaitForRequest(www));
        return www;
    }


    public WWW POST(string url, Dictionary<string, string> post)
    {
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<String, String> post_arg in post)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(url, form);

        StartCoroutine(WaitForRequest(www));
        return www;
    }

    private IEnumerator listWaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    private IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
            

            Dictionary<string, object>[] dicarr;
            dicarr = (Dictionary<string,object>[])jsonf.Readarray(www.text);
            
            for(int i=0;i<dicarr.Length;i++)
            {
                Debug.Log("" + dicarr[i]["userid"] + " " + dicarr[i]["score"]);
            }
            
                
                
            



            //Debug.Log((string)dicjson["userid"] + " " + (string)dicjson["long"] + " " + (string)dicjson["lat"] + " " + (string)dicjson["mmss"]);
            //Debug.Log (dicjson ["result"] + " " + dicjson ["result_text"]);

        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }


}
