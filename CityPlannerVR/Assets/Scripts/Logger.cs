using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class CompleteLog
{
	public string userID;
	public string startTime;
	public List<LoggedAction> loggedActions;
	public List<LoggedPosition> userPositions;

}

[Serializable]
public class LoggedAction
{
	public string timestamp;
	public string action;
}

[Serializable]
public class LoggedPosition
{
	public string timestamp;
	public Vector3 position;
}

public class Logger : MonoBehaviour {

	//Logging syntax:
	//{"userID": xxxx,
	//"startTime": xxxx,
	//"loggedActions": [
	//{"timestamp": xxxx, "action": xxxx}
	//],
	//"userPositions": [
	//{"timestamp": xxxx, "position": {"x": xxx, "y": xxx, "z": xxx}}
	//]}


	#region Private Variables

	private CompleteLog logForThisUser;

	private string userID;
	private string startTime;

	private string currentJSON = "";

	private const string LOG_TIMER_NAME = "LoggingTimer";
	private const float LOG_SAVE_INTERVAL = 30;

	private const string POSITION_TIMER_NAME = "PositionTimer";
	private const float POSITION_TIMER_INTERVAL = 2;

	#endregion

	//LAST SAVE BEFORE QUITTING
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnDisable()
	{
		BasicTimer.IntervalReached -= HandleIntervalReached;
	}

	public void StartLogging()
	{
		StartTimers ();
		CreateJSON ();
	}

	private string GetUserID()
	{
		if (this.userID == null) {
		
		}

		return this.userID;
	}

	private void StartTimers()
	{
		this.startTime = DateTime.Now.Day.ToString () + "." + DateTime.Now.Month.ToString() + "." + DateTime.Now.Year.ToString() + "-";
		this.startTime += DateTime.Now.Hour.ToString () + "." + DateTime.Now.Minute.ToString () + "." + DateTime.Now.Second.ToString ();
		Debug.LogWarning ("Logging started at: " + this.startTime);
		this.gameObject.GetComponent<BasicTimer> ().StarIntervalTimer (LOG_TIMER_NAME, LOG_SAVE_INTERVAL);
		this.gameObject.GetComponent<BasicTimer> ().StarIntervalTimer (POSITION_TIMER_NAME, POSITION_TIMER_INTERVAL);
		BasicTimer.IntervalReached += HandleIntervalReached;
	}

	private void HandleIntervalReached(string name)
	{
		switch (name) {
			case LOG_TIMER_NAME: 
				//Tässä kirjoitus tiedostoon?
				break;
			case POSITION_TIMER_NAME: 
				SavePosition ();
				break;
			default:
				break;
		}

	}

	private string GetTimeSinceStart()
	{
		return this.gameObject.GetComponent<BasicTimer> ().GetCurrentTime (3, LOG_TIMER_NAME);
	}

	private void CreateJSON()
	{
		logForThisUser = new CompleteLog ();
		logForThisUser.userID = GetUserID ();
		logForThisUser.startTime = this.startTime;
		logForThisUser.loggedActions = new List<LoggedAction> ();
		logForThisUser.userPositions = new List<LoggedPosition> ();

		string jsonstring = JsonUtility.ToJson (logForThisUser);
		currentJSON = jsonstring;

		PrintWarningLog ("New JSON created! Currently stored JSON: " + currentJSON);

	}

	public void LogActionLine(string action)
	{
		LoggedAction newAction = new LoggedAction ();
		newAction.timestamp = GetTimeSinceStart ();
		newAction.action = action;

		logForThisUser.loggedActions.Add (newAction);

		string jsonupdated = JsonUtility.ToJson (logForThisUser);
		currentJSON = jsonupdated;

		PrintWarningLog ("New JSON line added! Currently stored JSON: " + currentJSON);
	}

	private void SavePosition()
	{
		LoggedPosition newPosition = new LoggedPosition ();
		newPosition.timestamp = GetTimeSinceStart ();
		newPosition.position = GameObject.Find ("Player").transform.position;

		logForThisUser.userPositions.Add (newPosition);

		string jsonupdated = JsonUtility.ToJson (logForThisUser);
		currentJSON = jsonupdated;

		PrintWarningLog ("New JSON line added! Currently stored JSON: " + currentJSON);
	}

	public void PrintWarningLog(string text)
	{
		Debug.LogWarning (text);
	}
}
