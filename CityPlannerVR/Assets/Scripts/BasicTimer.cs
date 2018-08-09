using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Timer 
{
	public string name;
	public bool isFirstRound = true;
}

public class WaitTimer : Timer
{
	public float waitTime = 1f;
	public float waitTimer;
}

public class GeneralTimer : Timer
{
	public float genTimer;
	public bool shouldStop = false;
}

public class IntervalTimer : Timer
{
	public float interTimer;
	public float interval;
	public float currentInterval = 0;
	public bool shouldStop = false;
}

public class BasicTimer : MonoBehaviour {

	//Timer events
	public static event Action WaitTimerEnded;
	public static event Action GeneralTimerStopped;
	public static event Action IntervalTimerStopped;
	public static event Action<string> IntervalReached;

	//STILL NEED STOPPERS

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
				if (WaitTimerEnded != null) {
					WaitTimerEnded ();
				}
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
				if (GeneralTimerStopped != null) {
					GeneralTimerStopped ();
				}
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
				//Debug.LogWarning ("IntervalTimer " + timer.name + " interval reached!");
				timer.currentInterval = 0;
				if (IntervalReached != null) {
					IntervalReached (timer.name);
				}
			}

			if (timer.shouldStop) {
				Debug.LogWarning ("IntervalTimer " + timer.name + " stopped!");
				interTimers.Remove (timer);
				if (IntervalTimerStopped != null) {
					IntervalTimerStopped ();
				}
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

	//Timertype 1 = wait, 2 = general, 3 = interval
	private Timer GetTimerByName(int timerType, string timerName)
	{
		if (timerType == 1) {
			foreach (WaitTimer wt in waitTimers) {
				if (wt.name.Equals (timerName)) {
					return wt as WaitTimer;
				}
			}
			return null;
		} else if (timerType == 2) {
			foreach (GeneralTimer gt in genTimers) {
				if (gt.name.Equals (timerName)) {
					return gt as GeneralTimer;
				}
			}
			return null;
		} else if (timerType == 3) {
			foreach (IntervalTimer it in interTimers) {
				if (it.name.Equals (timerName)) {
					return it as IntervalTimer;
				}
			}
			return null;
		} else {
			return null;
		}
	}

	public string GetCurrentTime(int timerType, string timerName)
	{
		Timer timer = GetTimerByName (timerType, timerName);
		if (timerType == 1) {
			WaitTimer wt = timer as WaitTimer;
			return wt.waitTimer.ToString ();
		} else if (timerType == 2) {
			GeneralTimer gt = timer as GeneralTimer;
			return gt.genTimer.ToString ();
		} else if (timerType == 3) {
			IntervalTimer it = timer as IntervalTimer;
			return it.interTimer.ToString ();
		} else {
			return null;
		}
	}
}
