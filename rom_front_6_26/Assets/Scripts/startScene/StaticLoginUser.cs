using UnityEngine;
using System.Collections;

public class StaticLoginUser : MonoBehaviour {

    public static string UID;
    public static string theName;
    public static int grade;
    public static string userid;
    public static string email;

    // Use this for initialization
    void Start () {
        UID = "";
        theName = "";
        grade = -1;
        userid = "";
        email = "";
    }
}
