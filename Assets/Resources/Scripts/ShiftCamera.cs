using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCamera : MonoBehaviour
{
    //Move the Camera Left 50 units
    public void shiftCameraLeft()
    {
        Camera.main.transform.position -= new Vector3(50,0,0);
    }

    //Move the Camera Right 50 units
    public void shiftCameraRight()
    {
        Camera.main.transform.position += new Vector3(50, 0, 0);
    }

    //Move the Camera Down 50 units
    public void shiftCameraDown()
    {
        Camera.main.transform.position -= new Vector3(0, 50, 0);
    }

    //Move the Camera Up 50 units
    public void shiftCameraUp()
    {
        Camera.main.transform.position += new Vector3(0, 50, 0);
    }
}
