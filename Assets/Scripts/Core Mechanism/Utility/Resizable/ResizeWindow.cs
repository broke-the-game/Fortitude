using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ResizeWindow : MonoBehaviour, Resizable
{
    [SerializeField]
    bool MinHeight, NotUseLayoutElement;

    [SerializeField]
    TMPro.TextMeshProUGUI text;

    [SerializeField]
    LayoutElement layout;

    [SerializeField]
    public float offset;

    // Update is called once per frame
    void Update()
    {
        Resize();

    }

    public void Resize()
    {

        if (NotUseLayoutElement)
        {
            ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, text.preferredHeight + offset);
        }
        else
        {
            if (MinHeight)
            {
                layout.minHeight = text.preferredHeight + offset;
            }
            else
            {
                layout.preferredHeight = text.preferredHeight + offset;
            }
        }
    }
}
