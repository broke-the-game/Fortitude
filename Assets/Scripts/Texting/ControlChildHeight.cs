using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class ControlChildHeight : MonoBehaviour
{
    [SerializeField, Range(0f, 1f)]
    float m_aspectRatio;

    RectTransform RectTransform { get { return (RectTransform)transform; } }
    // Start is called before the first frame update
    void Start()
    {
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.x * m_aspectRatio);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        RectTransform.sizeDelta = new Vector2(RectTransform.sizeDelta.x, RectTransform.sizeDelta.x * m_aspectRatio);
#endif
    }
}
