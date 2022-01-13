using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadingManager : MonoBehaviour
{
    //Load a Scene by its integer in the build manager
    public void LoadSceneByInt(int sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("Scene Loaded: " + sceneToLoad);
    }

    //Loadd a Scene by its name
    public void LoadSceneByString(string sceneToLoad)
    {
        SceneManager.LoadScene(sceneToLoad);
        Debug.Log("Scene Loaded: " + sceneToLoad);
    }
}
