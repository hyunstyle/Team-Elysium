using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using CP.ProChart;
using System.Text.RegularExpressions;
using JsonFx.Json;
using System.Net;
using System.IO;
using System.Net.Sockets;

public class download_PNGFILE : MonoBehaviour
{
    public void StartDownload()
    {
        StartCoroutine(downloadPNG(frontData.checkdateid));
    }
    public IEnumerator downloadPNG(string checkdateid)
    {
        WWWForm form = new WWWForm();
        form.AddField("checkdateid", checkdateid);
        WWW www = new WWW(frontData.checkDateListURL, form);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text.ToString());
            Dictionary<string, object>[] kdict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text);
            frontData.imagename = (string)kdict[0]["image"];
            StartCoroutine(downloadPNGfile_fromFTP());
        }
        else
        {
            Debug.Log("파일 받기 에러");
        }
    }
    public IEnumerator downloadPNGfile_fromFTP()
    {
        Debug.Log(frontData.imagename);
        string httpurl = "http://igrus.mireene.com/image/" + frontData.imagename;
        WWW www = new WWW(httpurl);
        yield return www;
        Debug.Log("2");
        if (www.error == null)
        {
            Debug.Log("111111111111111");
            gameObject.GetComponent<RawImage>().material.mainTexture = www.texture;
            gameObject.GetComponent<RawImage>().enabled = false;
            gameObject.GetComponent<RawImage>().enabled = true;
            Debug.Log("111111111111111");
        }
        else
        {
            Debug.Log(www.error.ToString());
        }
    }
}