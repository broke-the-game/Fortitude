using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
public class ChangeWithIconHeight : MonoBehaviour
{

    public float DefaultHeight;
    public float offset;
    [SerializeField]
    RectTransform targetTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        DefaultHeight = targetTransform.rect.height + 24f;
        float height = DefaultHeight * offset + GetComponent<LayoutElement>().preferredHeight;
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x,Mathf.Max(height, DefaultHeight));
        Debug.Log("height: " + height);
    }
}
