using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teacher
{
    //public Fields Required for JSON serialization
    public string name;
    public string prompt;

    //Create Default Teacher
    public Teacher(string name)
    {
        this.name = name;
        this.prompt = "";
    }

    //Create Specific Teacher
    public Teacher(string name, string prompt)
    {
        this.name = name;
        this.prompt = prompt;
    }
}
