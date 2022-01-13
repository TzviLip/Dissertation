//Screenshot System Modified From:
//Title: Getting started with Cloud Storage for Firebase - Firecasts
//Author: Firebase
//Date: 7/5/2020
//Availability: https://www.youtube.com/watch?v=JAaCUmQ6LBo&list=WL&index=21&t=3s

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class TakeScreenshot : MonoBehaviour, IPointerClickHandler
{

    public ScreenshotEvent onScreenCaptured = new ScreenshotEvent();

    //Called When Button is Clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        StartCoroutine(TakeScreenshotCoroutine());
    }

    private IEnumerator TakeScreenshotCoroutine()
    {
        yield return null;
        GameObject.FindWithTag("Canvas").GetComponent<Canvas>().enabled = false; //Disable Canvas for better picture

        yield return new WaitForEndOfFrame(); //To Ensure no updates are currently running
        var screenshot = ScreenCapture.CaptureScreenshotAsTexture(); //Save the current screen as a texture
        onScreenCaptured.Invoke(screenshot); //Call event to note the save and upload
        Debug.Log("Screenshot Saved");

        GameObject.FindWithTag("Canvas").GetComponent<Canvas>().enabled = true; //Enable Canvas after upload
    }

    [System.Serializable]
    public class ScreenshotEvent : UnityEvent<Texture2D>
    {
    }
}
