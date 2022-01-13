using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectController : MonoBehaviour
{
    private Vector3 screenPoint;
    private Vector3 offset;
    [SerializeField] private MenuController menuController;
    float rotSpeed = 200;

    //When this object is added to the scene
    private void OnEnable()
    {
        menuController = GameObject.FindGameObjectWithTag("MenuController").GetComponent<MenuController>(); //Find the Menu Controller
    }

    //When the left mouse button is pressed
    void OnMouseDown()
    {
        screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position); //Get the position pressed and convert to a screen point

        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z)); //Calculate the difference between current and clicked position

        if (menuController.getMode() == 2)
        {
            GetComponent<MeshRenderer>().material = menuController.getSelectedMaterial(); //If Mode is set to Colour then set the Material of the object
            Debug.Log("Colour Set: " + menuController.getSelectedMaterial());
        }

        if (menuController.getMode() == 3)
        {
            transform.localScale += new Vector3(0.1f, 0.1f, 0.1f) * menuController.getResizeMode(); //If mode is set to Resize then enlarge/reduce
            Debug.Log("Resize");
        }

        if (menuController.getMode() == 5)
        {
            Debug.Log("Object Deleted: " + gameObject.name);
            Destroy(gameObject); //If mode is set to Delete then destroy
        }
    }

    void OnMouseDrag()
    {
        if (menuController.getMode() == 1) //If mode is set to Move
        {
            Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z); //Get the cursor point on the screen
            Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset; //calculate the new position using the offset of the mouse to account for not clicking on the centre of an object
            transform.position = cursorPosition; //Move the object
        }

        if (menuController.getMode() == 4) //If mode is set to Rotate
        {
            float rotX = Input.GetAxis("Mouse X") * rotSpeed * Mathf.Deg2Rad; //get mouse rotation in the x-Axis and convert to radians
            float rotY = Input.GetAxis("Mouse Y") * rotSpeed * Mathf.Deg2Rad; //get mouse rotation in the y-Axis and convert to radians

            transform.Rotate(Camera.main.transform.up, -rotX, Space.World); //rotate in the x-Axis (World Space)
            transform.Rotate(Camera.main.transform.right, rotY, Space.World); //rotate in the y-Axis (World Space)
        }
    }
}
