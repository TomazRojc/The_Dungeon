using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class BuildDebug : MonoBehaviour
{
	string filename = "";

	void OnEnable()
	{
		if (!Application.isEditor) Application.logMessageReceived += Log;
	}

	void OnDisable()
	{
		Application.logMessageReceived -= Log;
	}

	public void Log(string logString, string stackTrace, LogType type)
	{
		if (filename == "")
		{
			string dir = Application.dataPath + "/YOUR_LOGS";
			System.IO.Directory.CreateDirectory(dir);
			filename = dir + "/log_" + System.DateTime.UtcNow.ToString().Replace(". ", "").Replace(" ", "_").Replace(":", "") + ".txt";
		}
			try{ System.IO.File.AppendAllText(filename, "- " + logString + "\n"); }
			catch { }
	}
}
