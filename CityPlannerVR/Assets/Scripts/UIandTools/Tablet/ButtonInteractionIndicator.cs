using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractionIndicator : MonoBehaviour {

    SpriteRenderer sprite;
    Image image;
    int deviceIndex;

    /// <summary>
    /// Gets the deviceIndex of a hand used to press this button for haptic feedback call
    /// </summary>
    /// <param name="other">The hand that presses this button</param>
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.layer == LayerMask.NameToLayer("ButtonLayer"))
        {
            if(other.GetComponent<Valve.VR.InteractionSystem.Hand>() != null)
            {
                deviceIndex = (int)other.GetComponent<Valve.VR.InteractionSystem.Hand>().controller.index;
            }
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

    //--------------------------------------------------------------------------------------------------------------------------------------

    public void OnHoverUI()
    {
        if(image == null)
        {
            image = GetComponent<Image>();
        }

        image.color = Color.gray;
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(500);
    }

    public void OnStopHoverUI()
    {
        image.color = Color.white;
    }

}
