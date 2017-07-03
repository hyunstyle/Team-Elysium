#if UNITY_4_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5_0 || UNITY_5_1 || UNITY_5_2
	#define PRE_UNITY_5_3
#endif

using UnityEngine;
using System.Collections;

#if !PRE_UNITY_5_3
	using UnityEngine.SceneManagement;
#endif	

///<summary>
/// Menu class to transfer jumping function to load a scene
///</summary>
public class Menu : MonoBehaviour
{
	public void GoCanvasDemo()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("CanvasDemo");
#else
		SceneManager.LoadScene("CanvasDemo");
#endif
	}

	public void GoCanvasDemoBarAndLine()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("CanvasDemoBarAndLine");
#else
		SceneManager.LoadScene("CanvasDemoBarAndLine");
#endif
	}

	public void GoCanvasDemoPie()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("CanvasDemoPie");
#else
		SceneManager.LoadScene("CanvasDemoPie");
#endif
	}

	public void GoCountdownDemo()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("CountdownDemo");
#else
		SceneManager.LoadScene("CountdownDemo");
#endif
	}

	public void GoMeshDemo()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("MeshDemo");
#else
		SceneManager.LoadScene("MeshDemo");
#endif
	}

	public void GoMeshDemo3D()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("MeshDemo3D");
#else
		SceneManager.LoadScene("MeshDemo3D");
#endif
	}

	public void GoMenu()
	{
#if PRE_UNITY_5_3
		Application.LoadLevel("Menu");
#else
		SceneManager.LoadScene("Menu");
#endif
	}
}
