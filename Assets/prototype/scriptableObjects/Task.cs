using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG.Tweening;

public class Task : MonoBehaviour
{
    // Start is called before the first frame update

    public TaskSO head;
    public HashSet<TaskSO> nodes = new HashSet<TaskSO>();
    public string filePath;
    public UIManager uiManager;


    private void Start()
    {

        uiManager = GameObject.Find("gameManager").GetComponent<UIManager>();

        head = new GameObject().AddComponent<TextToTreeParser>().generateTree(filePath);

        uiManager.generateCheckList(head, uiManager.UICompletionPanel.transform);
        StartCoroutine(animCo());

        

    }

    IEnumerator animCo() {
        while (true) {
            Tween t = transform.DOShakeRotation(1f, 10f).SetDelay(Random.Range(2f, 5f));
            yield return t.WaitForCompletion();

        }
    }


    


}
