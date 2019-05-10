using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AudioManaging;

public class TextViewController : MonoBehaviour
{
    [SerializeField]
    public TextProgression TextProg;

    [SerializeField]
    public ContactsController ContactsController;

    [SerializeField]
    RectTransform TextProgTrans;

    [SerializeField]
    Transform ContactsConTrans;

    public enum Page {Contact, Message}

    Page m_currentPage = Page.Contact;

    Coroutine m_pageSwitchingCoroutine;

    [Header("Page Switch Speed")]
    [SerializeField]
    float m_lerpSpeed, m_lerpThreshold;

    private Vector3 m_textProgRightPos
    {
        get
        {
            return m_textProgCenterPos + ((RectTransform)TextProgTrans).rect.width * TextProgTrans.right.normalized;
        }
    }

    private Vector3 m_textProgCenterPos;

    public void SetDefaultTextProgPos()
    {
        m_textProgCenterPos = TextProgTrans.localPosition;
    }

    public void OnInit()
    {
        TextProgTrans.position = m_textProgRightPos;
    }

    public void OnShowBeforeTransition()
    {
        m_currentPage = Page.Contact;
        //ContactsController.gameObject.SetActive(true);
        //TextProg.gameObject.SetActive(true);
        TextProgTrans.position = m_textProgRightPos;
        switch (m_currentPage)
        {
            case Page.Contact:
                ContactsController.OnShow();
                break;
            case Page.Message:
                TextProg.OnShow();
                break;
        }
    }

    public void OnHide()
    {
        if (m_pageSwitchingCoroutine != null)
            StopCoroutine(m_pageSwitchingCoroutine);
        switch (m_currentPage)
        {
            case Page.Contact:
                ContactsController.OnHide();
                TextProgTrans.localPosition = m_textProgRightPos;
                break;
            case Page.Message:
                TextProg.OnHide();
                TextProgTrans.localPosition = m_textProgCenterPos;
                break;
        }
    }

    public void OnTextMsgObjExecute(string sender)
    {
        //if (m_currentPage == Page.Message && sender == TextProg.CurrentSpeaker)
        //{
        //    TextProg.StartToShowText();
        //}
        if (m_currentPage == Page.Contact)
        {
            ContactsController.UpdateView();
        }
    }

    public void Switch2Page(Page page)
    {
        if (m_currentPage == page)
        {
            return;
        }
        if (m_pageSwitchingCoroutine != null)
        {
            StopCoroutine(m_pageSwitchingCoroutine);
            m_pageSwitchingCoroutine = null;
        }
        m_currentPage = page;
        switch (page)
        {
            case Page.Contact:
                ContactsController.OnShow();
                TextProg.OnHide();
                break;
            case Page.Message:
                ContactsController.OnHide();
                TextProg.OnShow();
                break;
        }
        m_pageSwitchingCoroutine = StartCoroutine(switch2Page(page));
    }

    IEnumerator switch2Page(Page page, UnityAction postTransition = null)
    {
        float progress = 0f;

        Vector3 startPos, endPos;
        switch (page)
        {
            case Page.Contact:
                startPos = m_textProgCenterPos;
                endPos = m_textProgRightPos;
                break;
            case Page.Message:
                startPos = m_textProgRightPos;
                endPos = m_textProgCenterPos;
                break;
            default:
                startPos = m_textProgCenterPos;
                endPos = m_textProgCenterPos;
                break;
        }
        TextProgTrans.localPosition = startPos;
        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, m_lerpSpeed * Time.deltaTime);
            TextProgTrans.localPosition = Vector3.Lerp(startPos, endPos, progress);
            if (progress > m_lerpThreshold)
            {
                TextProgTrans.localPosition = endPos;
                if(postTransition != null)
                    postTransition();
                yield break;
            }
            yield return null;
        }
    }

    public void OpenMessage(string sender)
    {
        if (m_currentPage != Page.Contact)
            return;
        TextProg.SetCurrentSpeaker(sender);
        Switch2Page(Page.Message);
        AudioManager.Instance.Play(AudioEnum.Button_Default);

    }

    public void BackToContact()
    {
        Switch2Page(Page.Contact);
        AudioManager.Instance.Play(AudioEnum.Button_Default);
    }

}
