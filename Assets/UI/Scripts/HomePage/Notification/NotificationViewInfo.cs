using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NotificationViewInfo : MonoBehaviour
{
    [SerializeField]
    Image m_icon;
    [SerializeField]
    TMPro.TextMeshProUGUI m_title;
    [SerializeField]
    RectTransform m_customPanel;
    [SerializeField]
    LayoutElement m_layoutElement;
    [SerializeField, Range(0f, 3f)]
    private float m_aspectRatio;

    public bool Persistent;

    public void UpdateAspectRatio()
    {
        if (!Persistent)
            aspectRatio = m_aspectRatio;
        else
            m_aspectRatio = aspectRatio;
    }

    RectTransform rectTransform { get { return (RectTransform)transform; } }

    private float aspectRatio
    {
        get { return rectTransform.sizeDelta.x / rectTransform.sizeDelta.y; }
        set
        {
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, rectTransform.sizeDelta.x * value);
            m_layoutElement.minHeight = rectTransform.sizeDelta.x * value;
            gameObject.SetActive(true);
        }
    }

    public float AspectRatio { get { return m_aspectRatio; }set { m_aspectRatio = value; UpdateAspectRatio(); } }

    public Image Icon { get { return m_icon; } }
    public TMPro.TextMeshProUGUI Title { get { return m_title; } }
    public RectTransform CustomPanel { get { return m_customPanel; } }
}
