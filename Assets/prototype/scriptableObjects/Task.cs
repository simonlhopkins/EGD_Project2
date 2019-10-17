using System.Collections;
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

        TaskSO testHead = ScriptableObject.CreateInstance<TaskSO>();
        testHead.title = "test head";
        TaskSO testc1 = ScriptableObject.CreateInstance<TaskSO>();
        testc1.title = "c1";
        TaskSO testc2 = ScriptableObject.CreateInstance<TaskSO>();
        testc2.title = "c2";
        TaskSO testc3 = ScriptableObject.CreateInstance<TaskSO>();
        testc3.title = "c3";
        TaskSO testc4 = ScriptableObject.CreateInstance<TaskSO>();
        testc4.title = "c4";
        TaskSO testc5 = ScriptableObject.CreateInstance<TaskSO>();
        testc5.title = "c5";
        TaskSO testc6 = ScriptableObject.CreateInstance<TaskSO>();
        testc6.title = "c6";
        TaskSO testc7 = ScriptableObject.CreateInstance<TaskSO>();
        testc5.title = "c7";
        TaskSO testc8 = ScriptableObject.CreateInstance<TaskSO>();
        testc6.title = "c8";

        addNodes(testHead, new List<TaskSO>() { testc1, testc2 });
        addNodes(testc1, new List<TaskSO>() { testc3, testc4 });
        addNodes(testc4, new List<TaskSO>() { testc7, testc8 });
        addNodes(testc3, new List<TaskSO>() { testc5, testc6 });


        head = new GameObject().AddComponent<TextToTreeParser>().generateTree("Assets/prototype/textFiles/test0.txt");

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
