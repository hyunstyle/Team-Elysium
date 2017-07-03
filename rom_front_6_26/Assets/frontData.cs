using System.Collections.Generic;

public static class frontData
{
    public static Dictionary<int, Dictionary<string, object>> PatientData;
    public static Dictionary<int, Dictionary<string, object>> CheckDateData;
    public static Dictionary<int, Dictionary<string, object>> ChecktimeData;    // 보류
    public static Dictionary<int, Dictionary<string, object>> kinectscData;

    public static int selectedPatientID;
    public static int selectedCheckDataID;

    public static List<string> dateSet;

    // 클릭한 객체를 알려주는 인디케이터, 이벤트 시스템 대용
    public static string patientid = "0";
    public static string checkdateid = "0";
    public static string[] patientInfo = { "", "", "", "" };
    public static string imagename = "";

    // 0626 유상현 추가. front doctor scene에서 진료 완료 후 리스트에서 제거시 사용할 static 정보
    public static string[] clickedPatientInfo;
    public static string clickedData;
    public static int dictionaryLength;

    public class checktime
    {
        public string checktimeid = "0";
        public int framecount = 0;
        public string jsons = "";
    }
    public static List<checktime> checktimearr;

    public static string patientListURL = "http://igrus.mireene.com/rom_server/patientlist.php";
    public static string patientAddURL = "http://igrus.mireene.com/rom_server/patientadd.php";
    public static string checkDateListURL = "http://igrus.mireene.com/rom_server/checkdatelist.php";
    public static string checkTimeListURL = "http://igrus.mireene.com/rom_server/checktimelist.php";
    public static string fronttoKinectURL = "http://igrus.mireene.com/rom_server/fronttokinect.php";
    public static string registeredListURL = "http://igrus.mireene.com/rom_server/kinectsclist.php"; // 0625 유상현
    public static string kinectsc2URL = "http://igrus.mireene.com/rom_server/kinectsc2list.php"; // 0626 유상현
    public static string kinectsc2deleteURL = "http://igrus.mireene.com/rom_server/kinect2delete.php"; // 0626 유상현
}