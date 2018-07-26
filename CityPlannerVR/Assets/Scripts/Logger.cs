using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class LoggedAction
{
	public string userID;
	public string timestamp;
	public string action;
	public Vector3 userPosition;
}

public class Logger : MonoBehaviour {

	//Logging syntax:
	//{"LoggedActions": [
	//{"userID": xxxx, "timestamp": xxxx, "action": xxxx, "userPosition": ["x": xxx, "y": xxx, "z": xxx]}
	//]}


	#region Private Variables
			
	private string userID;

	private string currentJSON = "";

	#endregion

	// Use this for initialization
	void Start () {
		GetUserID ();
		StartTimer ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void GetUserID()
	{
		
	}

	private void StartTimer()
	{
		
	}

	private string GetTimeSinceJoin()
	{
		return null;
	}

	public void LogLine(string action)
	{
		LoggedAction newAction = new LoggedAction ();
		newAction.userID = this.userID;
		newAction.timestamp = GetTimeSinceJoin ();
		newAction.action = action;
		newAction.userPosition = this.gameObject.transform.position;

		AddActionLine (newAction);

	}

	private void AddActionLine(LoggedAction actionLine)
	{
		string jsonline = JsonUtility.ToJson (actionLine);
		currentJSON += jsonline;

		PrintWarningLog ("New JSON line added! Currently stored JSON: " + currentJSON);

	}

	public void PrintWarningLog(string text)
	{
		Debug.LogWarning (text);
	}
}
