using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using UnityEngine.UI;

public class UploadEvaluation : MonoBehaviour
{
    DatabaseReference databaseReference;
    [SerializeField] private GameObject evaluationInputField;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance.RootReference; //Get Reference to Firebase Database Root
    }

    //Upload Evaluation to Firebase
    public void uploadEvaluation()
    {
        string evaluation = evaluationInputField.GetComponent<InputField>().text.ToString(); //Get the evaluation text to upload
        string studentId = GameManager.getID();  //Get ID to Upload to

        //Don't Allow Empty Prompt
        if (evaluation != "")
        {
            Debug.Log("Evaluation Entered: " + evaluation);
            databaseReference.Child("users").Child(studentId).Child("evaluation").SetValueAsync(evaluation);  //Find Student at set their evaluation value
        }
        else
        {
            Debug.Log("Evaluation Entered is empty");
        }
    }
}