using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

//UI Manager handles all behavior for UI on the screen. However, these functions are called in
//GameManager
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject canvas;
    //task popup prefabs
    public GameObject popupContainer;
    public GameObject taskButton;
    public Sprite tailSprite;
    public List<TaskSO> currentTasks = null;
    public GameManagerScript gm;

    //dictionary for keeping track of everything on the screen
    Dictionary<TaskSO, GameObject> taskToButtonDict = new Dictionary<TaskSO, GameObject>();
    Dictionary<GameObject, GameObject> boxToTailDict = new Dictionary<GameObject, GameObject>();

    //called one time on mouse down
    //creates, positions, and adds buttons with corresponding tasks
    public void generateTaskPopup(List<TaskSO> tasksToDisplay, Vector3 position) {
        if (tasksToDisplay.Count == 0) {
            
            return;
        }
        foreach (TaskSO task in tasksToDisplay) {
            if (taskToButtonDict.ContainsKey(task)) {
                return;
            }
        }

        currentTasks = tasksToDisplay;
        Vector3 pointToSpawn = findValidPositionForPopup(position);
        GameObject _tail = drawTail(position, pointToSpawn, canvas.transform);
        GameObject _popupContainer = Instantiate(popupContainer);

        boxToTailDict.Add(_popupContainer, _tail);

        _popupContainer.transform.SetParent(canvas.transform, false);
        
        _popupContainer.transform.position = pointToSpawn;

        
        List<GameObject> renderedButtons = new List<GameObject>();
        foreach (TaskSO task in tasksToDisplay) {

            GameObject newButton = createButton(task, _popupContainer.transform);
            renderedButtons.Add(newButton);

        }
        Canvas.ForceUpdateCanvases();
        int i = 0;
        Debug.Log("length: " + renderedButtons.Count);
        foreach (GameObject button in renderedButtons) {
            Vector3 originalPos = button.GetComponent<RectTransform>().position;
            //button.transform.position = originalPos + (Vector3.right * 300f);
            Sequence s = DOTween.Sequence();
            
            //s.Append(button.GetComponent<Image>().material.DOFade(1f, 1f));
            s.Append(button.transform.DOShakeRotation(1f, 10f).SetLoops(int.MaxValue));
            
            i++;
        }
    }


    //returns ScreenPoint
    public Vector3 findValidPositionForPopup(Vector2 _mouseDownPos) {
        bool foundValidPoint = false;
        Vector3 tryPoint = Vector3.zero;
                
        float screenHeight = Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        while (!foundValidPoint)
        {
            Vector3 mouseWP = Camera.main.ScreenToWorldPoint(_mouseDownPos);
            tryPoint = new Vector2(mouseWP.x, mouseWP.y) + Random.insideUnitCircle.normalized*2f;
            if (tryPoint.x > -screenWidth && tryPoint.x < screenWidth) {
                if (tryPoint.y > -screenHeight && tryPoint.y < screenHeight)
                {
                    foundValidPoint = true;
                }
            }
            

        }

        return Camera.main.WorldToScreenPoint(tryPoint);
        
        
    }

    //takes 2 screen space args
    public GameObject drawTail(Vector3 startPos, Vector3 endPos, Transform _parent) {
        GameObject tail = new GameObject();

        tail.transform.SetParent(_parent, false);

        tail.transform.position = endPos;
        
        Vector3 vecToTarg = Vector3.Scale((endPos - startPos), new Vector3(1f, 1f, 0f));
        tail.AddComponent<LayoutElement>().ignoreLayout = true;
        tail.transform.rotation = Quaternion.LookRotation(Vector3.forward, vecToTarg);
        Image i = tail.AddComponent<Image>();
        i.sprite = tailSprite;
        RectTransform rt = tail.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 1f);
        rt.localScale = new Vector3(0.5f, 0f, 1f);
        rt.DOScaleY(vecToTarg.magnitude / rt.rect.height, 0.5f);
        return tail;
        //rt.localScale = new Vector3(0.1f, vecToTarg.magnitude / rt.rect.height, 1f);
    }

    

    public GameObject createButton(TaskSO task, Transform parent) {
        
        GameObject _newButton = Instantiate(taskButton);
        //_newButton.transform.position = Vector3.zero;
        _newButton.transform.SetParent(parent, true);
        _newButton.GetComponentInChildren<Text>().text = task.title;
        if (!taskToButtonDict.ContainsKey(task)) {
            taskToButtonDict.Add(task, _newButton);
            //when you are creating a new button
            if (task.complete)
            {
                taskToButtonDict[task].GetComponent<Image>().color = Color.green;
                if (task.children.Count != 0) {
                    if (!allChildrenComplete(task)) {
                        taskToButtonDict[task].GetComponent<Image>().color = Color.yellow;
                    }
                }
            }
        }
        
        

        //_newButton.GetComponent<RectTransform>().DOAnchorPos(originalPos, 1f);
        _newButton.GetComponent<Button>().onClick.AddListener(delegate { setNewHead(task); });

        return _newButton;
    }

    public bool allChildrenComplete(TaskSO task) {
        if (task.children.Count == 0)
        {
            return task.complete;
        }
        bool allComplete = true;
        foreach (TaskSO t in task.children)
        {
            if (!allChildrenComplete(t))
            {
                allComplete = false;
            }

        }

        return allComplete;
        
        
    }

    public void updateCompleteness(TaskSO head) {
        if (head == null) {
            Debug.Log("head is null");
            return;
        }

        //if all of the children are completed
        if (allChildrenComplete(head)) {

            //recurses up the parents, and sets them to green if they are all compelte
            if (taskToButtonDict.ContainsKey(head))
            {
                taskToButtonDict[head].GetComponent<Image>().color = Color.green;
                
            }

            else
            {
                Debug.Log("isn't in dict: " + head.title);
            }
            if (head.children.Count != 0)
            {
                
                //check siblings
                GameObject wrapperToDelete = null;
                foreach (TaskSO child in head.children)
                {
                    if (!taskToButtonDict.ContainsKey(child)) {
                        continue;
                    }
                    if (wrapperToDelete != taskToButtonDict[child].transform.parent.gameObject)
                    {
                        wrapperToDelete = taskToButtonDict[child].transform.parent.gameObject;
                    }
                    taskToButtonDict.Remove(child);
                }
                if (wrapperToDelete == null) {
                    return;
                }

                //removeal
                Sequence s = DOTween.Sequence();
                s.Append(boxToTailDict[wrapperToDelete].transform.DOScaleY(0f, 0.5f));
                s.Append(wrapperToDelete.transform.DOMoveY(5f, 0.5f));
                boxToTailDict.Remove(wrapperToDelete);
                Debug.Log(head.title + " is destroying children");
                Destroy(wrapperToDelete, s.Duration());
            }
            
            
            updateCompleteness(head.parent);
        }
    }
    public void setNewHead(TaskSO t) {
        //need to get siblings of task
        if (allChildrenComplete(t)) {
            Debug.Log("All paths exhuasted on this node");
            return;
        }
        t.complete = true;
        //this will only be entered if all of the children are not complete
        if (taskToButtonDict.ContainsKey(t))
        {
            
            taskToButtonDict[t].GetComponent<Image>().color = Color.yellow;

        }
        updateCompleteness(t);
        generateTaskPopup(t.children, Input.mousePosition);


    }

    public void clearAllPopups(TaskSO head) {
        //inefficient, iterate over entire dict

        foreach (TaskSO key in taskToButtonDict.Keys) {
            GameObject box = taskToButtonDict[key].transform.parent.gameObject;
            //if the box is still there
            if (boxToTailDict.ContainsKey(box)) {
                //remove box and tail
                GameObject tail = boxToTailDict[box];

                Sequence s = DOTween.Sequence();
                s.Append(boxToTailDict[box].transform.DOScaleY(0f, 0.5f));
                s.Append(box.transform.DOMoveY(5f, 0.5f));
                boxToTailDict.Remove(box);
                Debug.Log(head.title + " is destroying children");
                Destroy(box, s.Duration());

            }

            
        }

        taskToButtonDict.Clear();

    }
}
