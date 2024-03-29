﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Task Scriptable Object", order = 1)]
public class TaskSO : ScriptableObject
{

    public string title;
    public Sprite icon;
    public TaskSO parent;
    public string achievementText;
    public List<TaskSO> children = new List<TaskSO>();
    public bool complete = false;
    public bool hasBeenVisited = false;
    public float timeToAppear = 0f;
    public string[] tags = new string[0];

}


[CreateAssetMenu(menuName = "ScriptableObjects/Task Scriptable Object", order = 1)]
public class HeadTaskSO : TaskSO
{
    
}
