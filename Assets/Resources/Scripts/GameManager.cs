using System.Collections;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    static GameManager instance;
    static string id;
    static string targetRoom;
    [SerializeField] static bool drawUnlocked = false;
    [SerializeField] static bool modelUnlocked = false;
    [SerializeField] static bool questUnlocked = false;
    [SerializeField] static bool demonstrationUnlocked = false;
    [SerializeField] static bool submitted = false;

    void Update()
    {
        //Quit when pressing escape
        if (Input.GetKeyDown("escape"))
        {
            Application.Quit();
            Debug.Log("Quit");
        }

    }

    void Awake()
    {
        //Singleton
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
            Destroy(gameObject);
    }



    //Get Input Text and Make a New User
    public void createUser()
    {
        var nameInput = GameObject.FindWithTag("NameInput").GetComponent<InputField>().text.ToString().ToLower(); //Get Name, reduce to Lower Case
        var userIdInput = GameObject.FindWithTag("IDInput").GetComponent<InputField>().text.ToString().ToLower(); //Get ID, reduce to Lower Case

        if (!string.IsNullOrWhiteSpace(nameInput) && !string.IsNullOrWhiteSpace(userIdInput)) //Only create if not empty otherwise database is overwritten entirely
        {
            writeNewUser(userIdInput, nameInput, "teacher1");
        }

        Debug.Log("Creating User: " + userIdInput);
    }

    //Get Input Text and Make a New Teacher
    public void createTeacher()
    {
        var nameInput = GameObject.FindWithTag("NameInput").GetComponent<InputField>().text.ToString().ToLower(); //Get Name, reduce to Lower Case
        var teacherIdInput = GameObject.FindWithTag("IDInput").GetComponent<InputField>().text.ToString().ToLower(); //Get ID, reduce to Lower Case

        if (!string.IsNullOrWhiteSpace(nameInput) && !string.IsNullOrWhiteSpace(teacherIdInput)) //Only create if not empty otherwise database is overwritten entirely
        {
            writeNewTeacher(teacherIdInput, nameInput, "");
        }

        Debug.Log("Creating Teacher: " + teacherIdInput);
    }

    //Add a user to the database
    private void writeNewUser(string userId, string name, string teacher)
    {
        Debug.Log("Adding User to Database");
        User user = new User(name, teacher); //Create a new user
        string json = JsonUtility.ToJson(user); //Serialise the variables

        FirebaseDatabase.DefaultInstance.RootReference.Child("users").Child(userId).SetRawJsonValueAsync(json); //Get Root reference and create user by JSON
    }

    //Add a teacher to the database
    private void writeNewTeacher(string userId, string name, string prompt)
    {
        Debug.Log("Adding Teacher to Database");
        Teacher teacher = new Teacher(name, prompt); //Create a new teacher
        string json = JsonUtility.ToJson(teacher); //Serialise the variables

        FirebaseDatabase.DefaultInstance.RootReference.Child("teachers").Child(userId).SetRawJsonValueAsync(json); //Get Root reference and create teacher by JSON
    }

    //Check that ID exists
    public void checkID()
    {
        Debug.Log("Checking ID");
        var popup = GameObject.FindGameObjectWithTag("Canvas").transform.Find("Popup"); //Get a reference to the popup menu
        var studentButton = popup.Find("StudentButton"); //Get enter student room button
        var teacherButton = popup.Find("TeacherButton"); //Get enter teacher room button

        var idField = GameObject.FindWithTag("InputID"); //Get the id Input Field
        id = idField.GetComponent<InputField>().text.ToString().ToLower(); //get id text
        Debug.Log("ID from field: " + id);

        //Test that ID exisits in the user database
        FirebaseDatabase.DefaultInstance.GetReference("users").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Only occurs if application cannot connect, should not have reached this point
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot user in snapshot.Children) //Check all users for the chosen id
                {
                    if (id.Equals(user.Key)) //If user is found then activate buttons
                    {
                        Debug.Log("Student Found: " + user.Key);
                        popup.gameObject.SetActive(true);
                        studentButton.gameObject.SetActive(true);
                        teacherButton.gameObject.SetActive(false);
                    }
                }
            }
        });

        FirebaseDatabase.DefaultInstance.GetReference("teachers").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Only occurs if application cannot connect, should not have reached this point
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                foreach (DataSnapshot teacher in snapshot.Children) //Check all teachers for the chosen name
                {
                    if (id.Equals(teacher.Key)) //If teacher is found then activate buttons
                    {
                        Debug.Log("Teacher Found: " + teacher.Key);
                        popup.gameObject.SetActive(true);
                        studentButton.gameObject.SetActive(false);
                        teacherButton.gameObject.SetActive(true);
                    }
                }
            }
        });
    }

    //Getters and Setters for Static Variables
    public static void setDrawUnlocked(bool toSet)
    {
        drawUnlocked = toSet;
    }

    public static bool getDrawUnlocked()
    {
        return drawUnlocked;
    }

    public static void setModelUnlocked(bool toSet)
    {
        modelUnlocked = toSet;
    }

    public static bool getModelUnlocked()
    {
        return modelUnlocked;
    }

    public static void setQuestUnlocked(bool toSet)
    {
        questUnlocked = toSet;
    }

    public static bool getQuestUnlocked()
    {
        return questUnlocked;
    }

    public static void setDemonstrationUnlocked(bool toSet)
    {
        demonstrationUnlocked = toSet;
    }

    public static bool getDemonstrationUnlocked()
    {
        return demonstrationUnlocked;
    }

    public static void setTargetRoom(string toSet)
    {
        targetRoom = toSet;
    }

    public static string getTargetRoom()
    {
        return targetRoom;
    }

    public static void setID(string toSet)
    {
        id = toSet;
    }

    public static string getID()
    {
        return id;
    }

    public static void setSubmitted(bool toSet)
    {
        submitted = toSet;
    }

    public static bool getSubmitted()
    {
        return submitted;
    }


}
