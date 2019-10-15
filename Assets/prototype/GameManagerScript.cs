﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    private UIManager uiManager;
    public TaskSO head;

    void Start()
    {
        uiManager = GetComponent<UIManager>();
        head = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            uiManager.findValidPositionForPopup(Input.mousePosition);
            if (hit.collider != null)
            {
                Debug.Log("generate");
                head = hit.collider.gameObject.GetComponent<Task>().head;
                uiManager.setNewHead(head);
            }
        }
    }

    private bool hasUncompletedTasksInChildren(TaskSO _head) {

        if (_head.children.Count == 0) {
            return false;
        }
        foreach (TaskSO child in head.children)
        {
            if (!child.complete)
            {
                return true;
            }
            hasUncompletedTasksInChildren(child);
        }
        return false;
        
    }
    private void bfs(TaskSO _head) {
        Queue<TaskSO> q = new Queue<TaskSO>();
        HashSet<TaskSO> visitedNodes = new HashSet<TaskSO>();
        visitedNodes.Add(_head);
        q.Enqueue(_head);
        while (q.Count != 0) {
            TaskSO v = q.Dequeue();
            //v is the current vertex and we are going to add all of
            //its children to the queue
            foreach (TaskSO child in v.children) {
                if (!visitedNodes.Contains(child)) {
                    q.Enqueue(child);
                    visitedNodes.Add(child);

                }
            }
        }
    }

    public void displayTasks() {
        
    }
}
