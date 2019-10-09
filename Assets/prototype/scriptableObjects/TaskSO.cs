
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

}


[CreateAssetMenu(menuName = "ScriptableObjects/Task Scriptable Object", order = 1)]
public class HeadTaskSO : TaskSO
{
    
}
