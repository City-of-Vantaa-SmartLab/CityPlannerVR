/**
 * Copyright (c) 2017 The Campfire Union Inc - All Rights Reserved.
 *
 * Licensed under the MIT license. See LICENSE file in the project root for
 * full license information.
 *
 * Email:   info@campfireunion.com
 * Website: https://www.campfireunion.com
 */

using UnityEngine;
using System;
using System.Text.RegularExpressions;
using System.Collections;

namespace VRKeys
{

    /// <summary>
    /// Example use of VRKeys keyboard.
    /// </summary>
    public class DemoScene : MonoBehaviour
    {

        /// <summary>
        /// Reference to the VRKeys keyboard.
        /// </summary>
        public Keyboard keyboard;
        public GameObject vrKeyboard;
        public GameObject hoverTablet;

        public bool keyboardOn;

        /// <summary>
        /// Show the keyboard with a custom input message. Attaching events dynamically,
        /// but you can also use the inspector.
        /// </summary>
        /// 

        private void Start()
        {
            vrKeyboard = GameObject.Find("VRKeys");
            vrKeyboard.SetActive(false);
            keyboardOn = false;
            Debug.Log("Keyboard found");
        }


        /// <summary>
        /// Press space to show/hide the keyboard.
        ///
        /// Press Q for Qwerty keyboard, D for Dvorak keyboard, and F for French keyboard.
        /// </summary>
        private void Update()
        {
            if (!vrKeyboard)
            {

            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (keyboard.disabled)
                    {
                        keyboard.Enable();
                    }
                    else
                    {
                        keyboard.Disable();
                    }
                }

                if (keyboard.disabled)
                {
                    return;
                }
            }

        }

        /// <summary>
        /// Hide the validation message on update. Connect this to OnUpdate.
        /// </summary>
        public void HandleUpdate(string text)
        {
            keyboard.HideValidationMessage();
        }

        /// <summary>
        /// Validate the email and simulate a form submission. Connect this to OnSubmit.
        /// </summary>
        public void HandleSubmit(string text)
        {
            keyboard.DisableInput();
            string temptext = text;
            GameObject targetobject = HoverTabletManager.CommentTarget;
            Comment.GenerateTextComment(temptext, targetobject, "wadap");


            //if (!ValidateEmail (text)) {
            //	keyboard.ShowValidationMessage ("Please enter a valid email address");
            //	keyboard.EnableInput ();
            //	return;
            //}

            StartCoroutine(SubmitEmail(text));
        }

        public void HandleCancel()
        {
            Debug.Log("Cancelled keyboard input!");
        }

        public void KeyboardOn()
        {
            if (!keyboardOn)
            {
                Debug.Log("Keyboard On");
                keyboardOn = true;
                vrKeyboard.SetActive(true);
                Debug.Log("Keyboard set up");
                keyboard.Enable();
                Debug.Log("Keyboard set up2");
                keyboard.SetPlaceholderMessage("Kirjoita kommentti");

                keyboard.OnUpdate.AddListener(HandleUpdate);
                keyboard.OnSubmit.AddListener(HandleSubmit);
                keyboard.OnCancel.AddListener(HandleCancel);
            }
            else if (keyboardOn)
            {
                Debug.Log("Did something");
                keyboardOn = false;
                vrKeyboard.SetActive(false);
                keyboard.OnUpdate.RemoveListener(HandleUpdate);
                keyboard.OnSubmit.RemoveListener(HandleSubmit);
                keyboard.OnCancel.RemoveListener(HandleCancel);

                keyboard.Disable();
            }
            else
                Debug.Log("Keyboard not found");
        }


        /// <summary>
        /// Pretend to submit the email before resetting.
        /// </summary>
        private IEnumerator SubmitEmail(string email)
        {
            keyboard.ShowInfoMessage("Tallennetaan kommenttia");

            yield return new WaitForSeconds(2f);

            keyboard.ShowSuccessMessage("Kommenttisi on tallennettu");

            yield return new WaitForSeconds(2f);

            keyboard.HideSuccessMessage();
            keyboard.SetText("");
            keyboard.EnableInput();
        }
    }
}