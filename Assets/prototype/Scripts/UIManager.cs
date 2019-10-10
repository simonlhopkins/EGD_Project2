using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update


    public GameObject canvas;
    //task popup prefabs
    public GameObject popupContainer;
    public GameObject taskButton;

    //dictionary for keeping track of everything on the screen
    Dictionary<TaskSO, GameObject> taskToButtonDict = new Dictionary<TaskSO, GameObject>();

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //called one time on mouse down
    //creates, positions, and adds buttons with corresponding tasks
    public void generateTaskPopup(List<TaskSO> tasksToDisplay, Vector3 position) {
        if (tasksToDisplay.Count == 0) {
            return;
        }
        GameObject _popupContainer = Instantiate(popupContainer);
        
        
        _popupContainer.transform.SetParent(canvas.transform, false);
        _popupContainer.transform.position = position;
        foreach (TaskSO task in tasksToDisplay) {
            
            createButton(task, _popupContainer.transform);
        }
    }

    public void createButton(TaskSO task, Transform parent) {
        
        GameObject _newButton = Instantiate(taskButton);
        _newButton.transform.position = Vector3.zero;
        _newButton.transform.SetParent(parent, true);
        _newButton.GetComponentInChildren<Text>().text = task.title;
        if (!taskToButtonDict.ContainsKey(task)) {
            taskToButtonDict.Add(task, _newButton);
        }
        
        _newButton.GetComponent<Button>().onClick.AddListener(delegate { printTask(task); });

    }

    public void printTask(TaskSO t) {
        generateTaskPopup(t.children, Input.mousePosition);
        t.complete = true;
        taskToButtonDict[t].GetComponent<Image>().color = Color.green;
    }
}
