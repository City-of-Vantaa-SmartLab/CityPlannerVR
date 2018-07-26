using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitTimer
{
	public string name;
	public float waitTime = 1f;
	public float waitTimer;
	public bool isFirstRound = true;
}

public class GeneralTimer
{
	public string name;
	public float genTimer;
	public bool isFirstRound = true;
	public bool shouldStop = false;
}

public class IntervalTimer
{
	public string name;
	public float interTimer;
	public float interval;
	public float currentInterval = 0;
	public bool isFirstRound = true;
	public bool shouldStop = false;
}

public class BasicTimers : MonoBehaviour {

	//STILL NEED STOPPERS AND EVENTS FOR WAIT END AND INTERVAL AND GETTTERS FOR CURRENT TIME

	private List<WaitTimer> waitTimers = new List<WaitTimer>();
	private List<GeneralTimer> genTimers = new List<GeneralTimer>();
	private List<IntervalTimer> interTimers = new List<IntervalTimer> ();
	
	// Update is called once per frame
	void Update () {
		foreach(WaitTimer timer in waitTimers){
			if (timer.isFirstRound) {
				timer.waitTimer = Time.time - timer.waitTimer;
				timer.isFirstRound = false;
			} else {
				timer.waitTimer += Time.deltaTime;
			}

			if (timer.waitTimer > timer.waitTime) {
				Debug.LogWarning ("WaitTimer " + timer.name + " completed!");
				waitTimers.Remove (timer);
			}
		}

		foreach(GeneralTimer timer in genTimers){
			if (timer.isFirstRound) {
				timer.genTimer = Time.time - timer.genTimer;
				timer.isFirstRound = false;
			} else {
				timer.genTimer += Time.deltaTime;
			}

			if (timer.shouldStop) {
				Debug.LogWarning ("GeneralTimer " + timer.name + " stopped!");
				genTimers.Remove (timer);
			}
		}

		foreach(IntervalTimer timer in interTimers){
			if (timer.isFirstRound) {
				timer.interTimer = Time.time - timer.interTimer;
				timer.currentInterval += timer.interTimer;
				timer.isFirstRound = false;
			} else {
				timer.interTimer += Time.deltaTime;
				timer.currentInterval += Time.deltaTime;
			}

			if (timer.currentInterval >= timer.interval) {
				Debug.LogWarning ("IntervalTimer " + timer.name + " interval reached!");
				timer.currentInterval = 0;
			}

			if (timer.shouldStop) {
				Debug.LogWarning ("IntervalTimer " + timer.name + " stopped!");
				interTimers.Remove (timer);
			}
		}

	}

	public void StartWaitTimer(string timerName, float timeInSeconds)
	{
		float tmpTime = Time.time;
		WaitTimer newTimer = new WaitTimer ();
		newTimer.name = timerName;
		newTimer.waitTime = timeInSeconds;
		newTimer.waitTimer = tmpTime;
		waitTimers.Add (newTimer);
		Debug.LogWarning ("WaitTimer " + newTimer.name + " active!");
	}

	public void StarGeneralTimer(string timerName)
	{
		float tmpTime = Time.time;
		GeneralTimer newTimer = new GeneralTimer ();
		newTimer.name = timerName;
		newTimer.genTimer = tmpTime;
		genTimers.Add (newTimer);
		Debug.LogWarning ("GeneralTimer " + newTimer.name + " active!");
	}

	public void StarIntervalTimer(string timerName, float intervalInSeconds)
	{
		float tmpTime = Time.time;
		IntervalTimer newTimer = new IntervalTimer ();
		newTimer.name = timerName;
		newTimer.interval = intervalInSeconds;
		newTimer.interTimer = tmpTime;
		interTimers.Add (newTimer);
		Debug.LogWarning ("IntervalTimer " + newTimer.name + " active!");
	}
}
