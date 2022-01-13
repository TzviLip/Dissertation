using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject ColourMenu, CreateMenu, ResizeMenu;
    [SerializeField] private Material selectedMaterial;
    [SerializeField] private GameObject ModelCanvas, IntroModelCanvas;
    [SerializeField] private GameObject ModelController;
    [SerializeField] private int mode = 0;
    [SerializeField] private int resizeMode = 1;

    //Swap between Introduction and Modelling Canvas
    public void swapView()
    {
        Debug.Log("View Swapped");
        ModelCanvas.SetActive(!ModelCanvas.activeSelf);
        IntroModelCanvas.SetActive(!IntroModelCanvas.activeSelf);
        ModelController.SetActive(!ModelController.activeSelf);
    }

    //Set Mode to Create and activate buttons
    public void create()
    {
        Debug.Log("Create Menu");
        ColourMenu.SetActive(false);
        CreateMenu.SetActive(true);
        ResizeMenu.SetActive(false);
        mode = 0;
    }

    //Set Mode to Move and activate buttons
    public void move()
    {
        Debug.Log("Move Menu");
        ColourMenu.SetActive(false);
        CreateMenu.SetActive(false);
        ResizeMenu.SetActive(false);
        mode = 1;
    }

    //Set Mode to Colour and activate buttons
    public void colour()
    {
        Debug.Log("Colour Menu");
        ColourMenu.SetActive(true);
        CreateMenu.SetActive(false);
        ResizeMenu.SetActive(false);
        mode = 2;
    }

    //Set Mode to Resize and activate buttons
    public void resize()
    {
        Debug.Log("Resize Menu");
        ColourMenu.SetActive(false);
        CreateMenu.SetActive(false);
        ResizeMenu.SetActive(true);
        mode = 3;
    }

    //Set Mode to Rotate and activate buttons
    public void rotate()
    {
        Debug.Log("Rotate Menu");
        ColourMenu.SetActive(false);
        CreateMenu.SetActive(false);
        ResizeMenu.SetActive(false);
        mode = 4;
    }

    //Set Mode to Delete and activate buttons
    public void delete()
    {
        Debug.Log("Delete Menu");
        ColourMenu.SetActive(false);
        CreateMenu.SetActive(false);
        ResizeMenu.SetActive(false);
        mode = 5;
    }

    //Pause the game while saving to prevent changing the model
    public void togglePause()
    {
        Debug.Log("Toggle Pause");
        mode *= -1; //negative mode does nothing but allows changing back to selected mode
        Button[] allChildren = ModelCanvas.GetComponentsInChildren<Button>(); //get All Buttons to activate/deactivate
        foreach (Button button in allChildren)
        {
            button.interactable = !button.IsInteractable(); //toggle interactable
        }
    }

    //Variable Getters and Setters
    public void setColour(Material mat)
    {
        selectedMaterial = mat;
    }

    public void setMode(int toSet)
    {
        mode = toSet;
    }

    public int getMode()
    {
        return mode;
    }

    public void setResizeMode(int toSet)
    {
        resizeMode = toSet;
    }

    public int getResizeMode()
    {
        return resizeMode;
    }

    public Material getSelectedMaterial()
    {
        return selectedMaterial;
    }

    public void enlarge()
    {
        resizeMode = 1;
    }

    public void reduce()
    {
        resizeMode = -1;
    }
}
