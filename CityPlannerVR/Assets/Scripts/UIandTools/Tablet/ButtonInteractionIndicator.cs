using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInteractionIndicator : MonoBehaviour {

    SpriteRenderer sprite;
    Image image;
    MeshRenderer meshRenderer;
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

    /// <summary>
    /// Used if the used to hover on an object with a sprite renderer (ie. a tablet button)
    /// </summary>
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
    /// <summary>
    /// Used if the used to hover on an object with a image component (ie. a play and pause button button)
    /// </summary>
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

    //--------------------------------------------------------------------------------------------------------------------------------------
    /// <summary>
    /// Used if the used to hover on an object with a mesh renderer (ie. the facade button next R-kioski)
    /// </summary>
    public void OnHoverObject()
    {
        if(meshRenderer == null)
        {
            meshRenderer = GetComponent<MeshRenderer>();
        }

        meshRenderer.material.color = Color.gray;
        SteamVR_Controller.Input(deviceIndex).TriggerHapticPulse(500);
    }

    public void OnStopHoverObject()
    {
        meshRenderer.material.color = Color.white;
    }

}
