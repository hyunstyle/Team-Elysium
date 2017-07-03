using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class UIsearchcontrol : MonoBehaviour {
    public GameObject searchPanel;
    public GameObject joinPenal;
    public GameObject submitPenal;
    public GameObject messagePanel;

    public void button_search_add_click()
    {
        joinPenal.SetActive(true);
    }

    public void button_search_checkup_click()
    {
        Debug.Log("patientid : " + frontData.patientid);
        if (!frontData.patientid.Equals("0"))
        {
            submitPenal.SetActive(true);

            string info = frontData.patientInfo[1].Trim() + " / " + frontData.patientInfo[0].Trim() + " / "
                + (frontData.patientInfo[2].Trim().Equals("1") ? "남" : "여") + " / " + frontData.patientInfo[3].Trim();

            Debug.Log(info);

            submitPenal.transform.Find("SubmitCenterPenal").Find("Text").GetComponent<Text>().text = info;
        }
        else
        {
            messagePanel.SetActive(true);
        }
    }

    public void button_join_close_click()
    {
        joinPenal.SetActive(false);
    }

    public void button_submit_close_click()
    {
        submitPenal.SetActive(false);
    }

    public void closePage(GameObject obj)
    {
         obj.SetActive(false);
    }
}
