using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextToTreeParser : MonoBehaviour {
    [Tooltip("Example: 'Assets/prototype/textFiles/test0.txt'")]
    public string filePath = "Assets/prototype/textFiles/test0.txt";

    private StreamReader reader;
    private int previousTabCount = -1;
    private string line = "";

    // Start is called before the first frame update
    void Awake() {
        return;
        TaskSO root = new TaskSO();

        reader = new StreamReader(filePath);

        int tabCount = 0;
        TaskSO currentParent = root;
        TaskSO mostRecentNode = root;

        // loop through every line of the text file
        while((line = reader.ReadLine()) != null) {
            tabCount = line.Split('\t').Length - 1; // get the tab count of this line
            line = line.Replace("\t", ""); // get rid of all tabs after getting the tab count

            // logic to update current parent based on change in tab count
            if (tabCount > previousTabCount) { // case for if a new child grouping has been introduced
                currentParent = mostRecentNode;
            } else if(tabCount < previousTabCount) { // case for if a child grouping has just been ended (special case, can go back multiple tabs)
                int tabDifference = (previousTabCount - tabCount);

                // update the current parent for however many tabs back the next line is
                for(int i=0; i<tabDifference; i++) {
                    currentParent = currentParent.parent;
                }
            }

            // split the lines of the text file into each component
            string[] info = line.Split('|');

            // update all information for the new TaskSO from the text file
            mostRecentNode = new TaskSO();
            mostRecentNode.parent = currentParent;
            currentParent.children.Add(mostRecentNode);
            mostRecentNode.icon = Resources.Load(info[0]) as Sprite;
            mostRecentNode.title = info[1];
            mostRecentNode.achievementText = info[2];

            previousTabCount = tabCount; // update the previous tab count to the tab count of the line just specified
        }

        // for testing if it is parsing properly
        foreach(TaskSO child in root.children) {
            foreach(TaskSO c in child.children) {
                Debug.Log("Parent: " + child.title + " and Child: " + c.title);
                foreach(TaskSO c2 in c.children) {
                    Debug.Log("Parent: " + c.title + " and Child: " + c2.title);
                    foreach(TaskSO c3 in c2.children) {
                        Debug.Log("Parent: " + c2.title + " and Child: " + c3.title);
                    }
                }
            }
        }
        
    }

    public TaskSO generateTree(string _filePath) {
        TaskSO root = ScriptableObject.CreateInstance<TaskSO>();

        reader = new StreamReader(_filePath);

        int tabCount = 0;
        TaskSO currentParent = root;
        TaskSO mostRecentNode = root;

        // loop through every line of the text file
        while ((line = reader.ReadLine()) != null)
        {
            tabCount = line.Split('\t').Length - 1; // get the tab count of this line
            line = line.Replace("\t", ""); // get rid of all tabs after getting the tab count

            // logic to update current parent based on change in tab count
            if (tabCount > previousTabCount)
            { // case for if a new child grouping has been introduced
                currentParent = mostRecentNode;
            }
            else if (tabCount < previousTabCount)
            { // case for if a child grouping has just been ended (special case, can go back multiple tabs)
                int tabDifference = (previousTabCount - tabCount);

                // update the current parent for however many tabs back the next line is
                for (int i = 0; i < tabDifference; i++)
                {
                    currentParent = currentParent.parent;
                }
            }

            // split the lines of the text file into each component
            string[] info = line.Split('|');

            // update all information for the new TaskSO from the text file
            mostRecentNode = ScriptableObject.CreateInstance<TaskSO>();
            mostRecentNode.parent = currentParent;
            currentParent.children.Add(mostRecentNode);
            mostRecentNode.icon = Resources.Load(info[0]) as Sprite;
            mostRecentNode.title = info[1];
            mostRecentNode.achievementText = info[2];

            previousTabCount = tabCount; // update the previous tab count to the tab count of the line just specified
        }

        // for testing if it is parsing properly
        //foreach (TaskSO child in root.children)
        //{
        //    foreach (TaskSO c in child.children)
        //    {
        //        Debug.Log("Parent: " + child.title + " and Child: " + c.title);
        //        foreach (TaskSO c2 in c.children)
        //        {
        //            Debug.Log("Parent: " + c.title + " and Child: " + c2.title);
        //            foreach (TaskSO c3 in c2.children)
        //            {
        //                Debug.Log("Parent: " + c2.title + " and Child: " + c3.title);
        //            }
        //        }
        //    }
        //}

        root.timeToAppear = 0f;
        return root;
    }
}
