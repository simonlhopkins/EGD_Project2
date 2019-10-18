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
        
        
    }

    public TaskSO generateTree(string _fileName) {
        TaskSO root = ScriptableObject.CreateInstance<TaskSO>();
        root.parent = null;

        reader = new StreamReader("Assets/Resources/textFiles/"+_fileName);

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
            int index = Mathf.Max(info[0].IndexOf("."), 0);
            string modded = info[0].Substring(0, index);
            mostRecentNode.icon = Resources.Load<Sprite>(modded);
            mostRecentNode.title = info[1];
            mostRecentNode.achievementText = info[2];

            if(info.Length > 3) {
                if(info[3] == "S") {
                    mostRecentNode.timeToAppear = 6;
                } else if (info[3] == "M") {
                    mostRecentNode.timeToAppear = 15;
                } else if (info[3] == "L") {
                    mostRecentNode.timeToAppear = 30;
                }                
            } else {
                mostRecentNode.timeToAppear = 0;
            }

            TaskSO getAllParentsTime = mostRecentNode;
            while(getAllParentsTime = getAllParentsTime.parent) {
                mostRecentNode.timeToAppear += getAllParentsTime.timeToAppear;
            }

            previousTabCount = tabCount; // update the previous tab count to the tab count of the line just specified
        }

        root.timeToAppear = 0f;
        return root;
    }
}
