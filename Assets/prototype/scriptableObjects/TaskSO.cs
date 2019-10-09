
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Task Scriptable Object", order = 1)]
public class TaskSO : ScriptableObject
{

    public string flavorName;
    public Sprite sprite;
    public TaskSO parent;
    public List<TaskSO> children;

    public bool hasBeenCompleted = false;

    

}
