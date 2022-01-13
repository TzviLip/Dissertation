using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
using System;

public class RevealController : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageReference;
    FirebaseDatabase databaseReference;
    public GameObject Drawing, Model;
    private RawImage DrawingImage, ModelImage;
    public Image Avatar;


    // Start is called before the first frame update
    void Start()
    {
        DrawingImage = Drawing.GetComponent<RawImage>(); //Get the Drawing Image
        ModelImage = Model.GetComponent<RawImage>();  //Get the Modelling Image

        //Get Firebase References
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://dissertation-workshop.appspot.com");

        string userid = GameManager.getID(); //Get the students ID
        getStudent(userid); //Get the student images from the Database
    }

    //Get all student images form Database
    public void getStudent(string userid)
    {
        getDrawing(userid);
        getModel(userid);
        getAvatar(userid);
    }

    //Submit project as completed
    public void submit()
    {
        string userid = GameManager.getID(); //Get current student
        databaseReference.GetReference("users").Child(userid).Child("completed").SetValueAsync(true); //Set completed variable in database
        if (!GameManager.getSubmitted()) //check if student has submitted to only submit once per game
        {
            addPoints(userid); //increase points
            GameManager.setSubmitted(true);
        }
    }

    //Fetch Student Drawing from Database
    void getDrawing(string userid)
    {
        DownloadImageByID(userid, "imageURLDraw", DrawingImage);
    }

    //Fetch Student Model from Database
    void getModel(string userid)
    {
        DownloadImageByID(userid, "imageURLModel", ModelImage);
    }

    //Increase Gold and update server
    void addPoints(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("gold").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Should not occur
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                var points = 50;
                var currentGold = int.Parse(snapshot.Value.ToString()) + points; //Get the gold value and increase by 50
                databaseReference.GetReference("users").Child(userid).Child("gold").SetValueAsync(currentGold); //Set gold value on the server
                Debug.Log("Gold Updated: " + currentGold);
            }
        });
    }

    //Fetch Student Avatar from Database
    void getAvatar(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("imageURLAvatar").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Should not occur
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Avatar Found:" + snapshot.Value.ToString());

                Sprite sp = Resources.Load<Sprite>("Sprites/Avatars/" + snapshot.Value.ToString());  //Load Avatar Sprite from Resources
                Avatar.sprite = sp; //Set avatar
            }
        });
    }

    //Find the image to be downloaded
    public void DownloadImageByID(string userid, string valueToGet, RawImage toSet)
    {
        //Get the correct URL field from the database
        FirebaseDatabase.DefaultInstance.GetReference("users").Child(userid).Child(valueToGet).GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                DownloadScreenshot(snapshot.Value.ToString(), toSet); //Download the image from the database
                Debug.Log("Retrieved from URL: " + snapshot.Value);
            }
        });
    }

    //Dowload the Screenshot from storage using its name and path
    private void DownloadScreenshot(string fileName, RawImage toSet)
    {
        StorageReference image = storageReference.Child(fileName);
        image.GetDownloadUrlAsync().ContinueWithOnMainThread(task =>
        {
            if (!task.IsFaulted && !task.IsCanceled)
            {
                StartCoroutine(LoadImage(Convert.ToString(task.Result), toSet)); //Start Download
            }
            else
            {
                Debug.Log(task.Exception); //No Image Found, Occurs if images are deleted manually
            }
        });
    }

    //Request to fetch a texture using WebRequests
    IEnumerator LoadImage(string MediaUrl, RawImage toSet)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl); //Generate a request
        yield return request.SendWebRequest();
        if (request.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log(request.error); //Invalid Protocol
        }
        else
        {
            toSet.texture = ((DownloadHandlerTexture)request.downloadHandler).texture; //Set the texture
        }
    }
}
