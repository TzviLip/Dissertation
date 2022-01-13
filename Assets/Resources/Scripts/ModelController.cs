//Rotation Modified From:
//Title: [Tutorial] How to rotate the camera around an object in Unity3D
//Author: Emma Prat
//Date: 8/6/2020
//Availability: https://emmaprats.com/p/how-to-rotate-the-camera-around-an-object-in-unity3d/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelController : MonoBehaviour
{
    static ModelController instance;

    [SerializeField] GameObject objectContainer;
    [SerializeField] Camera cam;

    private Vector3 previousPosition;

    void Awake()
    {
        //Modified Singleton - Copy Container and destroy other
        if (instance != null && instance != this)
        {
            CopyChildren(instance.GetComponent<ModelController>().objectContainer.transform); //Copy Children to a new container
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
        List<Transform> tempList = new List<Transform>(); //Temporary List to Store Objects

        //For each child, add them to the list
        foreach (Transform childTransform in parentTransform)
            tempList.Add(childTransform);

        //for each object in the list, change its parent to the new Container
        foreach (Transform trans in tempList)
        {
            trans.SetParent(objectContainer.transform);
        }

        Debug.Log("Children Copied");
    }

    //Create an Object From a Prefab
    public void createObject(GameObject toCreate)
    {
        var point = Camera.main.ViewportToWorldPoint(new Vector3(0.6f, 0.5f, 10.0f)); //The point to create the object based on camera position and rotation
        GameObject newObject = Instantiate(toCreate, point, cam.transform.rotation) as GameObject; //Create the required object
        newObject.transform.SetParent(objectContainer.transform); //Set the objects parent to the container for storage
        Debug.Log("Object Created: " + newObject);
    }



    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            previousPosition = cam.ScreenToViewportPoint(Input.mousePosition); //When pressing the right mouse button, get the position of the click
        }
        else if (Input.GetMouseButton(1))
        {
            Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition); //As mouse is moved, get the new position
            Vector3 direction = previousPosition - newPosition; //calculate the direction from the old click

            float rotationX = -direction.x * 180; //Get the X rotation using direction moved
            float rotationY = direction.y * 180; //Get the Y rotation using direction moved

            cam.transform.position = new Vector3 (0, 0, 0); //Set the camera position to origin

            cam.transform.Rotate(new Vector3(1.0f, 0, 0), rotationY); //Rotate the camera according to the Y rotation
            cam.transform.Rotate(new Vector3(0, 1.0f, 0), rotationX, Space.World); //Rotate the camera according to the X rotation

            cam.transform.Translate(new Vector3(0, 0, -10.0f)); //Move the camera to its initial point

            previousPosition = newPosition; //Reset the old position for next frame
        }
    }
}
