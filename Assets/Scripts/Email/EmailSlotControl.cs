using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailSlotControl : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI m_subject, m_sender, m_preview;

    [SerializeField]
    GameObject UnreadDotMark;

    public int SituationId{ get; private set; }

    public void OnClick()
    {
        EmailAppController.Instance.EmailViewController.OpenEmail(SituationId);
    }

    public void SetInfo(string subject, string sender, string preview, bool read, int situationId)
    {
        m_subject.text = subject;
        m_sender.text = sender;
        m_preview.text = preview;
        UnreadDotMark.SetActive(!read);
        SituationId = situationId;
    }
}
