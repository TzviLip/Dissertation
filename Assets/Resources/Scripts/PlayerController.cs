//Avatars: People vector created by skydesign - www.freepik.com
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.AI;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField] Text roomText;
    [SerializeField] Button enterRoom;
    FirebaseDatabase databaseReference;

    void Start()
    {
        databaseReference = FirebaseDatabase.DefaultInstance; //Get a reference to the database
        var userid = GameManager.getID(); //Get the current user id
        getAvatar(userid); //Get the students avatar from the database

        agent = GetComponent<NavMeshAgent>(); //Get the NavMeshAgent for movement
        agent.updateRotation = false; //dont allow rotation
        agent.updateUpAxis = false; //Dont allow Up Axis update

        enterRoom.GetComponent<Button>().onClick.AddListener(delegate { loadOverworldScene(); }); //Add a listener for when a room Enter Button is clicked to load the correct scene
    }

    //Load the Currently Selected Scene
    public void loadOverworldScene()
    {
        string targetRoom = GameManager.getTargetRoom(); //Get the target room from the Game Manager
        Debug.Log("Scene Loading: " + targetRoom);
        SceneManager.LoadScene(targetRoom); //Load the Room
    }


    void Update()
    {
        if (Input.GetMouseButtonDown(0)) //When Left Mouse is pressed
        {
            var target = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get the clicked position and conver to word space
            target.z = 0; //Remove the z-coord
            Debug.Log("Player Targer: " + target);
            agent.SetDestination(target); //Set the destination using the NavMeshAgent
        }
    }

    //Get the Player Avatar from the Database and load the correct sprite from resources
    public void getAvatar(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("imageURLAvatar").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //Should Not occur unless database cannot be accessed. Should not have reached this point
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Sprite sp = Resources.Load<Sprite>("Sprites/Avatars/" + snapshot.Value.ToString()); //Load the correct sprite
                
                if (sp == null) //if no avatar found
                {
                    sp = Resources.Load<Sprite>("Sprites/Avatars/tile000 (1)"); //Set the default avatar
                    databaseReference.GetReference("users").Child(userid).Child("imageURLAvatar").SetValueAsync("tile000 (1)"); //Upload the default avatar
                    Debug.Log("No Avatar Found");
                }

                gameObject.GetComponent<SpriteRenderer>().sprite = sp; //Set the Sprite of the Player
            }
        });
    }

    //When the Player enters a room collider
    private void OnTriggerEnter2D(Collider2D collision)
    {
        var collisionName = collision.name; //get the room name
        roomText.text = collisionName; //Set the UI text
        Debug.Log("Room Entered: " + collisionName);

        enterRoom.GetComponentInChildren<Text>().text = "Enter"; //Set the button text
        GameManager.setTargetRoom(collisionName.Replace(" ", String.Empty)); //Set the Target Room in the Game Manager
        enterRoom.interactable = true; //Allow the button to be pressed

        //Check which room was entered, disable button if room is still locked and display locked text
        if (collision.name == "Workshop")
        {
            enterRoom.gameObject.SetActive(false);
            return;
        }
        else if (collision.name == "Drawing Room")
        {
            if (!GameManager.getDrawUnlocked())
            {
                enterRoom.GetComponentInChildren<Text>().text = "Locked";
                enterRoom.interactable = false;
            }
        }
        else if (collision.name == "Modelling Room")
        {
            if (!GameManager.getModelUnlocked())
            {
                enterRoom.GetComponentInChildren<Text>().text = "Locked";
                enterRoom.interactable = false;
            }
        }
        else if (collision.name == "Quest Room")
        {
            if (!GameManager.getQuestUnlocked())
            {
                enterRoom.GetComponentInChildren<Text>().text = "Locked";
                enterRoom.interactable = false;
            }
        }
        else if (collision.name == "Demonstration Room")
        {
            if (!GameManager.getDemonstrationUnlocked())
            {
                enterRoom.GetComponentInChildren<Text>().text = "Locked";
                enterRoom.interactable = false;
            }
        }

        enterRoom.gameObject.SetActive(true);
    }
}
