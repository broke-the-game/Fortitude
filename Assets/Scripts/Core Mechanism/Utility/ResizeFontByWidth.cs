using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
public class ResizeFontByWidth : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI text;

    [SerializeField]
    uint wordCount;
    // Start is called 1before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text.fontSize = 2.4f * ((RectTransform)transform).rect.width / wordCount;
    }
}
