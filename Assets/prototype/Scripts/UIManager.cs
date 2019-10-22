using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using System;
//UI Manager handles all behavior for UI on the screen. However, these functions are called in
//GameManager
public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject canvas;
    //task popup prefabs
    public GameObject popupContainer;
    public GameObject taskButton;
    public GameObject achievementContainerPanel;
    public GameObject achievementPanelPrefab;
    public GameObject achievementAnimImagePrefab;
    public Sprite tailSprite;
    public List<TaskSO> currentTasks = null;
    public GameManagerScript gm;
    public float disapearDelay = 1f;

    public bool showingSentance = false;

    //dictionary for keeping track of everything on the screen
    Dictionary<TaskSO, GameObject> taskToButtonDict = new Dictionary<TaskSO, GameObject>();
    Dictionary<GameObject, GameObject> boxToTailDict = new Dictionary<GameObject, GameObject>();

    
    

    private void OnApplicationQuit()
    {
        StopAllCoroutines();
    }
    //called one time on mouse down
    //creates, positions, and adds buttons with corresponding tasks


    public Sequence generateTaskPopup(List<TaskSO> tasksToDisplay, Vector3 position) {

        if (tasksToDisplay.Count == 0) {

            return DOTween.Sequence();
        }

        //this shouldn't ever be entered, but checks to make sure there isn't already
        //a duplicate task on screen
        foreach (TaskSO task in tasksToDisplay) {
            if (taskToButtonDict.ContainsKey(task)) {
                return DOTween.Sequence();
            }
        }

        currentTasks = tasksToDisplay;

        GameObject _popupContainer = Instantiate(popupContainer);



        _popupContainer.transform.SetParent(canvas.transform, false);

        List<GameObject> renderedButtons = new List<GameObject>();
        foreach (TaskSO task in tasksToDisplay) {

            GameObject newButton = createButton(task, _popupContainer.transform);
            renderedButtons.Add(newButton);

        }

        //adding all of the buttons and then doing one force update is more efficient than
        //doing it after every frame.
        Canvas.ForceUpdateCanvases();
        foreach (GameObject button in renderedButtons) {
            Vector3 originalPos = button.GetComponent<RectTransform>().position;
            Sequence s = DOTween.Sequence();
            s.Append(button.transform.DOShakeRotation(1f, 10f));
        }

        Vector3 pointToSpawn = findValidPositionForPopup(position, _popupContainer);


        _popupContainer.transform.position = Input.mousePosition;
        _popupContainer.transform.localScale = Vector3.zero;
        Sequence s1 = DOTween.Sequence();
        s1.Append(_popupContainer.transform.DOMove(pointToSpawn, 0.5f));
        s1.Insert(0, _popupContainer.transform.DOScale(Vector3.one, 0.5f));

        return s1;
    }

    public void setButtonAlpha(GameObject button, float alpha) {
        button.GetComponent<Image>().DOFade(alpha, 0f);
        button.GetComponentInChildren<TMP_Text>().DOFade(alpha, 0f);
    }

    //returns ScreenPoint
    //finds valid point within bounds this was such a pain god
    //like wtf there are literally like 3 different coordinate systems like
    //the screen coordinates and canvas coordinates and world coordinates like fuck that noise for real
    public Vector3 findValidPositionForPopup(Vector2 _mouseDownPos, GameObject _popup) {
        bool foundValidPoint = false;

        float canvasHeight = canvas.GetComponent<RectTransform>().rect.height;
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width- achievementContainerPanel.GetComponent<RectTransform>().rect.width;
        Debug.Log("width of container: " +achievementContainerPanel.GetComponent<RectTransform>().rect.width);
        float scaler = Screen.width / canvasWidth;
        Rect popupRect = _popup.GetComponent<RectTransform>().rect;
        Vector3 tryPoint = new Vector2(_mouseDownPos.x, _mouseDownPos.y) + UnityEngine.Random.insideUnitCircle.normalized * 10f;
        while (!foundValidPoint)
        {
            //try point relative to the screen
            tryPoint = new Vector2(_mouseDownPos.x, _mouseDownPos.y) + UnityEngine.Random.insideUnitCircle.normalized * 200 * scaler;

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
    public Color colorFromRGB(float r, float g, float b) {
        return new Color(r / 255f, g / 255f, b / 255f, 1f);
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
        i.color = colorFromRGB(184, 186, 200);
        RectTransform rt = tail.GetComponent<RectTransform>();
        rt.pivot = new Vector2(0.5f, 1f);
        rt.localScale = new Vector3(0.5f, vecToTarg.magnitude / rt.rect.height, 1f);
        //rt.DOScaleY(vecToTarg.magnitude / rt.rect.height, 0.5f);

        return tail;
        //rt.localScale = new Vector3(0.1f, vecToTarg.magnitude / rt.rect.height, 1f);
    }



    public GameObject createButton(TaskSO task, Transform parent) {

        GameObject _newButton = Instantiate(taskButton);

        _newButton.transform.SetParent(parent, true);
        IEnumerator co = showText(task);
        StartCoroutine(co);
        
        _newButton.transform.Find("taskImage").gameObject.GetComponent<Image>().sprite = task.icon;

        
        if (task.timeToAppear <= 0) {
            _newButton.GetComponentInChildren<TMP_Text>().text = task.title;
        }

        if (!taskToButtonDict.ContainsKey(task)) {
            taskToButtonDict.Add(task, _newButton);
            //when you are creating a new button
            if (task.complete)
            {
                taskToButtonDict[task].GetComponent<Image>().color = colorFromRGB(180, 214, 211);
                if (task.children.Count != 0) {
                    if (!allChildrenComplete(task)) {
                        taskToButtonDict[task].GetComponent<Image>().color = colorFromRGB(170, 120, 166);
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

    public void updateCompleteness(TaskSO head, float delay) {
        if (head == null) {
            return;
        }

        //special case for clicking off of a tree to another partially complete node.
        if (head.parent != null && head.children.Count != 0) {
            foreach (TaskSO t in head.parent.children)
            {
                if (t == head)
                {
                    continue;
                }

                if (taskToButtonDict.ContainsKey(t))
                {
                    foreach (TaskSO childOfSibling in t.children) {
                        deleteSubTree(childOfSibling, 0f);
                    }
                }

            }

        }


        //if all of the children are completed in the node you just selected, check to
        //see if the parent is now compelte
        if (allChildrenComplete(head)) {

            //recurses up the parents, and sets them to green if they are all compelte
            if (taskToButtonDict.ContainsKey(head))
            {
                taskToButtonDict[head].GetComponent<Image>().color = colorFromRGB(180, 214, 211);

            }



            if (head.children.Count != 0)
            {

                //if it is complete, that means that all of the children are complete
                //so we can delete the children
                foreach (TaskSO child in head.children)
                {
                    //
                    deleteNode(child, delay);
                }



            }


            updateCompleteness(head.parent, delay + disapearDelay);
        }
    }


    float testDelay = 0f;
    public void deleteSubTree(TaskSO t, float delay)
    {
        if (t == null)
        {
            return;
        }

        
        foreach (TaskSO child in t.children)
        {
            if (taskToButtonDict.ContainsKey(child)) {
                deleteSubTree(child, delay);
            }
            

        }
        
        deleteNode(t, testDelay);

    }

    public void deleteNode(TaskSO node, float delay) {
        //if (_head == null) {
        //    return;
        //}
        if (node == null) {
            return;
        }

        if (!taskToButtonDict.ContainsKey(node))
        {
            return;
        }

        GameObject boxToDestroy = null;
        if (taskToButtonDict[node] != null)
        {
            boxToDestroy = taskToButtonDict[node].transform.parent.gameObject;
        }

        Vector3 targetPos;

        if (!taskToButtonDict.ContainsKey(node.parent))
        {
            targetPos = taskToButtonDict[node].transform.position - transform.up * 100f;
        }
        else {
            targetPos = taskToButtonDict[node.parent].transform.position;
        }
        
        taskToButtonDict.Remove(node);



        //s.Append(box.transform.DOMoveY(5f, 0.5f));
        if (boxToDestroy != null) {
            Sequence s = DOTween.Sequence();
            s.Append(boxToDestroy.transform.DOMove(targetPos, disapearDelay).SetDelay(delay));
            s.Insert(0f, boxToDestroy.transform.DOScale(Vector3.zero, disapearDelay).SetDelay(delay));
            Destroy(boxToDestroy, s.Duration());
        }


    }


    //this function sets the new head to act on, and updates the graph based on this head being completed!
    public void setNewHead(TaskSO t) {

        if (t.timeToAppear > 0) {
            return;
        }
        if (Array.IndexOf(t.tags, "end") > -1)
        {
            showingSentance = false;

        }

        if (!t.complete && t.achievementText!= null && t.achievementText!= "") {

            //generates new achievement button
            GameObject newAchievement = Instantiate(achievementPanelPrefab);
            newAchievement.transform.SetParent(achievementContainerPanel.transform);
            newAchievement.GetComponentInChildren<TMP_Text>().text = t.achievementText;
            newAchievement.GetComponent<Image>().color = colorFromRGB(178, 255, 214);
            newAchievement.transform.localScale = Vector3.one;
            Canvas.ForceUpdateCanvases();
            newAchievement.transform.DOShakeScale(1f, 0.5f);
            Destroy(newAchievement, 5f);


        }

        if (allChildrenComplete(t))
        {
            Debug.Log("All paths exhuasted on this node");
            return;
        }


        t.complete = true;
        if (taskToButtonDict.ContainsKey(t))
        {
            
            taskToButtonDict[t].GetComponent<Image>().color = colorFromRGB(170, 120, 166);

        }
        updateCompleteness(t, 0f);
        
        generateTaskPopup(t.children, Input.mousePosition);

        if (t.children.Count != 0)
        {
            
            if (t.children[0].tags.Length > 0) {
                Debug.Log(t.children[0].tags[0]);
            }
            if(Array.IndexOf(t.children[0].tags, "start") > -1)
            {
                showingSentance = true;
                t.complete = true;
                generateAutoTree(t.children[0]);
                return;
            }
        }
        //depth first search



    }

    //simon u dumb ass this is shitting the bed because of the other
    //animation coroutines prob not working super great with this
    //you can prob fix this by passing a flag to not generate the coroutines?? idk it should work

    public void generateAutoTree(TaskSO t) {
        StartCoroutine(snhCo(t));
    }

    
    //rename this
    IEnumerator snhCo(TaskSO t) {
        
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(test(t));

        if (Array.IndexOf(t.tags, "end") > -1)
        {
            showingSentance = false;
            yield return null;
        }
        else {
            foreach (TaskSO child in t.children)
            {

                yield return StartCoroutine(snhCo(child));
            }
        }
        
        


        yield return null;
    }

    //rename this
    IEnumerator test(TaskSO t) {

        if (Array.IndexOf(t.tags, "end") > -1)
        {
            yield return null;
        }
        else {
            setNewHead(t);
            yield return null;
        }
        
        
    }



    public void achievementAnimation(TaskSO t, GameObject objectAdded, Vector3 start, Vector3 end, float time) {
        Image i = Instantiate(achievementAnimImagePrefab).GetComponent<Image>();
        i.sprite = t.icon;
        
        i.transform.SetParent(canvas.transform);
        i.transform.position = start;
        i.gameObject.transform.localScale = Vector3.one;
        Tween anim = i.transform.DOMove(end, time);
        Destroy(i.gameObject, anim.Duration());
    }

    

    
}
