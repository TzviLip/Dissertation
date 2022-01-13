//Inventory System Modified From:
//Title: Unity UI - Scroll Menu Pt 5: Inventory / Grid Menu - A grid based, inventory style menu.
//Author: c00pala
//Date: 18/5/2017
//Availability: https://www.youtube.com/watch?v=Oba1k4wRy-0

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopGridControl : MonoBehaviour
{
    
    public int numAvatars = 66;
    public GameObject shopController;

    private List<Avatar> shopInventory;

    [SerializeField]
    private GameObject buttonTemplate;
    [SerializeField]
    private GridLayoutGroup gridGroup;
    [SerializeField]
    private Sprite[] avatarSprites;

    private void Start()
    {
        shopInventory = new List<Avatar>();

        //For each avatar sprite, add them to the shop. Increase cost of each new row by 10
        for (int i = 1; i <= numAvatars; i++)
        {
            Avatar newAvatar = new Avatar();
            newAvatar.iconSprite = avatarSprites[i - 1]; //Set Sprite
            int costTemp = (int) Mathf.Floor((i / 10) + 1); //Get the current row
            newAvatar.cost = costTemp * 10; //Calculate the cost

            shopInventory.Add(newAvatar);
        }

        generateButtons();
    }

    //Generate the buttons for the shop
    void generateButtons()
    {
        Debug.Log("Generating Shop");
        //Used if shop has less than 10 items for visibility
        if (shopInventory.Count < 11)
        {
            gridGroup.constraintCount = shopInventory.Count;
        }
        else
        {
            gridGroup.constraintCount = 10;
        }

        foreach (Avatar avatar in shopInventory)
        {
            GameObject newButton = Instantiate(buttonTemplate) as GameObject; //Create the button
            newButton.SetActive(true);
            newButton.GetComponent<Button>().onClick.AddListener(delegate { setControllerVariables(avatar.cost, avatar.iconSprite); }); //Add a Listener for when it is clicked
            newButton.GetComponent<ShopButton>().setIcon(avatar.iconSprite); //Set the sprite
            newButton.GetComponent<ShopButton>().setCost(avatar.cost);  //Set the Cost
            newButton.transform.SetParent(buttonTemplate.transform.parent, false); //Set the parent for clean hierarchy and being able to find later
        }

        Debug.Log("Shop Generated");
    }

    //OnClick Event, set the ShopController Variables
    public void setControllerVariables(int cost, Sprite icon)
    {
        shopController.GetComponent<ShopController>().selectCharacterCost(cost); //Set ShopController Cost
        shopController.GetComponent<ShopController>().selectCharacterSprite(icon); //Set ShopController Icon
    }

    //Default Avatar
    public class Avatar
    {
        public Sprite iconSprite;
        public int cost;
    }
}
