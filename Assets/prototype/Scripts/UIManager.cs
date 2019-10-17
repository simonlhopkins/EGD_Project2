using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

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

        GameObject _popupContainer = Instantiate(popupContainer);



        _popupContainer.transform.SetParent(canvas.transform, false);





        List<GameObject> renderedButtons = new List<GameObject>();
        foreach (TaskSO task in tasksToDisplay) {

            GameObject newButton = createButton(task, _popupContainer.transform);
            //setButtonAlpha(newButton, 1f);
            Debug.Log("setting alpha of..." + newButton.name);
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

        Vector3 pointToSpawn = findValidPositionForPopup(position, _popupContainer);
        //GameObject _tail = drawTail(position, pointToSpawn, canvas.transform);
        //boxToTailDict.Add(_popupContainer, _tail);
        _popupContainer.transform.position = pointToSpawn + Random.insideUnitSphere.normalized * 200f;
        _popupContainer.transform.DOMove(pointToSpawn, 1f);

    }

    public void setButtonAlpha(GameObject button, float alpha) {
        button.GetComponent<Image>().DOFade(alpha, 0f);
        button.GetComponentInChildren<TMP_Text>().DOFade(alpha, 0f);
    }

    //returns ScreenPoint
    public Vector3 findValidPositionForPopup(Vector2 _mouseDownPos, GameObject _popup) {
        bool foundValidPoint = false;
        Vector3 tryPoint = Vector3.zero;

        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;
        float scaler = Screen.width / canvasWidth;
        Rect popupRect = _popup.GetComponent<RectTransform>().rect;
        tryPoint = new Vector2(_mouseDownPos.x, _mouseDownPos.y) + Random.insideUnitCircle.normalized * 10f;
        while (!foundValidPoint)
        {
            //try point relative to the screen
            tryPoint = new Vector2(_mouseDownPos.x, _mouseDownPos.y) + Random.insideUnitCircle.normalized * 200 * scaler;
            Debug.Log("mouse pos" + _mouseDownPos);
            Debug.Log("x bounds: " + (Screen.width - popupRect.width * scaler / 2f) + " -> " + (popupRect.width * scaler) / 2f);
            Debug.Log("y bounds: " + (Screen.height - popupRect.height * scaler / 2f) + " -> " + popupRect.height * scaler / 2f);
            Debug.Log("trypoint: " + tryPoint);
            if (tryPoint.x < (Screen.width - popupRect.width * scaler / 2f) && tryPoint.x > (popupRect.width * scaler) / 2f)
            {
                if (tryPoint.y < (Screen.height - popupRect.height * scaler / 2f) && tryPoint.y > popupRect.height * scaler / 2f)
                {
                    foundValidPoint = true;
                }
            }


        }

        return tryPoint;


    }

    //takes 2 screen space args
    public GameObject drawTail(Vector3 startPos, Vector3 endPos, Transform _parent) {
        GameObject tail = new GameObject();

        tail.transform.SetParent(_parent, false);

        tail.transform.position = endPos;

        Vector3 vecToTarg = Vector3.Scale(endPos - startPos, new Vector3(1f, 1f, 0f));
        tail.AddComponent<LayoutElement>().ignoreLayout = true;
        tail.transform.rotation = Quaternion.LookRotation(Vector3.forward, vecToTarg);
        Image i = tail.AddComponent<Image>();
        i.sprite = tailSprite;
        i.color = Color.white;
        RectTransform rt = tail.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 1f);
        rt.localScale = new Vector3(0.5f, vecToTarg.magnitude / rt.rect.height, 1f);
        //rt.DOScaleY(vecToTarg.magnitude / rt.rect.height, 0.5f);
        //Debug.Log(startPos + "<start pos, end pos>" + endPos);
        //Debug.Log(Input.mousePosition);
        //Debug.Log(rt.rect.height);
        //Debug.Log(vecToTarg.magnitude);
        return tail;
        //rt.localScale = new Vector3(0.1f, vecToTarg.magnitude / rt.rect.height, 1f);
    }



    public GameObject createButton(TaskSO task, Transform parent) {

        GameObject _newButton = Instantiate(taskButton);
        //_newButton.transform.position = Vector3.zero;
        _newButton.transform.SetParent(parent, true);
        IEnumerator co = showText(task);
        StartCoroutine(co);
        if (task.timeToAppear <= 0) {
            _newButton.GetComponentInChildren<TMP_Text>().text = task.title;
        }

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

        _newButton.GetComponent<RectTransform>().localScale = Vector3.one;

        //_newButton.GetComponent<RectTransform>().DOAnchorPos(originalPos, 1f);
        _newButton.GetComponent<Button>().onClick.AddListener(delegate { setNewHead(task); });

        return _newButton;
    }

    public IEnumerator showText(TaskSO task) {
        if (task.hasBeenVisited) {
            yield return 0f;
        }
        task.hasBeenVisited = true;
        while (task.timeToAppear > 0) {
            task.timeToAppear -= Time.deltaTime;
            yield return 0;
        }
        if (taskToButtonDict.ContainsKey(task)) {
            taskToButtonDict[task].GetComponentInChildren<TMP_Text>().text = task.title;
        }


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

        if (head.parent != null && head.children.Count!=0) {
            foreach (TaskSO t in head.parent.children)
            {
                if (t == head)
                {
                    continue;
                }
                
                if (taskToButtonDict.ContainsKey(t))
                {
                    foreach (TaskSO childOfSibling in t.children) {
                        deleteSubTree(childOfSibling);
                    }
                }

            }

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

                Sequence s = DOTween.Sequence();
                //s.Append(boxToTailDict[wrapperToDelete].transform.DOScaleY(0f, 0.5f));
                s.Append(wrapperToDelete.transform.DOMoveY(5f, 0.5f));
                //GameObject tailToDelete = boxToTailDict[wrapperToDelete];

                //boxToTailDict.Remove(wrapperToDelete);
                //Destroy(tailToDelete);
                Destroy(wrapperToDelete, s.Duration());
            }


            updateCompleteness(head.parent);
        }
    }

    public void deleteIfNotInPath(TaskSO head, List<TaskSO> pathToHead) {

        pathToHead.Add(head);
        foreach (TaskSO t in head.children) {

        }
        

        if (head.parent == null) {

        }


    }

    public void deleteSubTree(TaskSO _head) {
        //if (_head == null) {
        //    return;
        //}
        if (_head == null) {
            Debug.Log("return");
            return;
        }
        foreach (TaskSO t in _head.children)
        {

            deleteSubTree(t);
        }
        if (!taskToButtonDict.ContainsKey(_head))
        {
            return;
        }
        
            
            
        

        GameObject boxToDestroy = null;
        if (taskToButtonDict[_head] != null)
        {
            boxToDestroy = taskToButtonDict[_head].transform.parent.gameObject;
        }
        taskToButtonDict.Remove(_head);
        


        //s.Append(box.transform.DOMoveY(5f, 0.5f));
        if (boxToDestroy != null) {
            Sequence s = DOTween.Sequence();
            s.Append(boxToDestroy.transform.DOMoveY(5f, 0.5f));
            Destroy(boxToDestroy, s.Duration());
        }
        
        
    }

    public void setNewHead(TaskSO t) {
        //need to get siblings of task
        if (t.timeToAppear > 0) {
            Debug.Log(t.timeToAppear);
            return;
        }
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

    
}
