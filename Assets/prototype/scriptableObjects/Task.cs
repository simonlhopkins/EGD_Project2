using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class Task : MonoBehaviour
{
    // Start is called before the first frame update

    public TaskSO taskSO;

    public void renderScriptableObject() {
        if (!gameObject.GetComponent<SpriteRenderer>()) {
            gameObject.AddComponent<SpriteRenderer>().sprite = taskSO.sprite;
        }
        if (!gameObject.GetComponent<Collider2D>()) {
            gameObject.AddComponent<BoxCollider2D>();
        }
    }

    private void Start()
    {
        renderScriptableObject();
        StartCoroutine("idleAnim");
    }

    private void Update()
    {
        
    }

    
    IEnumerator idleAnim()
    {
        float offset = Random.Range(-1f, 1f);
        while(true){
            yield return new WaitForSeconds(Time.deltaTime * 2f);
            transform.rotation = Quaternion.EulerAngles(new Vector3(transform.rotation.x,
                                                        transform.rotation.y,
                                                       transform.rotation.z + Mathf.Sin(offset+ Time.time*2f)/3f
                                                       ));

        }
        
        

    }
    


}
