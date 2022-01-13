//Paint Can: Umer Younas from the Noun Project (thenounproject.com)

//Drawing System Modified From:
//Title: Unity Drawing Lines with Mouse Position - Line Renderer and Edge Collider
//Author: Info Gamer
//Date: 8/11/2017
//Availability: https://www.youtube.com/watch?v=pa_U64G7gkE

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawController : MonoBehaviour
{
    static DrawController instance;

    public List<Vector2> touchPositions;

    [SerializeField] GameObject linePrefab;
    [SerializeField] List<Material> lineMaterials;  
    [SerializeField] float currentWidth;
    [SerializeField] List<GameObject> drawnLines;
    [SerializeField] GameObject lineContainer;
    [SerializeField] Button colourButton;
    [SerializeField] Button WidthButton;

    private Camera mainCamera;
    private GameObject currentLine;
    private LineRenderer lineRenderer;
    private Material currentMaterial;
    private int currentMaterialCount = 0;
    private bool canDrawBool = true;



    // Start is called before the first frame update
    void Start()
    {
        currentMaterial = lineMaterials[currentMaterialCount]; //Set Material to Starting Colour - Black
        currentWidth = 0.05f;   //Set starting width
        mainCamera = Camera.main;   //Get Camera
    }

    void Awake()
    {
        //Modified Singleton - Copy Container and destroy other
        if (instance != null && instance != this)
        {
            CopyChildren(instance.GetComponent<DrawController>().lineContainer.transform);
            Destroy(instance.gameObject);
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        gameObject.SetActive(false);
    }

    //Copy Children from this other parent Container
    void CopyChildren(Transform parentTransform)
    {
        Debug.Log("Copying Children");
        //List creation required to avoid iteration and deletion from container in a single loop
        List<Transform> tempList = new List<Transform>();

        //For each child, add them to the list
        foreach (Transform childTransform in parentTransform)
            tempList.Add(childTransform);

        //for each object in the list, storage reference in this script for possible deletion, and change its parent to the new Container
        foreach (Transform trans in tempList)
        {
            drawnLines.Add(trans.gameObject);
            trans.SetParent(lineContainer.transform);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //Draw New Line
            if (canDraw()) //Only Draw if over the allowed space and not currently saving
            {
                drawLine();
            }
        }
        if (Input.GetMouseButton(0)) //While drawing line and mouse is moving
        {
            //Check if touch position has moved
            Vector2 tempTouchPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); //Get the mouse position
            if (lineRenderer != null && Vector2.Distance(tempTouchPosition, touchPositions[touchPositions.Count - 1]) > 0.1f) //if a line is being drawn and mouse has moved more than 0.1f since last check
            {
                updateLine(tempTouchPosition); //Update the line renderer to continue drawing a line
            }
        }

        if (Input.GetMouseButtonUp(0)) //Stop drawing when mouse button is lifted
        {
            lineRenderer = null;
        }
    }

    //Toggle the canDrawBool Variable
    public void toggleCanDrawBool()
    {
        Debug.Log("Toggle Can Draw");
        canDrawBool = !canDrawBool;
    }

    //Check if drawing is allowed
    bool canDraw()
    {
        //Cast a ray to the screen to test for collision with drawing canvas
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit)) //If click on a collider
        {
            Debug.Log(hit.transform.name);
            if (hit.transform.name == "Drawable Area") //If collider is the drawable area
            {
                if (canDrawBool)
                {
                    Debug.Log("Can Draw");
                    return true;
                }
                else
                {
                    Debug.Log("Cannot Draw");
                    return false;
                }

            }
        }
        Debug.Log("Cannot Draw");
        return false;
    }

    //Start drawing a new line
    void drawLine()
    {
        Debug.Log("New Line");
        currentLine = Instantiate(linePrefab, Vector3.zero, Quaternion.identity) as GameObject; //Create a new line from prefab
        drawnLines.Add(currentLine);    //add to list of lines
        currentLine.transform.SetParent(lineContainer.transform);
        lineRenderer = currentLine.GetComponent<LineRenderer>();
        lineRenderer.material = currentMaterial;    //set the colour to use
        lineRenderer.startWidth = currentWidth;     //set the width to use
        lineRenderer.endWidth = currentWidth;       //set the width to use

        //Create a new Line from two touch positions and assign to the lineRenderer
        touchPositions.Clear();
        touchPositions.Add(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        touchPositions.Add(mainCamera.ScreenToWorldPoint(Input.mousePosition));
        lineRenderer.SetPosition(0, touchPositions[0]);
        lineRenderer.SetPosition(1, touchPositions[1]);
    }

    void updateLine(Vector2 newTouchPosition)
    {
        touchPositions.Add(newTouchPosition);   //add a new touch point
        lineRenderer.positionCount++;   //increase number of touch points
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, newTouchPosition); //add touch point to the lineRenderer
    }

    //Remove last drawn line
    public void undoLine()
    {
        if (drawnLines.Count > 0) //Only remove if at least one exists
        {
            Debug.Log("Clear last Lines");
            Destroy(drawnLines[drawnLines.Count - 1]);
            drawnLines.RemoveAt(drawnLines.Count - 1);
        }
    }

    //Remove all drawn lines
    public void deleteAll()
    {
        drawnLines.Clear();
        Debug.Log("Clear all Lines");
    }

    //Set the material to the next in the list
    public void setNextMaterial()
    {
        //Loop round to reset colour
        if (currentMaterialCount == lineMaterials.Count - 1)
        {
            currentMaterialCount = 0;
        }
        else
        {
            currentMaterialCount++;
        }

        currentMaterial = lineMaterials[currentMaterialCount]; //Get the matrial from the list
        colourButton.GetComponent<Image>().material = currentMaterial; //Set the paint can colour
        Debug.Log("Colour Selected: " + currentMaterial);
    }

    //Change to the next width
    public void setNextWidth()
    {
        //Loop from 0.05f to 0.25f in steps of 0.05f then reset
        currentWidth += 0.05f;

        if (currentWidth > 0.25f)
        {
            currentWidth = 0.05f;
        }

        WidthButton.transform.GetChild(0).GetComponent<RectTransform>().localScale = new Vector3(1, currentWidth, 1); //Draw a line to show the current width selected
        Debug.Log("Current Width: " + currentWidth);
    }

}
