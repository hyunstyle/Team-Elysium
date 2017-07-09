using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using JsonFx.Json;
using System;
//using Windows.Kinect;
//copyright : teamElysium

public class SkeletonOverlayer : MonoBehaviour
{
    [Tooltip("GUI-texture used to display the color camera feed on the scene background.")]
    public GUITexture backgroundImage;

    [Tooltip("Camera that will be used to overlay the 3D-objects over the background.")]
    public Camera foregroundCamera;

    [Tooltip("Index of the player, tracked by this component. 0 means the 1st player, 1 - the 2nd one, 2 - the 3rd one, etc.")]
    public int playerIndex = 0;

    [Tooltip("Game object used to overlay the joints.")]
    public GameObject jointPrefab;

    [Tooltip("Line object used to overlay the bones.")]
    public LineRenderer linePrefab;
    //public float smoothFactor = 10f;

    public LineRenderer degreeLine;

    LineRenderer firstLine;
    LineRenderer secondLine;
    LineRenderer thirdLine;
    //LineRenderer forthLine;

    //public GUIText debugText;

    private GameObject[] joints = null;
    private LineRenderer[] lines = null;

    private Quaternion initialRotation = Quaternion.identity;

    public MediaPlayerCtrl movie;

    public const int SpineBase = 0;
    public const int SpineMid = 1;
    public const int Neck = 2;
    public const int Head = 3;
    public const int ShoulderLeft = 4;
    public const int ElbowLeft = 5;
    public const int WristLeft = 6;
    public const int HandLeft = 7;
    public const int ShoulderRight = 8;
    public const int ElbowRight = 9;
    public const int WristRight = 10;
    public const int HandRight = 11;
    public const int HipLeft = 12;
    public const int KneeLeft = 13;
    public const int AnkleLeft = 14;
    public const int FootLeft = 15;
    public const int HipRight = 16;
    public const int KneeRight = 17;
    public const int AnkleRight = 18;
    public const int FootRight = 19;
    public const int SpineShoulder = 20;
    public const int HandTipLeft = 21;
    public const int ThumbLeft = 22;
    public const int HandTipRight = 23;
    public const int ThumbRight = 24;

    int frameCount = -1;

    int shoulder;
    int elbow;
    int wrist;
    int thumb;
    int hip;
    int knee;
    int handTip;

    int ScreenShot_State = 0;

    Color standardColor = Color.green;
    Color redColor = Color.red;
    float sh_angle;
    float sh_angle_middle = 0;
    float hh_angle;
    float hh_angle_middle = 0;
    List<float> sh_anglelist;
    List<float> hh_anglelist;


    //bool isDataSending = false;
    bool isMoviePlay = false;
    bool isChecking = false;
    bool isChecked = false;
    bool firstCheck = false;
    bool isRightShoulderHigh = true; // true 는 오른쪽 어깨가 높다 // false 는 왼쪽 어깨가 높다 
    bool isRightHipHigh = true;
    bool isRightKneeHigh = true;
    bool isHeadRight = true;
    bool c_check = false;
    bool hcheck = false;

    bool isJLON = true;

    bool isUploaded = false; // 영상이 성공적으로 업로드되었는지 체크한다. 0702 추가

    float currentAngle = 0;
    float maxAngle = 0.1f;

    float c_time = 0;
    float time = 0f;
    float stateTime = 0f;

    float face_xAngle = 0;
    float face_yAngle = 0;
    float face_zAngle = 0;

    float newface_xAngle = 0;
    float newface_yAngle = 0;
    float newface_zAngle = 0;


    private bool bGotHeadPos = false;
    Vector3 headVectorPos;

    Vector3 firstArrow = Vector3.zero;
    List<string> jsonList = new List<string>();
    List<string> jsonList_face = new List<string>();
    List<int> index = new List<int>();

    /// <김주환> ///UI및 스케줄링
    List<KinectInfo> kinectsInfo = new List<KinectInfo>();

    KinectInfo kinectInfo;

    public static string patientId;
    public static string patientName;
    public static string patientSex;
    public static string patientBirth;
    public static string patientNumber;
    public static int patientAge;

    public static int jointDirection;

    public static string checkdateId;

    string guideText2;
    string guideText3;


    int diagnosisState = 0;
    const int State_Idle = 0;
    const int State_Intro = 1;
    const int State_Intro2 = 12;
    const int State_Guide_UI = 2;
    const int State_Diagnosis = 3;
    const int State_Diagnosis_End = 4;

    public GameObject guideUI;

    public GameObject guideText;  //UI에 뜨는 텍스트들
    public GameObject testCaseText;
    public GameObject nameText;
    public GameObject patientInfoText;

    public GameObject angleValue;
    public GameObject maxAngleValue;
    public GameObject durationValue;

    public GameObject shAngleValue; //임시로 만듬
    public GameObject hhAngleValue;
    public GameObject r_durationValue;

    public GameObject x_angleValue;
    public GameObject y_angleValue;
    public GameObject z_angleValue;

    public GameObject lowUI;
    public GameObject RightUI;
    public GameObject Right_face_UI;

    public GameObject leftui;
    public GameObject leftman;

    public GameObject FaceModel;
    public float smoothFactor = 3f;

    public float verticalOffset = 0f;
    public float modelScaleFactor = 1f;

    string kinectInfoURL = "http://igrus.mireene.com/rom_server/kinectlist.php";
    string patientInfoURL = "http://igrus.mireene.com/rom_server/patientlist.php";
    string kinectInfoDeletURL = "http://igrus.mireene.com/rom_server/kinectdelete.php";
    string checkDateURL = "http://igrus.mireene.com/rom_server/checkdateadd.php";
    string checkTimeURL = "http://igrus.mireene.com/rom_server/checktimeadd.php";
    // TODO 작성해야됨 아직 없음
    string checkTimeFaceURL = "http://igrus.mireene.com/rom_server/checktimefaceadd.php";

    private bool enteredKinectInfo = false;
    private bool enteredCheckDate = false;
    private bool isfaceChecking = false;


    KinectManager manager;


    public void AV_name(string name)
    {
        name = name.Replace(".png", "");
        foregroundCamera.GetComponent<AVProMovieCaptureFromScene>()._forceFilename = name + ".wav";
        foregroundCamera.GetComponent<AVProMovieCaptureFromScene>()._autoFilenamePrefix = name;
        foregroundCamera.GetComponent<AVProMovieCaptureFromScene>()._autoFilenameExtension = "wav";
    }

    // TODO : 동영상 스탑시 프로세스를 많이 먹어서 잠시 멈춤
    IEnumerator stopAV()
    {
        AV_Stop();
        yield return null;
    }

    public void AV_Start()
    {
        foregroundCamera.GetComponent<AVProMovieCaptureGUI>().StartCapture();
    }

    public void AV_Stop()
    {
        foregroundCamera.GetComponent<AVProMovieCaptureGUI>().StopCapture();
    }

    public void deleteFile()
    {
        try
        {
            File.Delete(Directory.GetCurrentDirectory().ToString() + "\\" + PNG_FileName);
        }
        catch(Exception ex)
        {

        }
        File.Delete(Directory.GetCurrentDirectory().ToString() + "\\" + Movie_FileName + ".mp4");
    }
    void Start()
    {
        manager = KinectManager.Instance;


        //4-09 이종원 start
        faceTracking_Start(manager);
        //4-09 이종원 end

        if (manager && manager.IsInitialized())
        {
            int jointsCount = manager.GetJointCount();

            if (jointPrefab)
            {
                // array holding the skeleton joints
                joints = new GameObject[jointsCount];

                for (int i = 0; i < joints.Length; i++)
                {
                    joints[i] = Instantiate(jointPrefab) as GameObject;
                    joints[i].transform.parent = transform;
                    joints[i].name = ((KinectInterop.JointType)i).ToString();
                    joints[i].SetActive(false);
                }
            }

            // array holding the skeleton lines
            lines = new LineRenderer[jointsCount];

            //			if(linePrefab)
            //			{
            //				for(int i = 0; i < lines.Length; i++)
            //				{
            //					lines[i] = Instantiate(linePrefab) as LineRenderer;
            //					lines[i].transform.parent = transform;
            //					lines[i].gameObject.SetActive(false);
            //				}
            //			}
        }

        // always mirrored
        initialRotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));



        if (!foregroundCamera)
        {
            // by default - the main camera
            foregroundCamera = Camera.main;
        }
        RightUI.SetActive(false);
        Right_face_UI.SetActive(false);

        // 2017-3-26 checkdataid 받기
        // 2017-3-26 연속적으로 테스트 하는 경우가 존재하기 때문에 Start에서 다른 곳으로 옴겨야 함. % 경고
        
        // TODO : 사실 큰 문제는 아닐 수 있는데 check data id를 받아와서 동영상과 스크린샷 이름을 정하기 때문에 만약 checkdata 에 아무것도 없다면 문제가 생길 수 있음
        StartCoroutine(receivecheckdataId(checkDateURL));


    }

    // 4-09 이종원 start
    public GUIText debugText;

    private bool isTrackingFace = false;
    private float lastFaceTrackedTime = 0f;

    private Quaternion headRot = Quaternion.identity;
    private bool bGotHeadRot = false;

    private bool isFacetrackingInitialized = false;

    private long primaryUserID = 0;

    private KinectInterop.SensorData sensorData = null;

    private Vector3 headPos = Vector3.zero;

    public float faceTrackingTolerance = 0.5f;

    private static SkeletonOverlayer instance;

    public bool getFaceModelData = false;

    public bool displayFaceRect = false;

    // TODO :: 얼굴이 각도가 정확하게 나올질 않음
    public Quaternion GetHeadRotation(bool bMirroredMovement)
    {
        Vector3 rotAngles = headRot.eulerAngles; // bGotHeadRot ? headRot.eulerAngles : Vector3.zero;

        if (bMirroredMovement)
        {
            rotAngles.x = -rotAngles.x;
            rotAngles.z = -rotAngles.z;
        }
        else
        {
            rotAngles.x = -rotAngles.x;
            rotAngles.y = -rotAngles.y;
        }

        return Quaternion.Euler(rotAngles);
    }

    public Vector3 GetHeadPosition(bool bMirroredMovement)
    {
        Vector3 vHeadPos = headPos; // bGotHeadPos ? headPos : Vector3.zero;

        if (!bMirroredMovement)
        {
            vHeadPos.z = -vHeadPos.z;
        }

        return vHeadPos;
    }



    void Awake()
    {
        instance = this;
    }

    void OnDestroy()
    {
        if (isFacetrackingInitialized && sensorData != null && sensorData.sensorInterface != null)
        {
            // finish face tracking
            sensorData.sensorInterface.FinishFaceTracking();
        }

        //		// clean up
        //		Resources.UnloadUnusedAssets();
        //		GC.Collect();

        isFacetrackingInitialized = false;
        //instance = null;
    }

    public static SkeletonOverlayer Instance
    {
        get
        {
            return instance;
        }
    }

    void faceTracking_Start(KinectManager manager)
    {
        try
        {
            // get sensor data
            if (manager && manager.IsInitialized())
            {
                sensorData = manager.GetSensorData();
            }

            if (sensorData == null || sensorData.sensorInterface == null)
            {
                throw new Exception("Face tracking cannot be started, because KinectManager is missing or not initialized.");
            }

            if (debugText != null)
            {
                debugText.text = "Please, wait...";
            }

            // ensure the needed dlls are in place and face tracking is available for this interface
            bool bNeedRestart = false;
            if (sensorData.sensorInterface.IsFaceTrackingAvailable(ref bNeedRestart))
            {
                if (bNeedRestart)
                {
                    KinectInterop.RestartLevel(gameObject, "FM");
                    return;
                }
            }
            else
            {
                string sInterfaceName = sensorData.sensorInterface.GetType().Name;
                throw new Exception(sInterfaceName + ": Face tracking is not supported!");
            }

            //Initialize the face tracker

            if (!sensorData.sensorInterface.InitFaceTracking(getFaceModelData, displayFaceRect))
            {
                throw new Exception("Face tracking could not be initialized.");
            }

            isFacetrackingInitialized = true;

            //DontDestroyOnLoad(gameObject);

            if (debugText != null)
            {
                debugText.text = "Ready.";
            }
        }
        catch (DllNotFoundException ex)
        {
            Debug.LogError(ex.ToString());
            if (debugText != null)
                debugText.text = "Please check the Kinect and FT-Library installations.";
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.ToString());
            if (debugText != null)
                debugText.text = ex.Message;
        }
    }

    // 얼굴 움직이기
    void faceModel_moving()
    {
        Vector3 newPosition = this.GetHeadPosition(true);

        Quaternion newRotation = this.GetHeadRotation(true);

        Vector3 addedRotation = newPosition.z != 0f ? new Vector3(Mathf.Rad2Deg * (Mathf.Tan(newPosition.y) / newPosition.z), Mathf.Rad2Deg * (Mathf.Tan(newPosition.x) / newPosition.z), 0) : Vector3.zero;

        addedRotation.x = newRotation.eulerAngles.x + addedRotation.x;
        addedRotation.y = newRotation.eulerAngles.y + addedRotation.y;
        addedRotation.z = newRotation.eulerAngles.z + addedRotation.z;

        newRotation = Quaternion.Euler(addedRotation.x, addedRotation.y, addedRotation.z);

        if (smoothFactor != 0f)
            FaceModel.transform.rotation = Quaternion.Slerp(FaceModel.transform.rotation, newRotation, smoothFactor * Time.deltaTime);
        else
            FaceModel.transform.rotation = newRotation;



        // vertical offet
        if (verticalOffset != 0f)
        {
            // add the vertical offset
            Vector3 dirHead = new Vector3(0, verticalOffset, 0);
            dirHead = FaceModel.transform.InverseTransformDirection(dirHead);
            newPosition += dirHead;
        }

        // set the position
        /*if (false)
        {
            if (smoothFactor != 0f)
                FaceModel.transform.position = Vector3.Lerp(FaceModel.transform.position, newPosition, smoothFactor * Time.deltaTime);
            else
                FaceModel.transform.position = newPosition;
        }*/
        // scale factor
        if (FaceModel.transform.localScale.x != modelScaleFactor)
        {
            FaceModel.transform.localScale = new Vector3(modelScaleFactor, modelScaleFactor, modelScaleFactor);
        }
    }

    // TODO :: 최대 각도일때 이미지 찍는 것까지 넣어야됨
    // TODO : 얼굴 돌아가는 각도가 제대로 나오지 않음
    void faceTracking_update()  // 상채가 보여야지 얼굴을 인식함
    {
        Right_face_UI.SetActive(true);
        RightUI.SetActive(false);
        //leftman.SetActive(false);
        //leftui.SetActive(false);
        FaceModel.SetActive(true);

        lowUI.SetActive(true);
        JL_turnOff();
        isfaceChecking = true;

        if (isFacetrackingInitialized)
        {
            KinectManager kinectManager = KinectManager.Instance;
            if (kinectManager && kinectManager.IsInitialized())
            {
                primaryUserID = kinectManager.GetUserIdByIndex(playerIndex);
            }

            // update the face tracker
            isTrackingFace = false;

            if (sensorData.sensorInterface.UpdateFaceTracking())
            {
                // estimate the tracking state
                isTrackingFace = sensorData.sensorInterface.IsFaceTracked(primaryUserID);

                if (!isTrackingFace && (Time.realtimeSinceStartup - lastFaceTrackedTime) <= faceTrackingTolerance)
                {
                    // allow tolerance in tracking
                    isTrackingFace = true;
                }

                // get the facetracking parameters
                if (isTrackingFace)
                {
                    lastFaceTrackedTime = Time.realtimeSinceStartup;


                    bGotHeadRot = sensorData.sensorInterface.GetHeadRotation(primaryUserID, ref headRot);

                    bGotHeadPos = sensorData.sensorInterface.GetHeadPosition(primaryUserID, ref headPos);

                    //4-23 이종원 start
                    // 얼굴의 각도 뽑아오기 x축은 상하, y축은 좌우 ,z 축은 도리도리 
                    // getHeadRotation 안의 bool값 파라미터 true 하면 각도가 반전되서 나옴
                    /*고개를 왼쪽으로 꺽으면 z 값 20도 쯤
                      반대는 350도쯤
                      머리를 아래로 숙이면 x 값이 20도쯤 
                      반대는 350도 쯤
                      머리를 오른쪽으로 돌리면 y 값이 20도 쯤
                      반대는 350도 쯤
                    */
                    //4-23 이종원  end
                    Quaternion vec = this.GetHeadRotation(false);

                    newface_xAngle = vec.eulerAngles.x;
                    newface_yAngle = vec.eulerAngles.y;
                    newface_zAngle = vec.eulerAngles.z;

                    face_xAngle = (face_xAngle + newface_xAngle) / 2;
                    face_yAngle = (face_yAngle + newface_yAngle) / 2;
                    face_zAngle = (face_zAngle + newface_zAngle) / 2;

                    faceModel_moving();

                    c_time += Time.deltaTime;
                    // 진단 중일때만 실행
                    // 0.5초 마다 관절 좌표 저장하기 위함
                    if (isChecking && c_time > 0.5f)
                    {
                        jsonList_face.Add("");
                        frameCount++;
                        c_time = 0;
                        c_check = true;
                        faceData();
                    }

                    // 4-30 이종원 start
                    // 최대 각도 추출을 위한 기반
                    // 각 x,y,z 마다 2개씩 총 6개의 진단을 나눌것

                    // joint Direction 90로테이션 100플렉션 110어브덕션


                    // x 어브덕션
                    // y 로테이션
                    // z 플렉션
                    // 아래로 숙일때
                    durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

                    time += Time.deltaTime;
                    float nowangle;

                    // TODO :: 뚝배기 각도가 제대로 안나온다야! 뚝배기 좀 잘 챙겨라
                    switch (jointDirection)
                    {
                        case 92:
                            // 오른쪽으로 고개를 돌릴때
                            if (face_yAngle > 180)
                                break;
                            Debug.Log("고개를 오른쪽으로 돌렸다");

                            if (maxAngle < face_yAngle)
                            {
                                maxAngle = face_yAngle;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);


                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                isChecked = true;
                            }

                            break;

                        case 91:
                            // 왼쪽으로 고개를 돌릴때
                            nowangle = 360 - face_yAngle;
                            if (nowangle > 180)
                                break;
                            angleValue.GetComponent<Text>().text = nowangle.ToString();

                            Debug.Log("고개를 왼쪽으로 돌렸다");
                            if (maxAngle < nowangle)
                            {
                                maxAngle = nowangle;
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0f;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);

                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                isChecked = true;
                            }
                            break;
                        case 101:
                            if (face_xAngle > 180)
                                break;
                            Debug.Log("고개를 아래로 숙였다");

                            if (maxAngle < face_xAngle)
                            {
                                maxAngle = face_xAngle;
                                angleValue.GetComponent<Text>().text = face_xAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);
                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                isChecked = true;
                            }
                            break;
                        case 102:
                            // 위로 올릴떄
                            nowangle = 360 - face_xAngle;
                            if (nowangle > 180)
                                break;
                            Debug.Log("고개를 위로 올렸다");

                            if (maxAngle < nowangle)
                            {
                                maxAngle = nowangle;
                                angleValue.GetComponent<Text>().text = face_xAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);

                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                isChecked = true;
                            }

                            break;
                        case 111:
                            // 고개를 왼쪽으로 꺾을 때
                            if (face_zAngle > 180)
                                break;
                            Debug.Log("고개를 왼쪽으로 꺾었다");
                            if (maxAngle < face_zAngle)
                            {
                                maxAngle = face_zAngle;
                                angleValue.GetComponent<Text>().text = face_zAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);

                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                isChecked = true;
                            }

                            break;
                        case 112:
                            // 고개를 오른쪽으로 꺾을 떄
                            nowangle = 360 - face_zAngle;
                            if (nowangle > 180)
                                break;
                            Debug.Log("고개를 오른쪽으로 꺾었다");

                            if (maxAngle < nowangle)
                            {
                                maxAngle = nowangle;
                                angleValue.GetComponent<Text>().text = face_zAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                                time = 0;
                                durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";
                            }

                            if (time >= 3.0f)
                            {
                                FileNameSetting();
                                Debug.Log(checkdateId);
                                Application.CaptureScreenshot("" + PNG_FileName);

                                maxAngle = 0.1f;
                                time = 0f;
                                angleValue.GetComponent<Text>().text = face_yAngle.ToString();
                                maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                            }

                            break;
                        default:
                            break;
                    }


                    // 4-30 이종원 end

                    x_angleValue.GetComponent<Text>().text = string.Format("{0:F1}", face_xAngle) + "";
                    y_angleValue.GetComponent<Text>().text = string.Format("{0:F1}", face_yAngle) + "";
                    z_angleValue.GetComponent<Text>().text = string.Format("{0:F1}", face_zAngle) + "";
                }
            }
        }
    }



    // 4-09 이종원 end
    // 4-23 이종원 start
    // face checking 할때에 사람에 붙어있는 오브젝트들 껐다가 키기
    void JL_turnOff()
    {
        isJLON = false;
    }

    void JL_turnOn()
    {
        isJLON = true;
    }
    // 4-23 이종원 end


    void textTrans(int jointDir)   // 화면 왼쪽 위에 무슨 측정을 하고 있는지 텍스트 변경
    {
        switch (jointDir)
        {
            case 11:
                testCaseText.GetComponent<Text>().text = "Shoulder Flexion Left";
                guideText2 = "왼쪽 어깨 각도 측정을 시작하겠습니다.";
                guideText3 = "왼쪽 팔을 천천히 옆으로 최대한 들어주세요";
                break;

            case 12:
                testCaseText.GetComponent<Text>().text = "Shoulder Flexion Right";
                guideText2 = "오른쪽 어깨 각도 측정을 시작하겠습니다.";
                guideText3 = "오른쪽 팔을 천천히 옆으로 최대한 들어주세요";
                break;

            case 21:
                testCaseText.GetComponent<Text>().text = "Shoulder Abduction Left";
                guideText2 = "Left shoulder abduction Start .";
                guideText3 = "Left shoulder abduction doing";
                break;
            case 22:
                testCaseText.GetComponent<Text>().text = "Shoulder Abduction Right";
                guideText2 = "Right shoulder abduction Start.";
                guideText3 = "Right shoulder abduction Doing";
                break;
            // shoulder Rotation
            case 31:
                testCaseText.GetComponent<Text>().text = "Shoulder Rotation Left";
                guideText2 = "Left shoulder Rotation Start";
                guideText3 = "Left shoulder Rotation doing";
                break;
            case 32:
                testCaseText.GetComponent<Text>().text = "Shoulder Rotation Right";
                guideText2 = "Right shoulder Rotation Start";
                guideText3 = "Right shoulder Rotation doing";
                break;
            // elbow Flexion
            case 41:
                testCaseText.GetComponent<Text>().text = "Elbow Flexion Left";
                guideText2 = "Left elbow Flexion Start";
                guideText3 = "Left elbow Flexion Doing";
                break;
            case 42:
                testCaseText.GetComponent<Text>().text = "Elbow Flexion Right";
                guideText2 = "Right elbow Flexion Start";
                guideText3 = "Right elbow Flexion Doing";
                break;
            // elbow Pronation
            case 51:
                testCaseText.GetComponent<Text>().text = "Elbow Pronation Left";
                guideText2 = "Left elbow Pronation Start";
                guideText3 = "Left elbow Pronation Doing";
                break;
            case 52:
                testCaseText.GetComponent<Text>().text = "Elbow Pronation Right";
                guideText2 = "Right Elbow Pronation Start";
                guideText3 = "Right Elbow Pronation Doing";
                break;
            // hip Flexion
            case 71:
                testCaseText.GetComponent<Text>().text = "Hip Flexion Left";
                guideText2 = "Left Hip Flexion Start";
                guideText3 = "Left HIp Flexion Doing";
                break;
            case 72:
                testCaseText.GetComponent<Text>().text = "Hip Flexion Right";
                guideText2 = "Right Hip Flexion Start";
                guideText3 = "Right Hip Flexion Doing";
                break;

            case 201:
                testCaseText.GetComponent<Text>().text = "Shoulder Pose Estimation";
                guideText2 = "Shoulder Pose Estimation Start";
                guideText3 = "Shoulder Pose Estimation Doing";
                break;
            case 91:
                testCaseText.GetComponent<Text>().text = "Face Checking 목 로테이션 왼쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            case 92:
                // 목 로테이션
                testCaseText.GetComponent<Text>().text = "Face Checking 목 로테이션 오른쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            case 101:
                testCaseText.GetComponent<Text>().text = "Face Checking 목 플렉션 왼쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            case 102:
                // 목 플렉션
                testCaseText.GetComponent<Text>().text = "Face Checking 목 플렉션 오른쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            case 111:
                testCaseText.GetComponent<Text>().text = "Face Checking 목 어브덕션 왼쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            case 112:
                // 목 어브덕션
                testCaseText.GetComponent<Text>().text = "Face Checking 목 어브덕션 오른쪽";
                guideText2 = "Face Checking Start";
                guideText3 = "Face Checking Doing";
                break;
            default:
                break;
        }
    }

    void jointSelect(int jointDir) // 반짝이고 색바꿀 관절들 jointDirection 기반으로 선택
    {
        switch (jointDir)
        {
            case 11:
            case 21:
                index.Add(ShoulderLeft);
                index.Add(ElbowLeft);
                index.Add(WristLeft);
                index.Add(HandLeft);
                index.Add(HandTipLeft);
                index.Add(ThumbLeft);
                break;
            case 12:
            case 22:
                index.Add(ShoulderRight);
                index.Add(ElbowRight);
                index.Add(WristRight);
                index.Add(HandRight);
                index.Add(HandTipRight);
                index.Add(ThumbRight);
                break;
            // shoulder Rotation
            case 31:
                index.Add(ElbowLeft);
                index.Add(WristLeft);
                index.Add(HandLeft);
                index.Add(HandTipLeft);
                index.Add(ThumbLeft);
                break;
            case 32:
                index.Add(ElbowRight);
                index.Add(WristRight);
                index.Add(HandRight);
                index.Add(HandTipRight);
                index.Add(ThumbRight);
                break;
            // elbow Flexion
            case 41:
                index.Add(ElbowLeft);
                index.Add(WristLeft);
                index.Add(HandLeft);
                index.Add(HandTipLeft);
                index.Add(ThumbLeft);
                break;
            case 42:
                index.Add(ElbowRight);
                index.Add(WristRight);
                index.Add(HandRight);
                index.Add(HandTipRight);
                index.Add(ThumbRight);
                break;
            // elbow Pronation
            case 51:
                index.Add(ElbowLeft);
                index.Add(WristLeft);
                index.Add(HandLeft);
                index.Add(HandTipLeft);
                index.Add(ThumbLeft);
                break;
            case 52:
                index.Add(ElbowRight);
                index.Add(WristRight);
                index.Add(HandRight);
                index.Add(HandTipRight);
                index.Add(ThumbRight);
                break;
            // hip Flexion
            case 71:
                index.Add(HipLeft);
                index.Add(KneeLeft);
                break;
            case 72:
                index.Add(HipRight);
                index.Add(KneeRight);
                break;

            case 201:
                index.Add(ShoulderRight);
                index.Add(ShoulderLeft);
                index.Add(SpineShoulder);
                index.Add(HipRight);
                index.Add(HipLeft);
                break;
            default:
                //Debug.Log("NOPE");
                break;
        }
    }

    void glow_Color()
    {
        for (int i = 0; i < index.Count; i++)
        {
            int f_index = index[i];
            joints[f_index].GetComponent<Renderer>().material.color = redColor;
        }
    } // 관절 색 바꾸기

    void returnColor()
    {
        if (index.Count > 0)
        {
            for (int i = 0; i < index.Count; i++)
            {
                joints[index[i]].GetComponent<Renderer>().material.color = standardColor;
            }
        }
        index.Clear();
    } // 관절 색 되돌리기

    public void jointData(int joint)
    {
        Dictionary<string, float> dic = new Dictionary<string, float>();

        // 05-21 이종원
        if (joints[joint].activeSelf)
        {
            dic.Add(joints[joint].name.ToString() + "_x", joints[joint].transform.position.x);
            dic.Add(joints[joint].name.ToString() + "_y", joints[joint].transform.position.y);
            dic.Add(joints[joint].name.ToString() + "_z", joints[joint].transform.position.z);
            dic.Add(joints[joint].name.ToString() + "_r_x", joints[joint].transform.rotation.x);
            dic.Add(joints[joint].name.ToString() + "_r_y", joints[joint].transform.rotation.y);
            dic.Add(joints[joint].name.ToString() + "_r_z", joints[joint].transform.rotation.z);
            dic.Add(joints[joint].name.ToString() + "_r_w", joints[joint].transform.rotation.w);
        }
        else
        {
            dic.Add(joints[joint].name.ToString() + "_x", -999);
            dic.Add(joints[joint].name.ToString() + "_y", -999);
            dic.Add(joints[joint].name.ToString() + "_z", -999);
            dic.Add(joints[joint].name.ToString() + "_r_x", -999);
            dic.Add(joints[joint].name.ToString() + "_r_y", -999);
            dic.Add(joints[joint].name.ToString() + "_r_z", -999);
            dic.Add(joints[joint].name.ToString() + "_r_w", -999);
        }
        // 05-21 이종원

        foreach (KeyValuePair<string, float> items in dic)
        {
            //Debug.Log("func jsonCount " + jsonList.Count);
            //Debug.Log("func framCount " + frameCount);
            jsonList[frameCount] += items.Key + " : " + items.Value + "\n";
        }
        if (joint == 24)
            c_check = false;
    } // 관절 위치 저장하는 함수

    public void faceData()
    {
        Dictionary<string, float> dic = new Dictionary<string, float>();

        dic.Add("headRot_x", face_xAngle);
        dic.Add("headRot_y", face_yAngle);
        dic.Add("headRot_z", face_zAngle);

        foreach (KeyValuePair<string, float> items in dic)
        {
            //Debug.Log("FrameCount " + frameCount);
            //Debug.Log("jsonList " + jsonList_face.Count);
            jsonList_face[frameCount] += items.Key + " : " + items.Value + "\n";
        }
        c_check = false;
    }

    void moviePlay(int jointDirection)
    {
        switch (jointDirection)
        {
            // 불러올 동영상의 이름 선택 (streaming Assets 에 있는 동영상 이름만 가능함 확장자명 필수)
            case 11:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 12:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 21:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 22:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 31:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 32:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 41:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 42:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 51:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 52:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 71:
                movie.m_strFileName = "EasyMovieTexture.mp4";
                break;
            case 72:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 201:
                movie.m_strFileName = "shoulderhltemp.mp4";
                break;
            case 91:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 92:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 101:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 102:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 111:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
            case 112:
                movie.m_strFileName = "ed_1024_512kb.mp4";
                break;
        }
        movie.gameObject.SetActive(true);
        movie.Load(movie.m_strFileName);
    } // 재생할 영상 선택, setactive = true, 재생

    void movieStop() // 영상 정지 및 setactive = false
    {
        movie.Stop();
        movie.UnLoad();
        movie.gameObject.SetActive(false);
    }

    bool isUserStandRight()
    {
        Vector3 shoulderLine = joints[ShoulderLeft].transform.position - joints[ShoulderRight].transform.position;
        Vector3 hipLine = joints[HipLeft].transform.position - joints[HipRight].transform.position;
        Vector3 spineLine = joints[SpineShoulder].transform.position - joints[SpineBase].transform.position;

        Vector3 crossOne = -1 * Vector3.Cross(shoulderLine, spineLine).normalized;
        Vector3 crossTwo = -1 * Vector3.Cross(hipLine, spineLine).normalized;
        //Debug.Log("CrossONe NOM " + crossOne.normalized);
        //Debug.Log("CrossTwo NOM " + crossTwo.normalized);
        //Debug.Log("crossOne " + crossOne.x +" " + crossOne.y + " " + crossOne.z);
        //Debug.Log("crossTwo " + crossTwo.x + " " + crossTwo.y + " " + crossTwo.z);

        if (crossOne.z > 0.99 && crossOne.z < 1.01 && crossTwo.z > 0.99 && crossTwo.z < 1.01)
            return true;
        else
            return false;
    } // 정면인 상태인가 // 현재 update에 적용되어 있지 않음


    void horizon_ShoulderLine() // 어깨 선이 지면과 어느 정도 각도로 벌어져 있는가 // isRightShoulderHigh 변수로 어느 어깨가 높은지 판단 true = 오른쪽이 높다 false = 왼쪽이 높다
    {
        hcheck = true;

        Vector3 headPosition = joints[Head].transform.position;
        Vector3 neckPosition = joints[Neck].transform.position;

        Vector3 rightShoulder = joints[ShoulderRight].transform.position;
        Vector3 leftShoulder = joints[ShoulderLeft].transform.position;

        Vector3 rightHip = joints[HipRight].transform.position;
        Vector3 leftHip = joints[HipLeft].transform.position;

        Vector3 rightKnee = joints[KneeRight].transform.position;
        Vector3 leftKnee = joints[KneeLeft].transform.position;

        firstLine = drawDegreeLine(firstLine, rightShoulder, leftShoulder);
        secondLine = drawDegreeLine(secondLine, rightHip, leftHip);
        thirdLine = drawDegreeLine(thirdLine, rightKnee, leftKnee);
        //forthLine = drawDegreeLine(forthLine, rightFoot, leftFoot);

        firstLine.gameObject.SetActive(true);
        secondLine.gameObject.SetActive(true);
        thirdLine.gameObject.SetActive(true);


        if (joints != null)
        {
            RightUI.SetActive(true);
            lowUI.SetActive(false);
            Right_face_UI.SetActive(false);

            Vector3 shoulderLine = leftShoulder - rightShoulder;
            Vector3 hipLine = leftHip - rightHip;
            Vector3 kneeLine = leftKnee - rightKnee;
            headPosition.z = 0;
            neckPosition.z = 0;
            Vector3 headLine = headPosition - neckPosition;
            time += Time.deltaTime;

            float angle = Vector3.Angle(Vector3.left, shoulderLine);
            float angle2 = Vector3.Angle(Vector3.left, hipLine);
            float angle3 = Vector3.Angle(Vector3.left, kneeLine);
            float angle4 = Vector3.Angle(Vector3.up, headLine);
            //float angle4 = Vector3.Angle(Vector3.left, footLine);

            r_durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            // shoulder angle
            if (angle > 90)
            {
                sh_angle = 180 - angle;
                angle = 180 - angle;
            }
            else
            {
                sh_angle = angle;
            }

            // hip angle
            if (angle2 > 90)
            {
                hh_angle = 180 - angle2;
                angle2 = 180 - angle2;
            }
            else
            {
                hh_angle = angle2;
            }

            // knee angle
            if (angle3 > 90)
            {
                angle3 = 180 - angle3;
            }
            
            sh_anglelist.Add(sh_angle);
            shAngleValue.GetComponent<Text>().text = string.Format("{0:F1}", sh_angle) + "";
            hh_anglelist.Add(hh_angle);
            hhAngleValue.GetComponent<Text>().text = string.Format("{0:F1}", hh_angle) + "";


            if (rightShoulder.y > leftShoulder.y)
            {
                // 오른쪽 어깨가 높다
                isRightShoulderHigh = true;
            }
            else
            {
                // 왼쪽 어깨가 높다
                isRightShoulderHigh = false;
            }

            if (rightHip.y > leftHip.y)
            {
                // 오른쪽 엉덩이가 높다
                isRightHipHigh = true;
            }
            else
            {
                // 왼쪽 엉덩이가 높다
                isRightHipHigh = false;
            }
            if (rightKnee.y > leftKnee.y)
            {
                // 오른쪽 무릎이 높다
                isRightKneeHigh = true;
            }
            else
            {
                // 왼쪽 무릎이 높다
                isRightKneeHigh = false;
            }
            if(headPosition.x>neckPosition.x)
            {
                // 오른쪽으로 머리를 기울임
                isHeadRight = true;
            }
            else
            {
                isHeadRight = false;
            }

            horizontalUI(angle, angle2, angle3, angle4);


            if (time > 20.0f)
            {
                // 보낼 중앙값 데이터 찾기
                sh_anglelist.Sort();
                sh_angle_middle = sh_anglelist[sh_anglelist.Count / 2];

                // 보낼 중앙값 데이터 찾기
                hh_anglelist.Sort();
                hh_angle_middle = hh_anglelist[hh_anglelist.Count / 2];

                maxAngle = 0.1f;
                time = 0f;
                isChecked = true;
                RightUI.SetActive(false);
                lowUI.SetActive(true);
                Right_face_UI.SetActive(false);
                firstLine.gameObject.SetActive(false);
                secondLine.gameObject.SetActive(false);
                thirdLine.gameObject.SetActive(false);
               // forthLine.gameObject.SetActive(false);
                //Debug.Log("Time Clear");
            }

        }
    }

    public GameObject shoulderAngle;
    public GameObject hipAngle;
    public GameObject kneeAngle;
    public GameObject headAngle;

    public Text text1;
    public Text text2;
    public Text text3;
    public Text text4;

    public void horizontalUI(float angle1/*shoulder*/, float angle2/*hip*/, float angle3/*knee*/, float angle4 /*head*/)
    {
        Quaternion rotation = new Quaternion();

        if (isRightShoulderHigh)
        {
            rotation = Quaternion.Euler(0, 0, angle1);
            shoulderAngle.transform.rotation = rotation;
            text1.text = angle1.ToString();
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, -angle1);
            shoulderAngle.transform.rotation = rotation;
            text1.text = "-" + angle1.ToString();
        }

        if (isRightHipHigh)
        {
            rotation = Quaternion.Euler(0, 0, angle2);
            hipAngle.transform.rotation = rotation;
            text2.text = angle2.ToString();
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, -angle2);
            hipAngle.transform.rotation = rotation;
            text2.text = "-" + angle2.ToString();
        }

        if (isRightKneeHigh)
        {
            rotation = Quaternion.Euler(0, 0, angle3);
            kneeAngle.transform.rotation = rotation;
            text3.text = angle3.ToString();
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, -angle3);
            kneeAngle.transform.rotation = rotation;
            text3.text = "-" + angle3.ToString();
        }


        if (isHeadRight)
        {
            rotation = Quaternion.Euler(0, 0, angle4);
            headAngle.transform.rotation = rotation;
            text4.text = angle4.ToString();
        }
        else
        {
            rotation = Quaternion.Euler(0, 0, -angle4);
            headAngle.transform.rotation = rotation;
            text4.text = "-"+angle4.ToString();
        }
        //if (isRightFootHigh)
        //{
        //    rotation = Quaternion.Euler(0, 0, angle4);
        //    footAngle.transform.rotation = rotation;
        //    text4.text = angle4.ToString();
        //}
        //else
        //{
        //    rotation = Quaternion.Euler(0, 0, -angle4);
        //    footAngle.transform.rotation = rotation;
        //    text4.text = "-" + angle4.ToString();

        //}
    }

    // TODO : 논란이 있으니 성수형에게
    void shoulderRotation(int Check)
    {
        switch (Check)
        {
            case 32:
                //Debug.Log("Right");
                wrist = WristRight;
                elbow = ElbowRight;
                shoulder = ShoulderRight;
                break;
            case 31:
                //Debug.Log("Left");
                wrist = WristLeft;
                elbow = ElbowLeft;
                shoulder = ShoulderLeft;
                break;
            default:
                break;
        }

        Vector3 wristJoint = joints[wrist].transform.position;
        Vector3 shoulderJoint = joints[shoulder].transform.position;
        Vector3 elbowJoint = joints[elbow].transform.position;

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            // 어깨와 팔꿈치를 선으로 이어서 각도 재기
            if (shoulderJoint != Vector3.zero && elbowJoint != Vector3.zero && wristJoint != Vector3.zero)
            {
                Vector3 shoulderToElbow = elbowJoint - shoulderJoint;
                Vector3 elbowToWrist = wristJoint - elbowJoint;

                // 어깨부터 팔꿈치를 이은 선이 몸의 중심과 85도에서 95사이일때
                if (Vector3.Angle(Vector3.down, shoulderToElbow) < 95 && Vector3.Angle(Vector3.down, shoulderToElbow) > 85)
                {
                    //현재 각도
                    currentAngle = Vector3.Angle(Vector3.up, elbowToWrist);
                    //Debug.Log(currentAngle);
                    // ui에 현재 각도 표시
                    angleValue.GetComponent<Text>().text = currentAngle.ToString();


                    if (maxAngle < currentAngle)
                    {
                        // 최대 각도 갱신
                        maxAngle = currentAngle;
                        //Debug.Log(maxAngle);
                        maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                        //Debug.Log("maxAngle = " + maxAngle);

                        // 3초 제한 초기화 (임시)
                        time = 0f;
                        // Debug.Log("3 Seconds Clear up");
                    }

                    // 3초 시간 제한 초과
                    if (time > 3.0f)
                    {
                        if (shoulder == ShoulderRight)
                        {
                            //Debug.Log("Right Shoulder Rotation Checked");
                            //maxAngleRightShoulder = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        else if (shoulder == ShoulderLeft)
                        {
                            //Debug.Log("Left Shoulder Rotation Checked");
                            //maxAngleLeftShoulder = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        //Debug.Log("Time Clear");
                    }
                }
                else
                {
                    //Debug.Log("어깨부터 팔꿈치를 이은 선이 몸과 90도가 되게 맞춰주세요");
                }
                // Debug.Log("currentAngle = " + currentAngle);
            }
        }



    } // 팔을 들었을때 몸과 든 팔의 각도가 85에서 95 사이가 아니면 각도 측정 못함

    string PNG_FileName;

    void shoulderAbduction_Flexion(int Check)
    {
        switch (Check)
        {
            case 12:
            case 22:
                //Debug.Log("Right shoulder");
                shoulder = ShoulderRight;
                elbow = ElbowRight;
                break;
            case 11:
            case 21:
                //Debug.Log("Left shoulder");
                shoulder = ShoulderLeft;
                elbow = ElbowLeft;
                break;
            default:
                break;
        }
        // 어깨 팔꿈치 위치 잡기
        Vector3 shoulderJoint = joints[shoulder].transform.position;
        Vector3 elbowJoint = joints[elbow].transform.position;

        // 6-20

        if (firstLine != null)
            firstLine.gameObject.SetActive(true);
        if (secondLine != null)
            secondLine.gameObject.SetActive(true);

        firstLine = drawDegreeLine(firstLine, shoulderJoint, shoulderJoint + 3 * (elbowJoint - shoulderJoint));
        secondLine = drawDegreeLine(secondLine, shoulderJoint, new Vector3(shoulderJoint.x, shoulderJoint.y - 30, shoulderJoint.z));


        // 6-20

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            // 어깨와 팔꿈치를 선으로 이어서 각도 재기
            if (shoulderJoint != Vector3.zero && elbowJoint != Vector3.zero)
            {
                Vector3 arrow = elbowJoint - shoulderJoint;

                //현재 각도
                currentAngle = Vector3.Angle(arrow, Vector3.down);
                //Debug.Log(currentAngle);
                angleValue.GetComponent<Text>().text = currentAngle.ToString();

                if (maxAngle < currentAngle)
                {
                    // ScreenShot 찍을 상태임을 알려줌
                    ScreenShot_State = 0;

                    // 최대 각도 갱신
                    maxAngle = currentAngle;
                    //Debug.Log(maxAngle);
                    maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                    //Debug.Log("maxAngle = " + maxAngle);

                    // 3초 제한 초기화 (임시)
                    time = 0f;
                    // Debug.Log("3 Seconds Clear up");
                }
                else if ((maxAngle >= currentAngle) && (ScreenShot_State == 0))
                {
                    // 스크린샷 찍기
                    // 페이션트 ID, CheckData ID, 만든 날짜
                    // 2017-3-26
                    FileNameSetting();
                    Debug.Log(checkdateId);
                    Application.CaptureScreenshot("" + PNG_FileName);
                    ScreenShot_State = 1;
                }
                // 3초 시간 제한 초과
                if (time > 3.0f)
                {
                    if (shoulder == ShoulderRight)
                    {
                        //Debug.Log("Right shoulder Checked");
                        //maxAngleRightShoulder = maxAngle;
                        maxAngle = 0.1f;
                        time = 0f;
                        isChecked = true;
                    }
                    else if (shoulder == ShoulderLeft)
                    {
                        //Debug.Log("Left Shoulder Checked");
                        //maxAngleLeftShoulder = maxAngle;
                        maxAngle = 0.1f;
                        time = 0f;
                        isChecked = true;
                    }
                    //Debug.Log("Time Clear");

                    firstLine.gameObject.SetActive(false);
                    secondLine.gameObject.SetActive(false);
                }
                // Debug.Log("currentAngle = " + currentAngle);
            }
        }
    }

    void elbowPronation(int Check)   // 손바닥 뒤집기 측정
    {
        // diagnosisState 변수로 왼쪽 오른쪽을 판단할것
        // TODO : 변수 숫자 수정해야함// if elseif 문으로 교체할 가능성 있음
        switch (Check)
        {
            case 52:
                // Debug.Log("Right palm");
                wrist = WristRight;
                elbow = ElbowRight;
                thumb = ThumbRight;
                break;
            case 51:
                //Debug.Log("Left palm");
                wrist = WristLeft;
                elbow = ElbowLeft;
                thumb = ThumbLeft;
                break;
            default:
                break;
        }

        Vector3 wristJoint = joints[wrist].transform.position;
        //Vector3 elbowJoint = joints[elbow].transform.position;
        Vector3 thumbJoint = joints[thumb].transform.position;

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 지나간 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            // 처음 손바닥을 위로 폈을 때의 벡터


            if (wristJoint != Vector3.zero && thumbJoint != Vector3.zero)
            {
                // 손목에서 엄지 손가락 까지
                Vector3 arrowToThumb = thumbJoint - wristJoint;
                // 팔꿈치에서 손목까지
                //Vector3 arrowToWrist = wristJoint - elbowJoint;

                // TODO : 손바닥을 위로 폈을 때 firstArrow가 설정되게 해야됨 그 전에 설정되면 안됨
                if (firstArrow == Vector3.zero /*&& Input.GetKey(KeyCode.A)*/)
                {
                    firstArrow = arrowToThumb;
                    //Debug.Log("firstArrow" + firstArrow);
                }

                // 손바닥 뒤집기 각도 재기
                if (firstArrow != Vector3.zero)
                {
                    currentAngle = Vector3.Angle(firstArrow, arrowToThumb);
                    //Debug.Log(time + "Palm Angle " + currentAngle);
                    angleValue.GetComponent<Text>().text = currentAngle.ToString();

                    if (maxAngle < currentAngle)
                    {
                        // 최대 각도 갱신
                        maxAngle = currentAngle;
                        //Debug.Log(maxAngle);
                        maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                        //Debug.Log("maxAngle = " + maxAngle);

                        // 3초 제한 초기화
                        time = 0f;
                    }

                    // 3초 시간 제한 초과
                    if (time > 3.0f)
                    {
                        if (wrist == WristRight)
                        {
                            //Debug.Log("Right ThumbAngle Checked");
                            //maxAngleRightThumb = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        else if (wrist == WristLeft)
                        {
                            //Debug.Log("Left Shoulder Checked");
                            //maxAngleLeftThumb = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        //Debug.Log("Time Clear");
                    }
                }
            }
        }
    }

    void elbowFlexion(int Check)
    {
        switch (Check)
        {
            case 42:
                //Debug.Log("Right");
                wrist = WristRight;
                elbow = ElbowRight;
                break;
            case 41:
                //Debug.Log("Left");
                wrist = WristLeft;
                elbow = ElbowLeft;
                break;
            default:
                break;
        }

        Vector3 wristJoint = joints[wrist].transform.position;
        Vector3 elbowJoint = joints[elbow].transform.position;

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 지나간 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            // 처음 손바닥을 위로 폈을 때의 벡터


            if (wristJoint != Vector3.zero && elbowJoint != Vector3.zero)
            {
                // 팔꿈치에서 손목까지
                Vector3 elbowToWrist = wristJoint - elbowJoint;

                // 각도 재기
                if (elbowToWrist != Vector3.zero)
                {
                    currentAngle = Vector3.Angle(Vector3.down, elbowToWrist);
                    //Debug.Log(time + " elbow Flexion " + currentAngle);

                    // ui에 현재 각도 표시
                    angleValue.GetComponent<Text>().text = currentAngle.ToString();

                    if (maxAngle < currentAngle)
                    {
                        // 최대 각도 갱신
                        maxAngle = currentAngle;
                        // Debug.Log(maxAngle);
                        // ui에 최대 각도 표시
                        maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                        //Debug.Log("maxAngle = " + maxAngle);

                        // 3초 제한 초기화
                        time = 0f;
                    }

                    // 3초 시간 제한 초과
                    if (time > 3.0f)
                    {
                        if (wrist == WristRight)
                        {
                            //Debug.Log("Right ThumbAngle Checked");
                            //maxAngleRightThumb = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        else if (wrist == WristLeft)
                        {
                            //Debug.Log("Left Shoulder Checked");
                            //maxAngleLeftThumb = maxAngle;
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        //Debug.Log("Time Clear");
                    }
                }
            }
        }
    }

    void hipFlexion(int Check)
    {
        switch (Check)
        {
            case 72:
                //Debug.Log("Right");
                hip = HipRight;
                knee = KneeRight;
                //ankle = AnkleRight;
                break;
            case 71:
                //Debug.Log("Left");
                hip = HipLeft;
                knee = KneeLeft;
                //ankle = AnkleLeft;
                break;
            default:
                break;
        }

        Vector3 hipJoint = joints[hip].transform.position;
        Vector3 kneeJoint = joints[knee].transform.position;

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 지나간 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            if (hipJoint != Vector3.zero && kneeJoint != Vector3.zero)
            {
                // 팔꿈치에서 손목까지
                Vector3 hipToKnee = kneeJoint - hipJoint;
                //hipToKnee.x = 0f;

                /*float tempAngle;
                if(Vector3.Angle(Vector3.down, hipToKnee) > 160f)
                {
                    tempAngle = Vector3.Angle(Vector3.down, hipToKnee);

                    //tempAngle -= 180f;

                    if(tempAngle < 0)
                    {
                        tempAngle = -tempAngle;
                    }

                }
                else
                {
                    tempAngle = Vector3.Angle(Vector3.down, hipToKnee);
                    Debug.Log("100도 미만");
                }*/

                // 각도 재기
                if (hipToKnee != Vector3.zero)
                {
                    currentAngle = Vector3.Angle(Vector3.down, hipToKnee);
                    //Debug.Log(time + " hipFlexion : " + currentAngle);

                    // ui에 현재 각도 표시
                    angleValue.GetComponent<Text>().text = currentAngle.ToString();

                    if (maxAngle < currentAngle)
                    {
                        // 최대 각도 갱신
                        maxAngle = currentAngle;
                        //Debug.Log(maxAngle);
                        // ui에 최대 각도 표시
                        maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                        //Debug.Log("maxAngle = " + maxAngle);

                        // 3초 제한 초기화
                        time = 0f;
                    }

                    // 3초 시간 제한 초과
                    if (time > 3.0f)
                    {
                        if (hip == HipRight)
                        {
                            //Debug.Log("Right HipKnee Checked");
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        else if (hip == HipLeft)
                        {
                            //Debug.Log("Left HipKnee Checked");
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        //Debug.Log("Time Clear");
                    }
                }
            }
        }

    } // TODO : 각도가 제대로 표시 되지 않는걸로 판단됨 ------ 각도가 180 - 현재각도로 표시되는 듯함 확인요망



    void wristExtension(int Check)
    {
        switch (Check)
        {
            case 112111:
                //.Log("Right");
                handTip = HandTipRight;
                wrist = WristRight;
                elbow = ElbowRight;
                break;
            case 111111:
                //Debug.Log("Left");
                handTip = HandTipLeft;
                wrist = WristLeft;
                elbow = ElbowLeft;
                break;
            default:
                break;
        }

        Vector3 handTipJoint = joints[handTip].transform.position;
        Vector3 wristJoint = joints[wrist].transform.position;
        Vector3 elbowJoint = joints[elbow].transform.position;

        if (joints != null)
        {
            // 실제 시간 (임시) joints들이 나타나고 부터 됨
            time += Time.deltaTime;

            // ui에 지나간 시간 표시
            durationValue.GetComponent<Text>().text = string.Format("{0:F1}", time) + " sec";

            if (handTipJoint != Vector3.zero && wristJoint != Vector3.zero && elbowJoint != Vector3.zero)
            {
                // 팔꿈치에서 손목까지
                Vector3 elbowToWrist = wristJoint - elbowJoint;
                Vector3 wristToHandTip = handTipJoint - wristJoint;

                // 각도 재기
                if (elbowToWrist != Vector3.zero && wristToHandTip != Vector3.zero)
                {
                    currentAngle = Vector3.Angle(elbowToWrist, wristToHandTip);
                    //Debug.Log(time + " wristExtension " + currentAngle);

                    // ui에 현재 각도 표시
                    angleValue.GetComponent<Text>().text = currentAngle.ToString();

                    if (maxAngle < currentAngle)
                    {
                        // 최대 각도 갱신
                        maxAngle = currentAngle;
                        //Debug.Log(maxAngle);
                        // ui에 최대 각도 표시
                        maxAngleValue.GetComponent<Text>().text = maxAngle.ToString();
                        //Debug.Log("maxAngle = " + maxAngle);

                        // 3초 제한 초기화
                        time = 0f;
                    }

                    // 3초 시간 제한 초과
                    if (time > 3.0f)
                    {
                        if (hip == HipRight)
                        {
                            //Debug.Log("Right handTipExtension Checked");
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        else if (hip == HipLeft)
                        {
                            //Debug.Log("Left handTipExtension Checked");
                            maxAngle = 0.1f;
                            time = 0f;
                            isChecked = true;
                        }
                        //Debug.Log("Time Clear");
                    }
                }
            }
        }
    } // TODO : 현재 이 진단함수는 실제 적용될지 안될지 모름


    // TODO : 함수를 추가 할때마다 수정해야함
    public void reset()
    {
        manager = KinectManager.Instance;
        Debug.Log("Call Reset");
        diagnosisState = State_Idle;
        Debug.Log(diagnosisState.ToString());

        sh_angle = 0;
        hh_angle = 0;
        sh_anglelist.Clear();
        hh_anglelist.Clear();
        ScreenShot_State = 0;

        //bool isDataSending = false;
        isMoviePlay = false;
        isChecking = false;
        isChecked = false;
        firstCheck = false;
        c_check = false;

        currentAngle = 0;
        maxAngle = 0.1f;

        c_time = 0;
        time = 0f;
        stateTime = 0f;

        firstArrow = Vector3.zero;
        jsonList.Clear();
        returnColor();

        enteredKinectInfo = false;
        enteredCheckDate = false;

        angleValue.GetComponent<Text>().text = "0";
        maxAngleValue.GetComponent<Text>().text = "0";
        durationValue.GetComponent<Text>().text = "0";

        shAngleValue.GetComponent<Text>().text = "0";
        hhAngleValue.GetComponent<Text>().text = "0";
        r_durationValue.GetComponent<Text>().text = "0";

        DotLeftKnee = null;
        DotRightKnee = null;

        DotLeftHip = null;
        DotRightHip = null;

        DotLeftShoulder = null;
        DotRightShoulder = null;

        DotKneeMiddle = null;
        DotHipMiddle = null;
        DotShoulderMiddle = null;

        DotNeck = null;
        DotHead = null;

        Knee1 = null;
        Knee2 = null;

        Spine1 = null;
        Spine2 = null;

        hor1 = null;
        hor2 = null;
        hor3 = null;

        Neck1 = null;
        Head1 = null;
        Xp = 100;
        Yp = 1000;

        DotLeftKneeHor = null;
        DotRightKneeHor = null;

        DotLeftHipHor = null;
        DotRightHipHor = null;

        DotLeftShoulderHor = null;
        DotRightShoulderHor = null;

        Neck1Hor = null;
        Head1Hor = null;

        hor1hor = null;
        hor2hor = null;
        hor3hor = null;
        FeetHor = null;

        XpHor = 100;
        YpHor = 1.01f;
    }

    public void FileNameSetting()
    {
        string[] DataTime_Now = DateTime.Now.ToString().Replace("/", "_").Split(' ');
        PNG_FileName = patientId + "_" + checkdateId + "_" + DataTime_Now[0] + ".png";
    }

    string Movie_FileName;

    public void movieNameSetting()
    {
        string[] DataTime_Now = DateTime.Now.ToString().Replace("/", "_").Split(' ');
        Movie_FileName = patientId + "_" + checkdateId + "_" + DataTime_Now[0];
    }

    //조인트
    public RectTransform DotLeftKnee;
    public RectTransform DotRightKnee;

    public RectTransform DotLeftHip;
    public RectTransform DotRightHip;

    public RectTransform DotLeftShoulder;
    public RectTransform DotRightShoulder;

    public RectTransform DotKneeMiddle;
    public RectTransform DotHipMiddle;
    public RectTransform DotShoulderMiddle;

    public RectTransform DotNeck;
    public RectTransform DotHead;

    //뼈대들
    public RectTransform Knee1;
    public RectTransform Knee2;

    public RectTransform Spine1;
    public RectTransform Spine2;

    public RectTransform hor1;
    public RectTransform hor2;
    public RectTransform hor3;

    public RectTransform Neck1;
    public RectTransform Head1;

    public float Xp = 100;
    public float Yp = 1000;

    /*
    void showUISkeletonPoint() //왼쪽의 스켈레톤 모식도를 그려주는 함수
    {
        //중심과 기본이 되는 변수들
        float centerX = joints[SpineBase].transform.position.x;

        float foo = Math.Min(joints[FootLeft].transform.position.y, joints[FootRight].transform.position.y);

        float ShoulderToSpineHeight =
            joints[SpineShoulder].transform.position.y
            - joints[SpineBase].transform.position.y;

        float SpineToFooHeitht =
             joints[SpineBase].transform.position.y
            - foo;

        //밑바닥 부터 시작, 발바닥은 고정 되어 있다고 본다.
        //Knee파트
        float leftKneeX =
          joints[KneeLeft].transform.position.x
          - centerX;
        float rightKneeX =
            joints[KneeRight].transform.position.x
            - centerX;

        float leftKneeY =
           joints[KneeLeft].transform.position.y
           - foo;
        float rightKneeY =
            joints[KneeRight].transform.position.y
            - foo;

        //leftKneeHeight = 75 * leftKneeHeight / SpineToFooHeitht;
        //rightKneeHeight = 75 * rightKneeHeight / SpineToFooHeitht;

        DotLeftKnee.anchoredPosition = new Vector2(Xp * leftKneeX, Yp * leftKneeY);
        DotRightKnee.anchoredPosition = new Vector2(Xp * rightKneeX, Yp * rightKneeY);

        //DotKneeMiddle파트, 두 Knee 사이의 값을 잡아준다
        float DotKneeMiddleX = (leftKneeX + rightKneeX) / 2;

        float DotKneeMiddleY = (leftKneeY + rightKneeY) / 2;

        DotKneeMiddle.anchoredPosition = new Vector2(Xp * DotKneeMiddleX, Yp * DotKneeMiddleY);

        //Hip파트
        float leftHipX =
          joints[HipLeft].transform.position.x
          - centerX;
        float rightHipX =
            joints[HipRight].transform.position.x
            - centerX;

        float leftHipY =
           joints[HipLeft].transform.position.y
           - foo;
        float rightHipY =
            joints[HipRight].transform.position.y
            - foo;

        //WleftHipHeight = 75 * leftHipHeight / ShoulderToSpineHeight;
        //WrightHipHeight = 75 * rightHipHeight / ShoulderToSpineHeight;

        DotLeftHip.anchoredPosition = new Vector2(Xp * leftHipX, Yp * leftHipY);
        DotRightHip.anchoredPosition = new Vector2(Xp * rightHipX, Yp * rightHipY);

        //DotHipMiddle파트
        float DotHipMiddleX = (leftHipX + rightHipX) / 2;

        float DotHipMiddleY = (leftHipY + rightHipY) / 2;

        DotHipMiddle.anchoredPosition = new Vector2(Xp * DotHipMiddleX, Yp * DotHipMiddleY);

        //Shoulder파트
        float leftShoulderX =
            joints[ShoulderLeft].transform.position.x
            - centerX;
        float rightShoulderX =
            joints[ShoulderRight].transform.position.x
            - centerX;

        float leftShoulderY =
           joints[ShoulderLeft].transform.position.y
           - foo;
        float rightShoulderY =
            joints[ShoulderRight].transform.position.y
            - foo;

        //leftShoulderHeight = 60 * leftShoulderHeight / ShoulderToSpineHeight;
        //rightShoulderHeight = 60 * rightShoulderHeight / ShoulderToSpineHeight;

        DotLeftShoulder.anchoredPosition = new Vector2(Xp * leftShoulderX, Yp * leftShoulderY);
        DotRightShoulder.anchoredPosition = new Vector2(Xp * rightShoulderX, Yp * rightShoulderY);

        //DotShoulderMiddle파트
        float DotShoulderMiddleX = (leftShoulderX + rightShoulderX) / 2;

        float DotShoulderMiddleY = (leftShoulderY + rightShoulderY) / 2;

        DotShoulderMiddle.anchoredPosition = new Vector2(Xp * DotShoulderMiddleX, Yp * DotShoulderMiddleY);

        //Neck파트
        float neckX = joints[Neck].transform.position.x
           - centerX;

        float neckY = joints[Neck].transform.position.y
           - foo;

        DotNeck.anchoredPosition = new Vector2(Xp * neckX, Yp * neckY);

        //Head파트
        float headX = joints[Head].transform.position.x
            - centerX;

        float headY = joints[Head].transform.position.y
            - foo;

        DotHead.anchoredPosition = new Vector2(Xp * headX, Yp * headY);

        //선 긋기
        //DrawLine(Knee1, DotLeftKnee, DotLeftHip);
        //DrawLine(Knee2, DotRightKnee, DotRightHip);

        DrawLine(hor1, DotLeftKnee, DotRightKnee);
        DrawLine(hor2, DotLeftHip, DotRightHip);
        DrawLine(hor3, DotLeftShoulder, DotRightShoulder);

        DrawLine(Spine1, DotKneeMiddle, DotHipMiddle);
        DrawLine(Spine2, DotHipMiddle, DotShoulderMiddle);
        DrawLine(Neck1, DotShoulderMiddle, DotNeck);
        DrawLine(Head1, DotNeck, DotHead);
    }
    */

    public RectTransform DotLeftKneeHor;
    public RectTransform DotRightKneeHor;

    public RectTransform DotLeftHipHor;
    public RectTransform DotRightHipHor;

    public RectTransform DotLeftShoulderHor;
    public RectTransform DotRightShoulderHor;

    public RectTransform Neck1Hor;
    public RectTransform Head1Hor;

    public RectTransform hor1hor;
    public RectTransform hor2hor;
    public RectTransform hor3hor;
    public RectTransform FeetHor;

    public float XpHor = 100;
    public float YpHor = 1.01f;

    /*
    void showUISkeletonPoint2()
    {
        float centerAnkle = (joints[AnkleLeft].transform.position.z + joints[AnkleRight].transform.position.z) / 2;

        float DotLeftKneeHorZ = joints[KneeLeft].transform.position.z - centerAnkle;

        float DotRightKneeHorZ = joints[KneeRight].transform.position.z - centerAnkle;

        DotLeftKneeHor.anchoredPosition = new Vector2(-27, YpHor * DotLeftKneeHorZ);
        DotRightKneeHor.anchoredPosition = new Vector2(27, YpHor * DotRightKneeHorZ);

        DrawLine(hor1hor, DotLeftKneeHor, DotRightKneeHor);

        float DotLeftHipHorZ = joints[HipLeft].transform.position.z - centerAnkle;

        float DotRightHipHorZ = joints[HipRight].transform.position.z - centerAnkle;

        DotLeftHipHor.anchoredPosition = new Vector2(-15, YpHor * DotLeftHipHorZ);
        DotRightHipHor.anchoredPosition = new Vector2(15, YpHor * DotRightHipHorZ);

        DrawLine(hor2hor, DotLeftHipHor, DotRightHipHor);

        float DotLeftShoulderHorZ = joints[ShoulderLeft].transform.position.z - centerAnkle;

        float DotRightShoulderHorZ = joints[ShoulderRight].transform.position.z - centerAnkle;

        DotLeftShoulderHor.anchoredPosition = new Vector2(-40, YpHor * DotLeftShoulderHorZ);
        DotRightShoulderHor.anchoredPosition = new Vector2(40, YpHor * DotRightShoulderHorZ);

        DrawLine(hor3hor, DotLeftShoulderHor, DotRightShoulderHor);

        float Neck1HorZ = joints[Neck].transform.position.z - centerAnkle;
        float Head1HorZ = joints[Head].transform.position.z - centerAnkle;

        Neck1Hor.anchoredPosition = new Vector2(0, YpHor * Neck1HorZ);
        Head1Hor.anchoredPosition = new Vector2(0, YpHor * Head1HorZ);
    }

    void DrawLine(RectTransform imageRectTransform, RectTransform PointA, RectTransform PointB)
    {
        Vector2 differenceVector = PointB.localPosition - PointA.localPosition;

        imageRectTransform.sizeDelta = new Vector2(differenceVector.magnitude, 5f);
        imageRectTransform.pivot = new Vector2(0, 0.5f);
        imageRectTransform.localPosition = PointA.localPosition;
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        imageRectTransform.rotation = Quaternion.Euler(0, 0, angle);
    }
    */


    LineRenderer drawDegreeLine(LineRenderer d_line, Vector3 start, Vector3 destination)
    {
        if (d_line == null && degreeLine != null)
        {
            d_line = Instantiate(degreeLine) as LineRenderer;
            d_line.transform.parent = transform;
            d_line.gameObject.SetActive(false);
        }

        if (d_line != null)
        {

            if (start != Vector3.zero && destination != Vector3.zero)
            {
                d_line.gameObject.SetActive(true);
                d_line.SetPosition(0, start);
                d_line.SetPosition(1, destination);
            }
            else
            {
                d_line.gameObject.SetActive(false);
            }
        }

        return d_line;
    }


 
    public void state_intro()
    {
        diagnosisState = State_Intro;
        // 아래의 코루틴이 state_intro2 로 변경시킴
        StartCoroutine(receiveKinectInfo(kinectInfoURL));
    }

    public void state_intro2()
    {
        patientAge = DateTime.Now.Year % 100 + 100 - int.Parse(patientBirth.Substring(2, 2));
        jointDirection = kinectsInfo[0].getJointDirection();        //검사유형정보 추출


        nameText.GetComponent<Text>().text = patientName;           //UI에 정보 띄우기


        // TODO:: 오류남 patientinfo를 받아오기전에 먼저 실행되서 오류 나는걸로 추측됨 diagnosisState의 변수 대입시의 수정으로 고쳐질 듯 함
        patientInfoText.GetComponent<Text>().text = patientAge + "세 " + patientSex + " " + patientBirth;  //UI에 정보 띄우기

        guideText.GetComponent<Text>().text = "안녕하세요 " + patientName + "님\n 지금부터 진단을 시작하겠습니다.";
        guideUI.SetActive(true);

        sh_anglelist = new List<float>();
        hh_anglelist = new List<float>();

        diagnosisState = State_Guide_UI; //다음 state
    }

    public void state_guide_UI()
    {
        stateTime += Time.deltaTime;

        textTrans(jointDirection);
        if (stateTime >= 4.0f && stateTime <= 6.0f)
        {
            // 가이드 동영상 실행
            // 동영상이 준비된 후의 수정 요망
            /*if (!isMoviePlay)
            {
                moviePlay(jointDirection);
                isMoviePlay = true;
            }*/
            // 가이드 동영상 실행
            guideUI.SetActive(false);
            guideText.GetComponent<Text>().text = guideText2;
        }
        if (stateTime >= 6.0f)
            guideUI.SetActive(true);
        if (stateTime >= 10.0f)
        {
            guideUI.SetActive(false);
            guideText.GetComponent<Text>().text = guideText3;
        }
        if (stateTime >= 12.0f)
            guideUI.SetActive(true);
        if (stateTime >= 16.0f)
        {
            // 가이드 동영상 정지
            // 동영상이 준비된 후의 수정 요망
            /*if (isMoviePlay)
            {
                movieStop();
                isMoviePlay = false;
            }*/
            // 가이드 동영상 정지
            guideUI.SetActive(false);
            diagnosisState = State_Diagnosis; //다음 state
            stateTime = 0f;


        }
    }

    public void state_diagnosis()
    {
        if (!isChecked)
        {
            if (!isChecking)
            {
                // 진단 중 영상 실행
                movieNameSetting();
                AV_name(Movie_FileName);
                AV_Start();
            }
            switch (jointDirection)
            {
                // shoulder Abduction And Flexion
                case 11:
                case 12:
                case 21:
                case 22:
                    shoulderAbduction_Flexion(jointDirection);
                    break;
                // shoulder Rotation
                case 31:
                case 32:
                    shoulderRotation(jointDirection);
                    break;
                // elbow Flexion
                case 41:
                case 42:
                    elbowFlexion(jointDirection);
                    break;
                // elbow Pronation
                case 51:
                case 52:
                    elbowPronation(jointDirection);
                    break;
                // hip Flexion
                case 71:
                case 72:
                    hipFlexion(jointDirection);
                    break;
                case 201:
                    horizon_ShoulderLine();
                    break;
                // face checking
                case 91:
                case 92:
                case 101:
                case 102:
                case 111:
                case 112:
                    JL_turnOff();
                    faceTracking_update();
                    break;

                default:
                    Debug.Log("NOPE");
                    break;
            }
            stateTime += Time.deltaTime;

            // 진단 중이다.
            isChecking = true;
            // 진단 중에 관절 빨간색으로 표시
            jointSelect(jointDirection);
            glow_Color();
            // 진단 중에 관절 빨간색으로 표시
        }
        // state 상태 변경
        if (isChecked)
        {
            // MOVIE : 동영상 촬영 중단 및 저장 및 확장자명 수정
            StartCoroutine(stopAV());

            // leftman.SetActive(true);
            // leftui.SetActive(true);
            FaceModel.SetActive(false);
            JL_turnOn();
            diagnosisState = State_Diagnosis_End;
            enteredCheckDate = false;
            returnColor();
            isChecking = false;
            stateTime = 0;
        }
        // 진단 완료
        isChecked = false;
    }

    public void state_diagnosis_ending()
    {

        stateTime += Time.deltaTime;
        if (!enteredCheckDate)
        {
            if (hcheck == true)
            {
                StartCoroutine(checkDate_to_hLIne(checkDateURL, patientId, jointDirection.ToString(), sh_angle_middle.ToString(), hh_angle_middle.ToString()));
            }
            else
            {
                StartCoroutine(checkDate(checkDateURL, patientId, jointDirection.ToString(), maxAngleValue.GetComponent<Text>().text));
            }
            enteredCheckDate = true;
        }

        // 한 사람이 여러개의 검사를 할 때 if문 실행
        if (kinectsInfo.Count != 1 && kinectsInfo[1].getPatientID() == patientId)
        {
            if (!isUploaded)
            {
                guideText.GetComponent<Text>().text = "검사 결과를 업데이트 중입니다.\n잠시만 기다려 주십시오.";
                guideUI.SetActive(true);
            }
            else
            {
                guideText.GetComponent<Text>().text = "검사를 계속 진행하겠습니다.";
                guideUI.SetActive(true);

                // stateTime 최적화 해 볼 필요 있음
                if (stateTime >= 4.0f)
                {
                    state_deleteKinectSC();
                }
            }
        }
        else // 한 사람이 하나의 검사를 하고 퇴장.
        {
            guideText.GetComponent<Text>().text = "검사를 모두 마쳤습니다. \n수고하셨습니다.";
            firstCheck = false;
            guideUI.SetActive(true);

            //if (manager.GetUsersCount() == 0) //조건 : 사람이 떠날때 (수정필요)
            //{
            //    StartCoroutine(deleteKinectInfo(kinectInfoDeletURL, kinectsInfo[0].getKinectID()));
            //    kinectsInfo.RemoveAt(0);

            //    patientInfoText.GetComponent<Text>().text = "    ";
            //    testCaseText.GetComponent<Text>().text = "";
            //    nameText.GetComponent<Text>().text = "";
            //    angleValue.GetComponent<Text>().text = "0";
            //    maxAngleValue.GetComponent<Text>().text = "0";
            //    durationValue.GetComponent<Text>().text = "0초";

            //    enteredKinectInfo = false;
            //    guideUI.SetActive(false);
            //    stateTime = 0f;
            //    diagnosisState = State_Idle;
            //}
        }
    }

    public void state_deleteKinectSC()
    {
        Debug.Log("한 환자의 연속된 검진후 키넥트 스케쥴 딜리트 사람 유지 중에 실행되는거");
        StartCoroutine(deleteKinectInfo(kinectInfoDeletURL, kinectsInfo[0].getKinectID()));
        kinectsInfo.RemoveAt(0);

        jointDirection = kinectsInfo[0].getJointDirection();        //검사유형정보 추출
        guideUI.SetActive(false);
        stateTime = 0f;
        diagnosisState = State_Guide_UI;
    }

    public void state_anotherguy_need()
    {
        // TODO 진단이 한번 끝날 때 마다 delete 한번씩 돌아가게 해야된다.
        enteredKinectInfo = false;
        if (diagnosisState == State_Diagnosis_End /*&& !(kinectsInfo.Count >= 1 && kinectsInfo[1].getPatientID() == patientId)*/)
        {
            Debug.Log("한 환자의 모든 진단이 끝났을때 키넥트 스케쥴 삭제 사람 없어지면서 실행되는거");
            StartCoroutine(deleteKinectInfo(kinectInfoDeletURL, kinectsInfo[0].getKinectID()));
            kinectsInfo.RemoveAt(0);

            patientInfoText.GetComponent<Text>().text = "    ";
            testCaseText.GetComponent<Text>().text = "";
            nameText.GetComponent<Text>().text = "";
            angleValue.GetComponent<Text>().text = "0";
            maxAngleValue.GetComponent<Text>().text = "0";
            durationValue.GetComponent<Text>().text = "0초";
            maxAngle = 0.1f;
            sh_angle_middle = 0;
            hh_angle_middle = 0;

            enteredKinectInfo = false;
            guideUI.SetActive(false);
            stateTime = 0f;
        }
    }
    IEnumerator receiveKinectInfo(string URL)
    {
        WWW www = new WWW(URL);

        yield return www;
        //www가 웹에서 값을 전부 받아온 후 부터 이하의 코드 실행

        //Debug.Log("www.text : " + www.text); //URL에서 가져온 string 값을 출력

        if (www.error == null) //에러여부 확인, 에러가 존재할 경우 else부분에서 ERROR 출력
        {
            Dictionary<string, object>[] kdict;
            // 렉걸림
            if (www.text.Contains("[]"))
            {
                kdict = new Dictionary<string, object>[0];
            }
            else
            {
                kdict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text); //www가 가져온 string을 해석해 object 타입의 인스턴스
            }
            // 렉걸림

            kinectsInfo.Clear();

            for (int i = 0; i < kdict.Length; i++)
            {
                //Debug.Log(i + 1 + "번째 예약 " + kdict[i]);

                //Debug.Log("Num of column : " + kdict[i].Count); //콜럼의 갯수

                kinectsInfo.Add(new KinectInfo((string)kdict[i]["kinectid"], (string)kdict[i]["patientid"],
                    (string)kdict[i]["jointdirection"], (string)kdict[i]["datetime"], (string)kdict[i]["forcecode"]));//KinectInfo에 래핑
            }
            if (kdict.Length > 0)
            {
                WWWForm form = new WWWForm();
                form.AddField("patientid", kinectsInfo[0].getPatientID());
                StartCoroutine(receivePatientInfo(patientInfoURL, form));
            }
        }
        else
        {
            //Debug.LogError("ERROR : " + www.error); //콘솔에 에러 내용 출력
        }


        enteredKinectInfo = true;
    }

    IEnumerator receivePatientInfo(string URL, WWWForm form)
    {

        WWW www = new WWW(URL, form);

        yield return www;
        Debug.Log(www.text);
        if (www.error == null)
        {
            Dictionary<string, object>[] pdict;
            pdict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text);


            patientId = (string)pdict[0]["patientid"];
            //Debug.Log(patientId);
            patientName = (string)pdict[0]["name"];
            if ((string)pdict[0]["sex"] == "0")
            {
                patientSex = "남";
            }
            else if ((string)pdict[0]["sex"] == "1")
            {
                patientSex = "여";
            }
            patientBirth = (string)pdict[0]["birth"];
            patientNumber = (string)pdict[0]["number"];
            diagnosisState = State_Intro2;


        }
    }

    IEnumerator deleteKinectInfo(string URL, string kinectid)
    {
        diagnosisState = State_Idle;
        WWWForm form = new WWWForm();

        form.AddField("kinectid", kinectid); //POST의 BODY부분의 필드에 kinectid 값을 입력

        WWW www = new WWW(URL, form); //WWW를 이용해 URL로 form을 전송

        yield return www;
        //www에서 요청의 결과를 받아옴

        if (www.error == null)
        {
            Debug.Log("Delete done");
        }
        else
        {
            Debug.LogError("ERROR : " + www.error);
        }
        deleteFile();
    }

    // 2017-3-26 
    IEnumerator receivecheckdataId(string URL) // checkDateURL
    {
        WWWForm form = new WWWForm();
        form.AddField("Text", "Text");
        WWW www = new WWW(URL, form);
        yield return www;

        if (www.error == null)
        {
            Dictionary<string, object>[] cddict;
            try
            {
                cddict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text);


                checkdateId = (string)cddict[0]["checkdateid"];
                int tempcheck = Convert.ToInt32(checkdateId);
                tempcheck++;
                checkdateId = tempcheck.ToString();
            }
            catch (Exception ex)
            {
                Debug.Log("" + ex.StackTrace.ToString());
                Debug.Log("check data id 를 0으로 만들었다.");
                checkdateId = "0";
            }
        }
        else
        {
            Debug.LogError("ERROR : " + www.error);
        }
    }

    // 2017-3-26 FTP 서버로 파일 보내기
    // TODO : ftp 서버로 파일을 보낼 때, 에러가 발생함. 에러 수정 필요 

    // 2017-3-26
    IEnumerator checkDate(string URL, string patientID, string jointDirection, string maxAngle)
    {
        //isDataSending = true;
        WWWForm form = new WWWForm();
        form.AddField("patientid", patientID);
        form.AddField("jointdirection", jointDirection);
        form.AddField("maxangle", maxAngle);
        if (PNG_FileName != null)
        {
            form.AddField("image", PNG_FileName);
        }
        form.AddField("movie", Movie_FileName + ".mp4");
        WWW www = new WWW(URL, form);
        yield return www;
        if (www.error == null)
        {
            Debug.Log(www.text);
            Dictionary<string, object>[] cddict;
            cddict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text);
            checkdateId = (string)cddict[0]["checkdateid"];
        }
        else
        {
            // Debug.LogError("ERROR : " + www.error);
        }
        if (PNG_FileName != null)
        {
            StartCoroutine(sendimage());
        }
        StartCoroutine(sendMovie());
        // 05-21 이종원
        if (isfaceChecking)
        {
            StartCoroutine(checkTimeFace(checkTimeFaceURL, checkdateId, jsonList_face));
        }
        else
        {
            StartCoroutine(checkTime(checkTimeURL, checkdateId, jsonList));
        }
        // 05-21 이종원
    } // checkDate 전송 및 checkTime coroutine 실행



    private string m_URL = "http://igrus.mireene.com/rom_server/mysavefile.php";


    // 05-21 안형빈
    // TODO :: Thread 작동하는지 확인해봐야함
    IEnumerator sendimage()
    {
        Debug.Log("Directory : " + Directory.GetCurrentDirectory());

        WWW localFile = new WWW("file:///" + Directory.GetCurrentDirectory().ToString().Replace('\\', '/') + "/" + PNG_FileName);
        //Debug.Log(localFile.url);
        yield return localFile;
        if (localFile.error == null)
            Debug.Log("Loaded file successfully");
        else
        {
            //Debug.Log("Open file error: " + localFile.error);
            yield break; // stop the coroutine here
        }

        Debug.Log(localFile.size);

        WWWForm postForm = new WWWForm();
        // version 1
        //postForm.AddBinaryData("theFile",localFile.bytes);

        // version 2
        postForm.AddBinaryData("file", localFile.bytes, PNG_FileName);

        WWW upload = new WWW(m_URL, postForm);

        yield return upload;

        if (upload.error == null)
            Debug.Log("upload done :" + upload.text);
        else
            Debug.Log("Error during upload: " + upload.error);
    }

    private string k_Url = "http://igrus.mireene.com/rom_server/savemovie.php";

    IEnumerator sendMovie()
    {
        Debug.Log("Directory : " + Directory.GetCurrentDirectory());

        string directory = "file:///" + Directory.GetCurrentDirectory().ToString().Replace('\\', '/') + "/" + Movie_FileName;


        File.Move(Directory.GetCurrentDirectory().ToString() + "\\" + Movie_FileName + ".wav", Directory.GetCurrentDirectory().ToString() + "\\" + Movie_FileName + ".mp4");

        WWW localFile = new WWW(directory + ".mp4");
        //Debug.Log(localFile.url);
        yield return localFile;
        if (localFile.error == null)
            Debug.Log("Loaded file successfully");
        else
        {
            //Debug.Log("Open file error: " + localFile.error);
            yield break; // stop the coroutine here
        }

        Debug.Log(localFile.size);

        WWWForm postForm = new WWWForm();
        // version 1
        //postForm.AddBinaryData("theFile",localFile.bytes);

        // version 2
        postForm.AddBinaryData("file", localFile.bytes, Movie_FileName + ".mp4");

        WWW upload = new WWW(k_Url, postForm);

        yield return upload;

        if (upload.error == null)
        {
            Debug.Log("upload done :" + upload.text);
            isUploaded = true; // 에러 있을 시 ui 안넘어감.
        }
        else
        {
            Debug.Log("Error during upload: " + upload.error);
        }

        
    }

    // 4-30 이종원
    // TODO :: php 아직 작성 안되어 있음 /////// 만들어서 확인해봐야됨
    IEnumerator checkTimeFace(string URL, string checkid, List<string> dicjson)
    {
        WWWForm form = new WWWForm();

        for (int i = 0; i < dicjson.Count; i++)
        {
            form = new WWWForm();
            form.AddField("checkdateid", checkid);
            form.AddField("framecount", i.ToString());
            form.AddField("dicjson", dicjson[i]);

            WWW www = new WWW(URL, form);

            yield return www;
        }
        //isDataSending = false;
        //Debug.Log("Data All Sended");

        // TODO ;: jsonList 가 아니라 face 관련된 리스트로 교체해야됨
        jsonList_face.Clear();
        frameCount = -1;
        isfaceChecking = false;
    }
    // 4-30 이종원

    IEnumerator checkDate_to_hLIne(string URL, string patientID, string jointDirection, string sh_angle, string hh_angle)
    {
        WWWForm form = new WWWForm();
        form.AddField("patientid", patientID);
        form.AddField("jointdirection", jointDirection);
        form.AddField("sh_angle", sh_angle);
        form.AddField("hh_angle", hh_angle);

        WWW www = new WWW(URL, form);
        yield return www;

        if (www.error == null)
        {
            Debug.Log(www.text);
            Dictionary<string, object>[] cddict;
            cddict = (Dictionary<string, object>[])JsonReader.Deserialize(www.text);
            Debug.Log(cddict.ToString());
            checkdateId = (string)cddict[0]["checkdateid"];
        }
        else
        {
            Debug.LogError("ERROR : " + www.error);
        }
        StartCoroutine(checkTime(checkTimeURL, checkdateId, jsonList));
        hcheck = false;
    }
    IEnumerator checkTime(string URL, string checkid, List<string> dicjson)
    {
        WWWForm form;

        for (int i = 0; i < dicjson.Count; i++)
        {
            form = new WWWForm();
            form.AddField("checkdateid", checkid);
            form.AddField("framecount", i.ToString());
            form.AddField("dicjson", dicjson[i]);

            WWW www = new WWW(URL, form);

            yield return www;
        }
        //isDataSending = false;
        //Debug.Log("Data All Sended");
        jsonList.Clear();
        frameCount = -1;
    } // checkTime data 전송
}


public class PatientInfo
{
    public string patientID;
    public string nameInfo;
    public string sexInfo;
    public string birthInfo;
    public string numberInfo;

    public PatientInfo(string ID, string name, string sex, string birth, string number)
    {
        setData(ID, name, sex, birth, number);
    }

    public int getPatientID()
    {
        return Int32.Parse(patientID);
    }

    public void setPatientID(string ID)
    {
        patientID = ID;
    }

    public string getName()
    {
        return nameInfo;
    }

    public void setName(string name)
    {
        nameInfo = name;
    }

    public string getSex()
    {
        return sexInfo;
    }

    public void setSex(string sex)
    {
        sexInfo = sex;
    }

    public string getBirth()
    {
        return birthInfo;
    }

    public void setBirth(string birth)
    {
        birthInfo = birth;
    }

    public string getNumber()
    {
        return numberInfo;
    }

    public void setNumber(string number)
    {
        numberInfo = number;
    }

    public void setData(string ID, string name, string sex, string birth, string number)
    {
        setPatientID(ID);
        setName(name);
        setSex(sex);
        setNumber(number);
        setBirth(birth);
    }
}

public class KinectInfo
{
    string kinectID;
    string patientID;
    string joinDirection;
    string dateTime;
    string forceCode;

    public KinectInfo()
    {

    }

    public KinectInfo(string kID, string pID, string joint, string date, string force)
    {
        setData(kID, pID, joint, date, force);
    }

    public string getKinectID()
    {
        return kinectID;
    }

    public void setKinectID(string ID)
    {
        kinectID = ID;
    }

    public string getPatientID()
    {
        return patientID;
    }

    public void setPatientID(string ID)
    {
        patientID = ID;
    }

    public int getJointDirection()
    {
        return Int32.Parse(joinDirection);
    }

    public void setjointDirection(string joint)
    {
        joinDirection = joint;
    }

    public string getDataTime()
    {
        return dateTime;
    }

    public void setDateTime(string date)
    {
        dateTime = date;
    }

    public string getForceCode()
    {
        return forceCode;
    }

    public void setForcrCode(string code)
    {
        forceCode = code;
    }

    public void setData(string kID, string pID, string joint, string date, string force)
    {
        setKinectID(kID);
        setPatientID(pID);
        setjointDirection(joint);
        setDateTime(date);
        setForcrCode(force);
    }
}