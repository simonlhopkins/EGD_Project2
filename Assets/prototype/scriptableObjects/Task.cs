﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Task : MonoBehaviour
{
    // Start is called before the first frame update

    public TaskSO head;
    public HashSet<TaskSO> nodes = new HashSet<TaskSO>(); 

    public void renderScriptableObject() {
        if (!gameObject.GetComponent<SpriteRenderer>()) {
            gameObject.AddComponent<SpriteRenderer>().sprite = head.icon;
        }
        if (!gameObject.GetComponent<Collider2D>()) {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }


    private void Start()
    {
        renderScriptableObject();

        TaskSO testHead = new TaskSO();
        TaskSO testc1 = new TaskSO();
        testc1.title = "c1";
        TaskSO testc2 = new TaskSO();
        testc2.title = "c2";
        TaskSO testc3 = new TaskSO();
        testc3.title = "c3";
        TaskSO testc4 = new TaskSO();
        testc4.title = "c4";
        TaskSO testc5 = new TaskSO();
        testc5.title = "c5";
        TaskSO testc6 = new TaskSO();
        testc6.title = "c6";

        addNodes(testHead, new List<TaskSO>() { testc1, testc2 });
        addNodes(testc1, new List<TaskSO>() { testc3, testc4 });
        addNodes(testc3, new List<TaskSO>() { testc5, testc6 });


        head = testHead;
        foreach(TaskSO task in nodes)
        {
            task.complete = true;
        }

        testc5.complete = false;
        testc6.complete = false;

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
