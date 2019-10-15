using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestDoTween : MonoBehaviour
{
    // Start is called before the first frame update

    public RectTransform buttonRT;
    void Start()
    {
        
        DOTween.Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            Vector2 originalSize = buttonRT.sizeDelta;
            Sequence s = DOTween.Sequence();
            s.Append(buttonRT.DOAnchorPos(new Vector2(100f, 0f), 2f));
            s.Append(buttonRT.DOAnchorPos(new Vector2(0f, 0f), 1f));
            s.Insert(0, buttonRT.DOSizeDelta(originalSize/2f, s.Duration()));
            s.SetLoops(int.MaxValue, LoopType.Yoyo);


        }
    }
}
