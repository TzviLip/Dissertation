using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Storage;
using Firebase.Extensions;
using System;
using UnityEngine.Networking;

public class DropdownController : MonoBehaviour
{
    FirebaseStorage storage;
    StorageReference storageReference;
    FirebaseDatabase databaseReference;
    [SerializeField] GameObject Drawing, Model;
    private RawImage DrawingImage, ModelImage;
    [SerializeField] Image Avatar;
    [SerializeField] Text evalText;

    void Start()
    {
        DrawingImage = Drawing.GetComponent<RawImage>(); //Get the Drawing Image
        ModelImage = Model.GetComponent<RawImage>(); //Get the Modelling Image

        //Get Firebase References
        storage = FirebaseStorage.DefaultInstance;
        databaseReference = FirebaseDatabase.DefaultInstance;
        storageReference = storage.GetReferenceFromUrl("gs://dissertation-workshop.appspot.com");

        //Get the dropdown menu and clear to remove old names
        var dropdown = transform.GetComponent<Dropdown>();
        dropdown.options.Clear();
        dropdown.options.Add(new Dropdown.OptionData() { text = "" });

        generateDropdown(dropdown); //Generate the dropdown menu

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); }); //Listener for when values are clicked
    }


    //Item Selected
    void DropdownItemSelected(Dropdown dropdown)
    {
        int index = dropdown.value; //Get the selected item by index
        if (dropdown.options[index].text == "") //Ignore top item
        {
            Avatar.sprite = null;
            DrawingImage.texture = null;
            ModelImage.texture = null;
        }
        else
        {
            getStudentImages(dropdown.options[index].text); //Get the students images
            getStudentEvaluation(dropdown.options[index].text); //Get the students evaluation
        }
    }

    //Add completed students to dropdown Menu
    public void generateDropdown(Dropdown dropdown)
    {
        Debug.Log("Generating Dropdown");
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Should not occur
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children) //Get all students in the database
                {
                    IDictionary dictUser = (IDictionary)user.Value;
                    if (dictUser["completed"].ToString().ToLower().Equals("true") && dictUser["teacher"].ToString().ToLower().Equals(GameManager.getID())) //Add students if they are completed and the current teacher is theirs
                    {
                        dropdown.options.Add(new Dropdown.OptionData() { text = user.Key.ToString() }); //Add to the dropdown menu
                    } 
                }
            }
        });
    }

    //Fetch the student evaluation from the database
    public void getStudentEvaluation(string studentid)
    {
        databaseReference.GetReference("users").Child(studentid).Child("evaluation").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Should not occur
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                evalText.text = studentid.ToUpper() + " SELF-EVALUATION\n\n" + snapshot.Value.ToString(); //Set the evaluation text to the text found
            }
        });
    }

    //Get all student images form Database
    public void getStudentImages(string studentid)
    {
        getDrawing(studentid);
        getModel(studentid);
        getAvatar(studentid);
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

                Sprite sp = Resources.Load<Sprite>("Sprites/Avatars/" + snapshot.Value.ToString()); //Load Avatar Sprite from Resources
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
