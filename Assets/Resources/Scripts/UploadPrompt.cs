using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class UploadPrompt : MonoBehaviour
{
    DatabaseReference databaseReference;
    [SerializeField] private GameObject promptInputField;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference; //Get Reference to Firebase Database Root
    }

    //Upload Prompt to Firebase
    public void uploadPrompt()
    {
        string prompt = promptInputField.GetComponent<InputField>().text.ToString(); //Get Prompt Text to upload
        string teacherId = GameManager.getID(); //Get ID to Upload to

        //Don't Allow Empty Prompt
        if (prompt != "")
        {
            Debug.Log("Prompt Entered: " + prompt);
            databaseReference.Child("teachers").Child(teacherId).Child("prompt").SetValueAsync(prompt); //Find Teacher at set their prompt value
        }
        else
        {
            Debug.Log("Prompt Entered is empty");
        }
    }
}
