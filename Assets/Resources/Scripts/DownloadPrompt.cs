using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.UI;

public class DownloadPrompt : MonoBehaviour
{
    FirebaseDatabase databaseReference;
    [SerializeField] GameObject promptText;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance; //Get a reference to the database
        var studentId = GameManager.getID(); //Get the Current ID
        getTeacher(studentId); //Get the Students teacher from the database
    }

    //Get the Teacher using a given student ID
    void getTeacher(string studentId)
    {
        var teacher = databaseReference.GetReference("users").Child(studentId).Child("teacher").GetValueAsync().ContinueWithOnMainThread(task => {
             if (task.IsFaulted)
             {
                 Debug.Log(task.Exception); //No teacher found, should never occur unless database is edited manually
             }
             else if (task.IsCompleted)
             {
                DataSnapshot snapshot = task.Result;
                var teacher = snapshot.Value.ToString(); //Convert the Value to a string
                Debug.Log("Teacher Loaded from Database: " + teacher);
                getPrompt(teacher); //Get the Prompt from the database
            }
         });
    }

    //Get the prompt using a given teacherID
    void getPrompt(string teacherId)
    {
        databaseReference.GetReference("teachers").Child(teacherId).Child("prompt").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //No prompt submitted by given teacher
                promptText.GetComponent<Text>().text = "No brief found, ask your teacher to input a brief";
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Prompt Text: " + snapshot.Value);
                promptText.GetComponent<Text>().text = snapshot.Value.ToString(); //Set the prompt text to the teacher prompt found
            }
        });
    }

}
