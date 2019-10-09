using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            if (hit.collider != null)
            {
                Debug.Log(hasUncompletedTasksInChildren(hit.collider.gameObject.GetComponent<Task>().head));
            }
        }
    }

    private bool hasUncompletedTasksInChildren(TaskSO head) {

        if (head.children.Count == 0) {
            return false;
        }
        foreach (TaskSO child in head.children)
        {
            if (!child.complete)
            {
                Debug.Log(child.title + " is incomplete");
                return true;
            }
            hasUncompletedTasksInChildren(child);
        }
        return false;
        
    }
    private void bfs(TaskSO head) {
        Queue<TaskSO> q = new Queue<TaskSO>();
        HashSet<TaskSO> visitedNodes = new HashSet<TaskSO>();
        visitedNodes.Add(head);
        q.Enqueue(head);
        while (q.Count != 0) {
            TaskSO v = q.Dequeue();
            //v is the current vertex and we are going to add all of
            //its children to the queue
            foreach (TaskSO child in v.children) {
                if (!visitedNodes.Contains(child)) {
                    q.Enqueue(child);
                    visitedNodes.Add(child);
                    Debug.Log(child.title);
                }
            }
        }
    }

    public void displayTasks() {
        
    }
}
