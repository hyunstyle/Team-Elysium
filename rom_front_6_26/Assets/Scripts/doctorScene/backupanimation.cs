using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using JsonFx.Json;
public class backupanimation : MonoBehaviour
{
    public static Dictionary<string, Vector3>[] joint;
    public static Dictionary<string, Vector4>[] jointrot; //ys0625

    static List<string> jointFrom;
    static List<string> jointTo;

    static GameObject sphereRoot;
    static GameObject linerendereRoot;
    static Dictionary<string, GameObject> sphere;
    public Material linematp;
    static Material linemat;

    static GameObject SearchPanel;
    static GameObject LeftPanel;
    static GameObject RightPanel;

    static GameObject BackupPanel;
    static GameObject BackupSlider;

    static List<GameObject> linerenderer;

    public GameObject man;

    //Start() 에서 초기화 됨
    #region UISkeletonValues
    RectTransform DotLeftKnee;
    RectTransform DotRightKnee;

    RectTransform DotLeftHip;
    RectTransform DotRightHip;

    RectTransform DotLeftShoulder;
    RectTransform DotRightShoulder;

    RectTransform DotKneeMiddle;
    RectTransform DotHipMiddle;
    RectTransform DotShoulderMiddle;

    RectTransform DotNeck;
    RectTransform DotHead;

    RectTransform KneeBone;
    RectTransform HipBone;

    RectTransform ShoulderBone;
    RectTransform SpineBone;

    RectTransform SpineBone2;
    RectTransform SpineBone3;
    RectTransform SpineBone4;

    RectTransform DotLeftKneeHor;
    RectTransform DotRightKneeHor;

    RectTransform DotLeftHipHor;
    RectTransform DotRightHipHor;

    RectTransform DotLeftShoulderHor;
    RectTransform DotRightShoulderHor;

    RectTransform NeckHor;
    RectTransform HeadHor;

    RectTransform ShoulderLine;
    RectTransform HipLine;
    RectTransform KneeLine;
    RectTransform FeetLine;
    #endregion

    void connectionInfoSet()
    {
        jointFrom = new List<string>();
        jointTo = new List<string>();
        jointFrom.Add("FootLeft"); jointTo.Add("AnkleLeft");
        jointFrom.Add("AnkleLeft"); jointTo.Add("KneeLeft");
        jointFrom.Add("KneeLeft"); jointTo.Add("HipLeft");
        jointFrom.Add("HipLeft"); jointTo.Add("SpineBase");
        jointFrom.Add("FootRight"); jointTo.Add("AnkleRight");
        jointFrom.Add("AnkleRight"); jointTo.Add("KneeRight");
        jointFrom.Add("KneeRight"); jointTo.Add("HipRight");
        jointFrom.Add("HipRight"); jointTo.Add("SpineBase");
        jointFrom.Add("HandTipLeft"); jointTo.Add("HandLeft");
        jointFrom.Add("ThumbLeft"); jointTo.Add("HandLeft");
        jointFrom.Add("HandLeft"); jointTo.Add("WristLeft");
        jointFrom.Add("WristLeft"); jointTo.Add("ElbowLeft");
        jointFrom.Add("ElbowLeft"); jointTo.Add("ShoulderLeft");
        jointFrom.Add("ShoulderLeft"); jointTo.Add("SpineShoulder");
        jointFrom.Add("HandTipRight"); jointTo.Add("HandRight");
        jointFrom.Add("ThumbRight"); jointTo.Add("HandRight");
        jointFrom.Add("HandRight"); jointTo.Add("WristRight");
        jointFrom.Add("WristRight"); jointTo.Add("ElbowRight");
        jointFrom.Add("ElbowRight"); jointTo.Add("ShoulderRight");
        jointFrom.Add("ShoulderRight"); jointTo.Add("SpineShoulder");
        jointFrom.Add("SpineBase"); jointTo.Add("SpineMid");
        jointFrom.Add("SpineMid"); jointTo.Add("SpineShoulder");
        jointFrom.Add("SpineShoulder"); jointTo.Add("Neck");
        jointFrom.Add("Neck"); jointTo.Add("Head");
    }

    void Start()
    {
        SearchPanel = GameObject.Find("SearchPanel");
        LeftPanel = GameObject.Find("LeftPanel");
        RightPanel = GameObject.Find("RightPanel");

        BackupPanel = GameObject.Find("BackupPanel");
        BackupSlider = GameObject.Find("BackupSlider");

        DotLeftKnee = man.transform.Find("KneeLeft").GetComponent<RectTransform>();
        DotRightKnee = man.transform.Find("KneeRight").GetComponent<RectTransform>();
        DotLeftHip = man.transform.Find("HipLeft").GetComponent<RectTransform>();
        DotRightHip = man.transform.Find("HipRight").GetComponent<RectTransform>();
        DotLeftShoulder = man.transform.Find("ShoulderLeft").GetComponent<RectTransform>();
        DotRightShoulder = man.transform.Find("ShoulderRight").GetComponent<RectTransform>();
        DotKneeMiddle = man.transform.Find("KneeMid").GetComponent<RectTransform>();
        DotHipMiddle = man.transform.Find("HipMid").GetComponent<RectTransform>();
        DotShoulderMiddle = man.transform.Find("ShoulderMid").GetComponent<RectTransform>();
        DotNeck = man.transform.Find("Neck").GetComponent<RectTransform>();
        DotHead = man.transform.Find("Head").GetComponent<RectTransform>();

        KneeBone = man.transform.Find("KneeBone").GetComponent<RectTransform>();
        HipBone = man.transform.Find("HipBone").GetComponent<RectTransform>();
        ShoulderBone = man.transform.Find("ShoulderBone").GetComponent<RectTransform>();
        SpineBone = man.transform.Find("SpineBone").GetComponent<RectTransform>();
        SpineBone2 = man.transform.Find("SpineBone2").GetComponent<RectTransform>();
        SpineBone3 = man.transform.Find("SpineBone3").GetComponent<RectTransform>();
        SpineBone4 = man.transform.Find("SpineBone4").GetComponent<RectTransform>();

        Transform up = man.transform.parent.Find("Up");

        DotLeftKneeHor = up.Find("KneeLeft").GetComponent<RectTransform>();
        DotRightKneeHor = up.Find("KneeRight").GetComponent<RectTransform>();
        DotLeftHipHor = up.Find("HipLeft").GetComponent<RectTransform>();
        DotRightHipHor = up.Find("HipRight").GetComponent<RectTransform>();
        DotLeftShoulderHor = up.Find("ShoulderLeft").GetComponent<RectTransform>();
        DotRightShoulderHor = up.Find("ShoulderRight").GetComponent<RectTransform>();
        NeckHor = up.Find("Neck").GetComponent<RectTransform>();
        HeadHor = up.Find("Head").GetComponent<RectTransform>();
        ShoulderLine = up.Find("ShoulderLine").GetComponent<RectTransform>();
        KneeLine = up.Find("KneeLine").GetComponent<RectTransform>();
        HipLine = up.Find("HipLine").GetComponent<RectTransform>();
        FeetLine = up.Find("FeetLine").GetComponent<RectTransform>();

        BackupPanel.SetActive(false);

        linemat = linematp;

        connectionInfoSet();

    }

    public GameObject PNG;
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
            // StartCoroutine(this.downloadPNGfile_fromFTP());
            string httpurl = "http://igrus.mireene.com/image/" + frontData.imagename;
            www = new WWW(httpurl);
            yield return www;
            if (www.error == null)
            {
                PNG.GetComponent<RawImage>().material.mainTexture = www.texture;
                PNG.GetComponent<RawImage>().enabled = false;
                PNG.GetComponent<RawImage>().enabled = true;
            }
            else
            {
                Debug.Log(www.error.ToString());
            }
            // SearchPanel.SetActive(false);
            LeftPanel.SetActive(false);
            RightPanel.SetActive(false);
            BackupPanel.SetActive(true);
        }
        else
        {
            Debug.Log("파일 받기 에러");
        }
    }

    public static void init()
    {
        BackupSlider.GetComponent<Slider>().maxValue = joint.Length - 1;
        if (sphereRoot == null)
            sphereRoot = new GameObject();
        sphereRoot.name = "SphereRoot";
        if (linerendereRoot == null)
            linerendereRoot = new GameObject();
        linerendereRoot.name = "LR_Root";


        sphere = new Dictionary<string, GameObject>();
        //만드는거(초기화)
        linerenderer = new List<GameObject>();

        foreach (KeyValuePair<string, Vector3> i in joint[0])
        {//공만들기
            GameObject tmpgo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            tmpgo.name = i.Key;
            tmpgo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

            sphere.Add(i.Key, tmpgo);
            //if(i.Value.x>-900)
            sphere[i.Key].transform.position = i.Value;
        }
        //ys0625
        foreach (KeyValuePair<string, Vector4> i in jointrot[0])
        {//공각도맞추기
            sphere[i.Key].transform.rotation = new Quaternion(i.Value.x, i.Value.y, i.Value.z, i.Value.w);
        }


        for (int i = 0; i < jointFrom.Count; i++)
        {//선만들기
            GameObject tmpgo = new GameObject();
            tmpgo.AddComponent<LineRenderer>();
            tmpgo.GetComponent<LineRenderer>().material = linemat;
            tmpgo.GetComponent<LineRenderer>().startWidth = 0.1f;
            tmpgo.GetComponent<LineRenderer>().endWidth = 0.1f;

            tmpgo.name = "LR_" + jointFrom[i] + "_" + jointTo[i];
            tmpgo.transform.SetParent(linerendereRoot.transform);
            linerenderer.Add(tmpgo);

        }
        time = 0;
        ct = 0;
    }
    // Update is called once per frame

    static float time = -1;
    static int ct = 0;
    static bool scrollclicked = false;

    public void scrollPlayButton()
    {
        time = 0;
    }
    public void scrollStopButton()
    {
        time = -1;
    }

    public void scrollValueChange(float val)
    {
        ct = (int)val;

        if (scrollclicked == false)
        {
            scrollclicked = true;
            return;
        }
        time = -1;
        foreach (KeyValuePair<string, Vector3> i in joint[ct])
        { //원을 움직임
            if (sphere.ContainsKey(i.Key))
            {
                sphere[i.Key].transform.SetParent(sphereRoot.transform);
                if (sphere[i.Key].transform.position.x < -900)
                {
                    //바로옮김
                    sphere[i.Key].transform.position = i.Value;
                    sphere[i.Key].SetActive(false);
                }
                if (i.Value.x > -900)
                {
                    //점점 다가감
                    sphere[i.Key].transform.position = i.Value;
                    sphere[i.Key].SetActive(true);
                }

            }
            else
            {
                GameObject tmpgo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                tmpgo.name = i.Key;
                tmpgo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                sphere.Add(i.Key, tmpgo);
                //if(i.Value.x>-900)
                sphere[i.Key].transform.position = i.Value;
                //원을 새로 만들어야함
            }

        }
        foreach (KeyValuePair<string, Vector4> i in jointrot[ct]) //ys0625
        {
            if (sphere.ContainsKey(i.Key))
            {
                sphere[i.Key].transform.SetParent(sphereRoot.transform);
                sphere[i.Key].transform.rotation = new Quaternion(i.Value.x, i.Value.y, i.Value.z, i.Value.w);
            }
        }

        for (int i = 0; i < jointFrom.Count; i++)
        { //라인을 움직임

            if (sphere.ContainsKey(jointFrom[i]) && sphere.ContainsKey(jointTo[i]))
            {
                if (sphere[jointFrom[i]].transform.position.x < -900 ||
                    sphere[jointTo[i]].transform.position.x < -900 ||
                    sphere[jointFrom[i]].transform.position == Vector3.zero ||
                    sphere[jointTo[i]].transform.position == Vector3.zero)
                {
                    linerenderer[i].SetActive(false);
                }
                else
                {
                    linerenderer[i].GetComponent<LineRenderer>().SetPosition(0, sphere[jointFrom[i]].transform.position);
                    linerenderer[i].GetComponent<LineRenderer>().SetPosition(1, sphere[jointTo[i]].transform.position);
                    linerenderer[i].SetActive(true);
                }

            }
            else
            {
                linerenderer[i].SetActive(false);
            }
        }
        showUISkeletonPoint();
        showUISkeletonPointHor();
    }

    void Update()
    {
        if (time < 0)
        {
            //init 시작하기전까지 동작하지 않음
            return;
        }

        time += Time.deltaTime;

        if (time > 0.5f)
        {
            time = 0;
            if (joint.Length - 1 > ct)
            {
                ct++;
                BackupSlider.GetComponent<Slider>().value = ct;
                scrollclicked = false;
            }
            else
            {
                //측정 종료 ui가 떠야함
            }
        }
        else
        {
            foreach (KeyValuePair<string, Vector3> i in joint[ct])
            { //원을 움직임
                if (sphere.ContainsKey(i.Key))
                {
                    sphere[i.Key].transform.SetParent(sphereRoot.transform);
                    if (sphere[i.Key].transform.position.x < -900)
                    {
                        //바로옮김
                        sphere[i.Key].transform.position = i.Value;
                        sphere[i.Key].SetActive(false);
                    }
                    if (i.Value.x > -900)
                    {
                        //점점 다가감
                        sphere[i.Key].transform.Translate((i.Value - sphere[i.Key].transform.position) * (Time.deltaTime / 0.5f));
                        sphere[i.Key].SetActive(true);
                    }
                }
                else
                {
                    GameObject tmpgo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    tmpgo.name = i.Key;
                    tmpgo.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    sphere.Add(i.Key, tmpgo);
                    //if(i.Value.x>-900)
                    sphere[i.Key].transform.position = i.Value;
                    //원을 새로 만들어야함
                }

            }
            foreach (KeyValuePair<string, Vector4> i in jointrot[ct]) //ys0625 이거 검증해봐야함
            {
                if (sphere.ContainsKey(i.Key))
                {
                    sphere[i.Key].transform.SetParent(sphereRoot.transform);
                    if (sphere[i.Key].transform.position.x < -900)
                    {
                        //바로옮김
                        sphere[i.Key].transform.rotation = new Quaternion(i.Value.x, i.Value.y, i.Value.z, i.Value.w);
                    }
                    if (i.Value.x > -900)
                    {
                        //점점 다가감
                        sphere[i.Key].transform.Rotate(new Quaternion(i.Value.x, i.Value.y, i.Value.z, i.Value.w).eulerAngles - sphere[i.Key].transform.rotation.eulerAngles * (Time.deltaTime / 0.5f));
                    }
                }
            }

            for (int i = 0; i < jointFrom.Count; i++)
            { //라인을 움직임

                if (sphere.ContainsKey(jointFrom[i]) && sphere.ContainsKey(jointTo[i]))
                {
                    if (sphere[jointFrom[i]].transform.position.x < -900 ||
                        sphere[jointTo[i]].transform.position.x < -900 ||
                        sphere[jointFrom[i]].transform.position == Vector3.zero ||
                        sphere[jointTo[i]].transform.position == Vector3.zero)
                    {
                        linerenderer[i].SetActive(false);
                    }
                    else
                    {
                        linerenderer[i].GetComponent<LineRenderer>().SetPosition(0, sphere[jointFrom[i]].transform.position);
                        linerenderer[i].GetComponent<LineRenderer>().SetPosition(1, sphere[jointTo[i]].transform.position);
                        linerenderer[i].SetActive(true);
                    }

                }
                else
                {
                    linerenderer[i].SetActive(false);
                }
            }
            showUISkeletonPoint();
            showUISkeletonPointHor();
        }
    }

    //한상우 : 앞에서 보는 정면도의 가로값 가중치, 해결 못해서 가중치인 채로 남겨놓음 죄송
    public float Xp = 100;

    //한상우 : 앞에서 보는 정면도를 그림
    void showUISkeletonPoint() //왼쪽의 스켈레톤 모식도를 그려주는 함수
    {
        //중심과 기본이 되는 변수들
        float centerX = sphere["SpineBase"].transform.position.x;

        float foo = 0.0f;

        Xp = man.GetComponent<RectTransform>().rect.width;
        float max = sphere["Head"].transform.position.y;
        max = man.GetComponent<RectTransform>().rect.height * 0.9f / max;

        //밑바닥 부터 시작, 발바닥은 고정 되어 있다고 본다.
        //Knee파트
        float leftKneeX =
            sphere["KneeLeft"].transform.position.x
            - centerX;
        float rightKneeX =
            sphere["KneeRight"].transform.position.x
            - centerX;

        float leftKneeY =
           sphere["KneeLeft"].transform.position.y
           - foo;
        float rightKneeY =
            sphere["KneeRight"].transform.position.y
            - foo;

        DotLeftKnee.anchoredPosition = new Vector2(Xp * leftKneeX, max * leftKneeY);
        DotRightKnee.anchoredPosition = new Vector2(Xp * rightKneeX, max * rightKneeY);

        //DotKneeMiddle파트, 두 Knee 사이의 값을 잡아준다
        float DotKneeMiddleX = (leftKneeX + rightKneeX) / 2;

        float DotKneeMiddleY = (leftKneeY + rightKneeY) / 2;

        DotKneeMiddle.anchoredPosition = new Vector2(Xp * DotKneeMiddleX, max * DotKneeMiddleY);

        //Hip파트
        float leftHipX =
          sphere["HipLeft"].transform.position.x
          - centerX;
        float rightHipX =
            sphere["HipRight"].transform.position.x
            - centerX;

        float leftHipY =
           sphere["HipLeft"].transform.position.y
           - foo;
        float rightHipY =
            sphere["HipRight"].transform.position.y
            - foo;

        DotLeftHip.anchoredPosition = new Vector2(Xp * leftHipX, max * leftHipY);
        DotRightHip.anchoredPosition = new Vector2(Xp * rightHipX, max * rightHipY);

        //DotHipMiddle파트
        float DotHipMiddleX = (leftHipX + rightHipX) / 2;

        float DotHipMiddleY = (leftHipY + rightHipY) / 2;

        DotHipMiddle.anchoredPosition = new Vector2(Xp * DotHipMiddleX, max * DotHipMiddleY);

        //Shoulder파트
        float leftShoulderX =
            sphere["ShoulderLeft"].transform.position.x
            - centerX;
        float rightShoulderX =
            sphere["ShoulderRight"].transform.position.x
            - centerX;

        float leftShoulderY =
           sphere["ShoulderLeft"].transform.position.y
           - foo;
        float rightShoulderY =
            sphere["ShoulderRight"].transform.position.y
            - foo;

        DotLeftShoulder.anchoredPosition = new Vector2(Xp * leftShoulderX, max * leftShoulderY);
        DotRightShoulder.anchoredPosition = new Vector2(Xp * rightShoulderX, max * rightShoulderY);

        //DotShoulderMiddle파트
        float DotShoulderMiddleX = (leftShoulderX + rightShoulderX) / 2;

        float DotShoulderMiddleY = (leftShoulderY + rightShoulderY) / 2;

        DotShoulderMiddle.anchoredPosition = new Vector2(Xp * DotShoulderMiddleX, max * DotShoulderMiddleY);

        //Neck파트
        float neckX = sphere["Neck"].transform.position.x
           - centerX;

        float neckY = sphere["Neck"].transform.position.y
           - foo;

        DotNeck.anchoredPosition = new Vector2(Xp * neckX, max * neckY);

        //Head파트
        float headX = sphere["Head"].transform.position.x
            - centerX;

        float headY = sphere["Head"].transform.position.y
            - foo;

        DotHead.anchoredPosition = new Vector2(Xp * headX, max * headY);

        //선 긋기

        DrawLine(KneeBone, DotLeftKnee, DotRightKnee);
        DrawLine(HipBone, DotLeftHip, DotRightHip);
        DrawLine(ShoulderBone, DotLeftShoulder, DotRightShoulder);

        DrawLine(SpineBone, DotKneeMiddle, DotHipMiddle);
        DrawLine(SpineBone2, DotHipMiddle, DotShoulderMiddle);
        DrawLine(SpineBone3, DotShoulderMiddle, DotNeck);
        DrawLine(SpineBone4, DotNeck, DotHead);
    }

    //한상우 : 위에서 보는 평면도를 그림
    void showUISkeletonPointHor()
    {
        Rect rect = man.transform.parent.Find("Up").GetComponent<RectTransform>().rect;

        //min값과 max값은 모든 값들중 가장 큰 값과 가장 작은 값을 찾아냄, 이렇게하는 이유는 모니터 공간을 벗어나면 안되므로 
        float min, max, height;

        min = Mathf.Min(new float[] { sphere["KneeLeft"].transform.position.z,
            sphere["KneeRight"].transform.position.z,
            sphere["HipLeft"].transform.position.z,
            sphere["HipRight"].transform.position.z,
            sphere["ShoulderLeft"].transform.position.z,
            sphere["ShoulderRight"].transform.position.z,
            sphere["Neck"].transform.position.z,
            sphere["Head"].transform.position.z });

        max = Mathf.Max(new float[] { sphere["KneeLeft"].transform.position.z,
            sphere["KneeRight"].transform.position.z,
            sphere["HipLeft"].transform.position.z,
            sphere["HipRight"].transform.position.z,
            sphere["ShoulderLeft"].transform.position.z,
            sphere["ShoulderRight"].transform.position.z,
            sphere["Neck"].transform.position.z,
            sphere["Head"].transform.position.z });

        height = max - min;
        height = rect.height / height;

        float centerAnkle = ((sphere["AnkleLeft"].transform.position.z + sphere["AnkleRight"].transform.position.z) / 2) - min; //발목 Z값 평균치

        FeetLine.anchoredPosition = new Vector2(0, height * centerAnkle);

        float DotLeftKneeHorZ = sphere["KneeLeft"].transform.position.z - min;
        float DotRightKneeHorZ = sphere["KneeRight"].transform.position.z - min;

        DotLeftKneeHor.anchoredPosition = new Vector2(-0.25f * rect.width, height * DotLeftKneeHorZ);
        DotRightKneeHor.anchoredPosition = new Vector2(0.25f * rect.width, height * DotRightKneeHorZ);

        DrawLine(KneeLine, DotLeftKneeHor, DotRightKneeHor);

        float DotLeftHipHorZ = sphere["HipLeft"].transform.position.z - min;
        float DotRightHipHorZ = sphere["HipRight"].transform.position.z - min;

        DotLeftHipHor.anchoredPosition = new Vector2(-0.125f * rect.width, height * DotLeftHipHorZ);
        DotRightHipHor.anchoredPosition = new Vector2(0.125f * rect.width, height * DotRightHipHorZ);

        DrawLine(HipLine, DotLeftHipHor, DotRightHipHor);

        float DotLeftShoulderHorZ = sphere["ShoulderLeft"].transform.position.z - min;
        float DotRightShoulderHorZ = sphere["ShoulderRight"].transform.position.z - min;

        DotLeftShoulderHor.anchoredPosition = new Vector2(-0.3f * rect.width, height * DotLeftShoulderHorZ);
        DotRightShoulderHor.anchoredPosition = new Vector2(0.3f * rect.width, height * DotRightShoulderHorZ);

        DrawLine(ShoulderLine, DotLeftShoulderHor, DotRightShoulderHor);

        float Neck1HorZ = sphere["Neck"].transform.position.z - min;
        float Head1HorZ = sphere["Head"].transform.position.z - min;

        NeckHor.anchoredPosition = new Vector2(0, height * Neck1HorZ);
        HeadHor.anchoredPosition = new Vector2(0, height * Head1HorZ);
    }

    //한상우 : 두 조인트간의 선을 그려줌, 라인 렌더러 아님!
    void DrawLine(RectTransform imageRectTransform, RectTransform PointA, RectTransform PointB)
    {
        Vector2 differenceVector = PointB.localPosition - PointA.localPosition;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, 5f);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.localPosition = PointA.localPosition;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
}