using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(LayoutElement))]
public class FillRestHeight : MonoBehaviour
{
    RectTransform RectTransform => (RectTransform)transform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float height = ((RectTransform)transform.parent).rect.height;
        for (int i = 0; i < transform.parent.childCount; i++)
        {
            Transform child = transform.parent.GetChild(i);
            if (child != transform && child.gameObject.active)
            {
                height -= ((RectTransform)child).rect.height;
            }
        }
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
        GetComponent<LayoutElement>().enabled = false;
        GetComponent<LayoutElement>().enabled = true;
    }
}
