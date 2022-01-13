using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Extensions;
using Firebase.Storage;
using Firebase.Database;

public class UploadFile : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageReference;
    DatabaseReference databaseReference;

    void Start()
    {
        //Get Firebase references
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference;
        storageReference = storage.GetReferenceFromUrl("gs://dissertation-workshop.appspot.com");
    }

    //Upload a Drawing Texture
    public void StartDrawUpload(Texture2D screenshot)
    {
        StartCoroutine(UploadCoroutine(screenshot, "imageURLDraw"));
    }

    //Upload a Model Texture
    public void StartModelUpload(Texture2D screenshot)
    {
        StartCoroutine(UploadCoroutine(screenshot, "imageURLModel"));
    }

    //Upload an Avatar Sprite
    public void StartAvatarUpload(Sprite sprite)
    {
        var userid = GameManager.getID();  //Get the current student
        var uploadURL = sprite.name; //get the sprite to upload
        setImageUrl(userid, uploadURL, "imageURLAvatar");
    }

    //Set the specified value to a given URL in the database
    private void setImageUrl(string userId, string imageURL, string valueToSet)
    {
        databaseReference.Child("users").Child(userId).Child(valueToSet).SetValueAsync(imageURL);
    }

    //Upload the image to the database by converting to bytes
    IEnumerator UploadCoroutine(Texture2D screenshot, string destination)
    {
        Debug.Log("Convert To Bytes");
        var bytes = screenshot.EncodeToPNG(); //PNG Encoding

        //Set metaData to note that image is a png
        var newMetaData = new MetadataChange()
        {
            ContentType = "image/png"
        };

        var userid = GameManager.getID(); //Get current student ID
        var uploadURL = "uploads/" + userid + destination + ".png"; //Generate the file path
        var uploadRef = storageReference.Child(uploadURL); //Get a reference to the storage
        setImageUrl(userid, uploadURL, destination); //Set the URL in the database for later retrieval
        Debug.Log("File Upload Started");
        var uploadTask = uploadRef.PutBytesAsync(bytes, newMetaData).ContinueWithOnMainThread((task) =>
        {
            if (task.IsFaulted || task.IsCanceled)
            {
                Debug.Log(task.Exception.ToString()); //Should not occur unless database goes offline while uploading

            }
            else
            {
                Debug.Log("File Uploaded Successfully to: " + uploadURL);
            }
        });

        yield return new WaitUntil(() => uploadTask.IsCompleted);
    }
}
