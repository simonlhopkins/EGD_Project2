using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class GameManagerScript : MonoBehaviour
{
    // Start is called before the first frame update

    private UIManager uiManager;
    public TaskSO head;
    public bool UIactive = false;

    void Start()
    {
        uiManager = GetComponent<UIManager>();
        head = null;
        //uiManager.scrollRect.SetActive(false);
        //uiManager.scrollRect.transform.position = -Vector3.up * 500f;
        uiManager.scrollRect.GetComponent<RectTransform>().DOMoveY(-100f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!UIactive)
            {
                uiManager.scrollRect.GetComponent<RectTransform>().DOMoveY(100f, 1f);
            }
            else {
                uiManager.scrollRect.GetComponent<RectTransform>().DOMoveY(-100f, 1f);
            }
            UIactive = !UIactive;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (uiManager.showingSentance) {
                return;
            }


            if (EventSystem.current.IsPointerOverGameObject()) {
                return;
            }
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), -Vector2.up, Mathf.Infinity, 1 << LayerMask.NameToLayer("clickable"));
            if (hit.collider != null)
            {
                
                uiManager.deleteSubTree(head, 0f);
                head = hit.collider.gameObject.GetComponent<Task>().head;
                uiManager.setNewHead(head);
                //uiManager.generateAutoTree(head);
            }
            else {
                uiManager.deleteSubTree(head, 0f);
            }
        }


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
