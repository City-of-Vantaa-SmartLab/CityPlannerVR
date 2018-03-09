using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This fixes the bug where building jumped a bit, when tried to put object from one hand to another
/// </summary>
public class IsAttachedToHand : MonoBehaviour {

	int maxTimer = 2;
	int timer = 0;
	bool isInHand = false;

	public delegate void SnapDelegate();
	public event SnapDelegate OnSnapToGrid;

	private bool isHolding = false;

	public bool IsHolding {
		get {
			return isHolding;
		}
		set {
			isHolding = value;
		}
	}

    void Start(){
		timer = maxTimer;
    }

	//If the player wants to just put the object to their other hand, 
	//it cannot be considered off hand, because that causes some funky glitches
	IEnumerator CheckIfHandChanged(){

        isHolding = true;
        while (timer < maxTimer)
        {
            ++timer;
            yield return null;
        }

		//Debug.Log ("Nyt on kulunut riittävästi aikaa, että voidaan tarkistaa, onko asia vielä kädessä");
		if (!isInHand) {
			//Debug.Log ("Asia ei ole enää kädessä");
			isHolding = false;
			if (OnSnapToGrid != null) {
				OnSnapToGrid ();
			}
		}

        timer = 0;
		//else {
			//Debug.Log ("Asia on vielä kädessä");
		//}
	}

    private void OnAttachedToHand(Valve.VR.InteractionSystem.Hand hand)
	{
		isInHand = true;
        //StartCoroutine(CheckIfHandChanged ());
    }

	private void OnDetachedFromHand(Valve.VR.InteractionSystem.Hand hand)
	{
		isInHand = false;
		StartCoroutine(CheckIfHandChanged ());
	}
}
