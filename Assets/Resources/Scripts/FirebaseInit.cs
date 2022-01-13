using System.Collections;
using System.Collections.Generic;
using Firebase;
using UnityEngine;
using UnityEngine.Events;

public class FirebaseInit : MonoBehaviour
{
    public UnityEvent OnFirebaseInitialized;
    static FirebaseInit instance;

    async void Start()
    {
        //Fix Firebase Dependencies on First Load. Error should only occur on misconfiguration in the editor
        var dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();

        if (dependencyStatus == DependencyStatus.Available)
        {
            Debug.Log("Firebase: " + dependencyStatus);
            OnFirebaseInitialized.Invoke(); //Invoke Event to Let game know it is okay to proceed
        }
        else
        {
            Debug.LogError(System.String.Format("Could not resolve all Firebase dependencies: {0}", dependencyStatus));
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
}
