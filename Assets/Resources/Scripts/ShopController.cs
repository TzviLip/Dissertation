using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;
using Firebase.Extensions;

public class ShopController : MonoBehaviour
{
    FirebaseDatabase databaseReference;

    [SerializeField] private int currentGold = 100;
    [SerializeField] private int SelectedCost;
    [SerializeField] private Sprite SelectedSprite;
    [SerializeField] private Image Character;
    [SerializeField] private Image SelectedItem;
    [SerializeField] private Text CurrentGoldText;
    [SerializeField] private Text Cost;
    [SerializeField] private GameObject Upload;


    void Start()
    {
        var id = GameManager.getID(); //Get Current Player
        databaseReference = FirebaseDatabase.DefaultInstance; //Get Reference to Database
        getGold(id); //Get Gold from Database
        getAvatar(id); //Get Avatar from Database
    }

    //Get the Players current Avatar from the Database Asynchronously
    public void getAvatar(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("imageURLAvatar").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception); //No Avatar Found, should never occur unless database is edited manually
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Avatar ID Code:" + snapshot.Value.ToString());

                Sprite sp = Resources.Load<Sprite>("Sprites/Avatars/" + snapshot.Value.ToString());
                Character.sprite = sp;
            }
        });
    }

    //Set the cost variable and text
    public void selectCharacterCost(int cost)
    {
        SelectedCost = cost;
        Cost.text = "Cost to Buy: " + cost;
    }

    //Set the sprite variable and in game sprite
    public void selectCharacterSprite(Sprite toSet)
    {
        SelectedSprite = toSet;
        SelectedItem.sprite = toSet;
    }

    //Use the currently selected avatar, reduce gold, upload to database
    public void buyCharacter()
    {
        if (SelectedSprite == Character.sprite)
        {
            Cost.text = "Already Selected";
        }
        else if (SelectedSprite != null)
        {
            if (SelectedCost <= currentGold)
            {
                Character.sprite = SelectedSprite;  //Set the Sprite
                currentGold -= SelectedCost;    //Reduce Gold
                Cost.text = "Item Purchased";
                CurrentGoldText.text = "Current Gold: " + currentGold;
                setGold(GameManager.getID()); //Set gold in the database
                uploadCharacterAvatar(); //Set avatar in the database
                Debug.Log("Character Bought: " + SelectedSprite);
            }
            else
            {
                Cost.text = "Not Enough Gold";
            }
        }

        SelectedSprite = null; //Reset the selected sprite to avoid buying twice
    }

    //Fetch the gold variable from the database
    public void getGold(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("gold").GetValueAsync().ContinueWithOnMainThread(task => {
            if (task.IsFaulted)
            {
                Debug.Log(task.Exception);
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                Debug.Log("Current Gold: " + snapshot.Value.ToString());
                currentGold = int.Parse(snapshot.Value.ToString()); //Set variable
                CurrentGoldText.text = "Current Gold: " + currentGold; //Set Text
            }
        });
    }

    //Set the gold variable (database) of the current User 
    public void setGold(string userid)
    {
        databaseReference.GetReference("users").Child(userid).Child("gold").SetValueAsync(currentGold);
    }

    //Set the sprite variable (database) of the current User 
    public void uploadCharacterAvatar()
    {
        Upload.GetComponent<UploadFile>().StartAvatarUpload(SelectedSprite);
    }
}
