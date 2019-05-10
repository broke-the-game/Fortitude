using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ResizeWindowWidth : MonoBehaviour, Resizable
{
    [SerializeField]
    TMPro.TextMeshProUGUI text;
    [SerializeField]
    RectTransform window;
    [SerializeField]
    float offset;
    public void Resize()
    {
        window.sizeDelta = new Vector2(Mathf.Min(text.preferredWidth + offset, ((RectTransform)window.parent).rect.width ), window.sizeDelta.y);
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Resize();
    }
}
