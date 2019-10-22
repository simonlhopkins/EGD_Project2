using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Task : MonoBehaviour
{
    // Start is called before the first frame update

    public TaskSO head;
    public HashSet<TaskSO> nodes = new HashSet<TaskSO>();
    public string filePath;


    private void Start()
    {

        

        head = new GameObject().AddComponent<TextToTreeParser>().generateTree(filePath);

        //foreach(TaskSO task in nodes)
        //{
        //    task.complete = false;
        //}
        

    }

    private void addNodes(TaskSO parent, List<TaskSO> children) {
        nodes.Add(parent);
        foreach (TaskSO child in children) {
            parent.children.Add(child);
            child.parent = parent;
            nodes.Add(child);
        }
    }

    private void Update()
    {

        

        

    }

    


}
