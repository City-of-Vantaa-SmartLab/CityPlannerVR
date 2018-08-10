using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractionIndicator : MonoBehaviour {

    SpriteRenderer sprite;
    int deviceIndex;

    /// <summary>
    /// Gets the deviceIndex of a hand used to press this button for haptic feedback call
    /// </summary>
    /// <param name="other">The hand that presses this button</param>
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.layer == LayerMask.NameToLayer("ButtonLayer"))
        {
            deviceIndex = (int)other.GetComponent<Valve.VR.InteractionSystem.Hand>().controller.index;
        }

    }

    public void OnHoverSprite()
    {
        if (sprite == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        sprite.color = Color.gray;
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(500);
    }

    public void OnStopHoverSprite()
    {
        sprite.color = Color.white;
    }


}
