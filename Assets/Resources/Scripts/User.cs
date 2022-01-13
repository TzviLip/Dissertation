using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class User
{
    //public Fields Required for JSON serialization
    public string name;
    public string imageURLDraw;
    public string imageURLModel;
    public string imageURLAvatar;
    public int gold;
    public bool completed;
    public string teacher;
    public string evaluation;

    //Create Default User
    public User(string name, string teacher)
    {
        this.name = name;
        this.imageURLDraw = "";
        this.imageURLModel = "";
        this.imageURLAvatar = "";
        this.gold = 100;
        this.completed = false;
        this.teacher = teacher;
        this.evaluation = "";
    }

    //Create Specific User
    public User(string name, string imageURLDraw, string imageURLModel, string imageURLAvatar, int gold, bool completed, string teacher, string evaluation)
    {
        this.name = name;
        this.imageURLDraw = imageURLDraw;
        this.imageURLModel = imageURLModel;
        this.imageURLAvatar = imageURLAvatar;
        this.gold = gold;
        this.completed = completed;
        this.teacher = teacher;
        this.evaluation = evaluation;
    }


}
