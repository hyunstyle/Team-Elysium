using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class buttonclickpost_addkinect : MonoBehaviour {
   public string fronttoKinectURL = "http://igrus.mireene.com/rom_server/fronttokinect.php";
    public GameObject waitingPatientListPanel;
    public void kinectadd()
    {
        //딕셔너리 생성 : 이 딕셔너리는 정보를 키넥트 단으로 전달하기 위해 사용됩니다
        Dictionary<string, string> di = new Dictionary<string, string>();

        //드롭다운과 토글 그룹을 찾기 위해 먼저 부모인 submitcenterpenal를 찾습니다
        GameObject submitcenterpenal = GameObject.Find("SubmitCenterPenal");

        //드롭다운 인스턴스 링킹
        Dropdown input_dropdown = submitcenterpenal.transform.Find("Dropdown").GetComponent<Dropdown>();

        //각각 왼쪽, 오른쪽, 양쪽
        Toggle toggle_left = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleLeft").GetComponent<Toggle>();
        Toggle toggle_right = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleRight").GetComponent<Toggle>();
        Toggle toggle_both = submitcenterpenal.transform.Find("ToggleGroup").Find("ToggleBoth").GetComponent<Toggle>();


        //검진할 관절의 위차와 방향의 선언
        int jointdirectioncode = 0;

        //@TODO : value 이용해서 더 좋게 바꾸기
        if (input_dropdown.captionText.text.Equals("목-로테이션")) jointdirectioncode += 90;
        else if (input_dropdown.captionText.text.Equals("목-플렉션")) jointdirectioncode += 100;
        //인체를 내려보는 순서대로 적느라 코드순이 아니게 됨
        else if (input_dropdown.captionText.text.Equals("어깨-플렉션")) jointdirectioncode += 10;
        else if (input_dropdown.captionText.text.Equals("어깨-업덕션")) jointdirectioncode += 20;
        else if (input_dropdown.captionText.text.Equals("어깨-로테이션")) jointdirectioncode += 30;
        else if (input_dropdown.captionText.text.Equals("팔-플렉션")) jointdirectioncode += 40;
        else if (input_dropdown.captionText.text.Equals("팔-프로네이션")) jointdirectioncode += 50;
        else if (input_dropdown.captionText.text.Equals("무릎-플렉션")) jointdirectioncode += 60;
        else if (input_dropdown.captionText.text.Equals("엉덩이-플렉션")) jointdirectioncode += 70;
        else if (input_dropdown.captionText.text.Equals("엉덩이-로테이션")) jointdirectioncode += 80;
        else if (input_dropdown.captionText.text.Equals("어깨 자세 측정")) jointdirectioncode += 200;
        else if (input_dropdown.captionText.text.Equals("엉덩이 자세 측정")) jointdirectioncode += 210;

        //검진할 관절의 방향 설정 : 1 = 왼쪽, 2 = 오른쪽, 3 = 양쪽
        //@TODO : 양쪽, 씀?
        if (toggle_left.isOn) jointdirectioncode += 1;
        else if (toggle_right.isOn) jointdirectioncode += 2;
        else if (toggle_right.isOn) jointdirectioncode += 3;

        //딕셔너리에 patientid와 forcecode, jointdirection 전달
        di.Add("patientid", frontData.patientid);
        di.Add("forcecode", "1");
        di.Add("jointdirection", jointdirectioncode.ToString());
        Debug.Log(jointdirectioncode.ToString());

        //Form 생성후 키 값 삽입
        WWWForm form = new WWWForm();
        foreach (KeyValuePair<string, string> post_arg in di)
        {
            form.AddField(post_arg.Key, post_arg.Value);
        }
        WWW www = new WWW(fronttoKinectURL, form);

        //WaitForRequestkinectadd 코루틴 실행
        //@TODO 코루틴의 두번째 인자는 di에 있어서 필요없긴한데, 확인차로 넣는것, 나중에 없앨것
        StartCoroutine(WaitForRequestkinectadd(www, jointdirectioncode));
        waitingPatientList.Instance.updateWaitingPatientList();
    }

    public IEnumerator WaitForRequestkinectadd(WWW www, int i)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log(frontData.patientid + " " + i.ToString());
            //GameObject.Find("SubmitPenal").SetActive(false);
        }
        else
        {
            Debug.Log("ERROR: " + www.error);
        }
    }
}
