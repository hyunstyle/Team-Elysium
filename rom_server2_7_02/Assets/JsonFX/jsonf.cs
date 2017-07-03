using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JsonFx.Json;

public class jsonf : MonoBehaviour {

	public static string Write(Dictionary<string, object> dic)
	{
		return JsonWriter.Serialize (dic);
	}
	public static Dictionary<string, object> Read(string json)
	{
		return JsonReader.Deserialize<Dictionary<string, object>> (json);
	}
}
