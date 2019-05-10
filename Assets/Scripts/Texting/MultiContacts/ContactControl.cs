using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContactControl : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_name;

    [SerializeField]
    TMPro.TextMeshProUGUI m_messagePreview;

    [SerializeField]
    InitialControl InitialControl;

    [SerializeField]
    GameObject StarMark;

    private bool m_unfinishedMessage;

    private string m_currentName;

    public string CurrentName { get => m_currentName; }

    public bool UnfinishedMessage { get => m_unfinishedMessage; set { m_unfinishedMessage = value; SetFontStyle(); } }

    public void SetName(string name)
    {
        m_name.text = Utility.ParseName(name);
        m_currentName = name;
        InitialControl.SetInitial();
    }

    public void OnClick()
    {
        TextAppController.Instance.TextViewController.OpenMessage(m_currentName);
    }

    public void SetMessage(string message)
    {
        m_messagePreview.text = message;
        m_messagePreview.GetComponent<TextSizeControl>().UpdateDefaultText();
    }

    public void SetFontStyle()
    {
        if (m_unfinishedMessage)
        {
            m_name.fontStyle = TMPro.FontStyles.Bold;
            m_messagePreview.fontStyle = TMPro.FontStyles.Bold;
            StarMark.SetActive(true);
        }
        else {
            m_name.fontStyle = TMPro.FontStyles.Normal;
            m_messagePreview.fontStyle = TMPro.FontStyles.Normal;
            StarMark.SetActive(false);
        }
    }
}
