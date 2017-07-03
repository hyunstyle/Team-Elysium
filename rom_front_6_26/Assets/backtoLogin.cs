using UnityEngine;
using UnityEngine.SceneManagement;

public class backtoLogin : MonoBehaviour
{
    public void BackLogin()
    {
        SceneManager.LoadScene("startScene");
    }
}