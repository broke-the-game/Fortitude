using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteAlways]
[RequireComponent(typeof(VerticalLayoutGroup))]
public class VerticalLayoutHeight : MonoBehaviour
{
    RectTransform RectTransform { get { return (RectTransform)transform; } }

    [SerializeField]
    RectTransform minHeight;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetHeight();
    }
    public void SetHeight()
    {
        float height = 0f;
        for (int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).gameObject.activeInHierarchy)
                height += ((RectTransform)transform.GetChild(i)).rect.height;
        }
        if (minHeight && height < minHeight.rect.height)
        {
            height = minHeight.rect.height;
        }
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, height);
    }
}
