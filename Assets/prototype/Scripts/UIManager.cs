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

    //called one time on mouse down
    //creates, positions, and adds buttons with corresponding tasks
    public void generateTaskPopup(List<TaskSO> tasksToDisplay, Vector3 position) {
        if (tasksToDisplay.Count == 0) {
            
            return;
        }

        currentTasks = tasksToDisplay;
        Vector3 pointToSpawn = findValidPositionForPopup(position);
        drawTail(position, pointToSpawn, canvas.transform);
        GameObject _popupContainer = Instantiate(popupContainer);



        _popupContainer.transform.SetParent(canvas.transform, false);
        
        _popupContainer.transform.position = pointToSpawn;

        
        List<GameObject> renderedButtons = new List<GameObject>();
        foreach (TaskSO task in tasksToDisplay) {

            renderedButtons.Add(createButton(task, _popupContainer.transform));


        }
        Canvas.ForceUpdateCanvases();
        int i = 0;
        foreach (GameObject button in renderedButtons) {
            Vector3 originalPos = button.GetComponent<RectTransform>().position;
            button.transform.position = button.GetComponent<RectTransform>().position + (Vector3.right * 100f);
            Sequence s = DOTween.Sequence();
            s.Append(button.transform.DOMove(originalPos, 0.5f).SetDelay(i*0.1f));
            s.Append(button.transform.DOShakeRotation(1f, 2f).SetLoops(int.MaxValue));
            
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
            tryPoint = new Vector2(mouseWP.x, mouseWP.y) + Random.insideUnitCircle.normalized*5f;
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
    public void drawTail(Vector3 startPos, Vector3 endPos, Transform _parent) {
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
        Debug.Log(vecToTarg.magnitude/ rt.rect.height);
        rt.localScale = new Vector3(0.5f, 0f, 1f);
        rt.DOScaleY(vecToTarg.magnitude / rt.rect.height, 0.5f);
        //rt.localScale = new Vector3(0.1f, vecToTarg.magnitude / rt.rect.height, 1f);
    }

    

    public GameObject createButton(TaskSO task, Transform parent) {
        
        GameObject _newButton = Instantiate(taskButton);
        //_newButton.transform.position = Vector3.zero;
        _newButton.transform.SetParent(parent, true);
        _newButton.GetComponentInChildren<Text>().text = task.title;
        if (!taskToButtonDict.ContainsKey(task)) {
            taskToButtonDict.Add(task, _newButton);
        }
        

        //_newButton.GetComponent<RectTransform>().DOAnchorPos(originalPos, 1f);
        _newButton.GetComponent<Button>().onClick.AddListener(delegate { setNewHead(task); });

        return _newButton;
    }

    public bool allChildrenComplete(TaskSO task) {
        
        foreach (TaskSO t in task.children)
        {
            if (!t.complete)
            {
                return false;
            }
            allChildrenComplete(t);
        }
        return true;
    }

    public void setNewHead(TaskSO t) {
        //need to get siblings of task

        //if (!currentTasks.Contains(t))
        //{
        //    return;
        //}
        t.complete = true;
        if (allChildrenComplete(t)) {
            taskToButtonDict[t].GetComponent<Image>().color = Color.green;
        }
        Debug.Log(t.children);
        generateTaskPopup(t.children, Input.mousePosition);

        //iterate down to see if all of the chikdren are complete

        //if (allComplete) {
        //    Debug.Log("All are complete");
        //    foreach (TaskSO task in t.parent.children)
        //    {


        //        if (taskToButtonDict[task].transform.parent.gameObject) {
        //            Destroy(taskToButtonDict[task].transform.parent.gameObject);
        //        }
        //        taskToButtonDict.Remove(task);



        //    }
        //    if (t.parent.parent) {
        //        currentTasks = t.parent.parent.children;
        //    }

        //    return;

        //}


    }
}
