using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AudioManaging;

public class EmailViewController : MonoBehaviour
{
    [SerializeField]
    float m_lerpSpeed = 10f;
    [SerializeField]
    float m_lerpThreshold = 0.997f;

    [SerializeField]
    private EmailListView m_emailListView;
    public EmailListView EmailListView => m_emailListView;

    [SerializeField]
    private EmailContentView m_emailContentView;
    public EmailContentView EmailContentView => m_emailContentView;

    private Coroutine m_pageSwitchCoroutine;

    enum Page { List, Content }
    private Page m_currentPage = Page.List;

    private Vector3 ContentCenterPos;
    private Vector3 ContentRightPos
    {
        get
        {
            return ContentCenterPos + EmailContentView.RectTransform.rect.width * EmailContentView.transform.right;
        }
    }
    public void OnInit()
    {
        ContentCenterPos = EmailContentView.transform.localPosition;
        EmailContentView.transform.localPosition = ContentRightPos;
        EmailContentView.OnInit();
    }

    public void OnExecute(EmailExecutionObj exeObj)
    {
        if (NotificationController.Instance && exeObj.IsPlayerTalking == false)
        {
            NewEmail_1_NotificationData data = NotificationController.Instance.CreateDataInstance(Utility.App.Mail, "NewEmail") as NewEmail_1_NotificationData;
            data.FromWhom = exeObj.WithWho;
            data.Subject = exeObj.Subject;
            NotificationController.Instance.PushNotification(data, exeObj.Situation_Id.ToString());
            AudioManager.Instance.Play(AudioEnum.Email_Notifi_A);
        }
        if (exeObj.IsPlayerTalking)
        {
            AudioManager.Instance.Play(AudioEnum.Email_Sent);
        }
        if (m_currentPage == Page.List)
        {
            EmailListView.UpdateView();
        }
        if (m_currentPage == Page.Content)
        {
            EmailContentView.UpdateView();
            EmailContentView.ScrollToBottom();
            if (EmailContentView.CurrentSituationId == exeObj.Situation_Id)
            {
                EmailContentView.OnExecuteObj(exeObj);
            }
        }

    }

    public void OnShowBeforeTransition()
    {
        m_currentPage = Page.List;
        EmailContentView.transform.localPosition = ContentRightPos;
        switch (m_currentPage)
        {
            case Page.List:
                EmailListView.OnShowBeforeTransition();
                break;
            case Page.Content:
                EmailContentView.OnShowBeforeTransition();
                break;
        }
    }

    public void OnHide()
    {
        if (m_pageSwitchCoroutine != null)
            StopCoroutine(m_pageSwitchCoroutine);
        switch (m_currentPage)
        {
            case Page.List:
                EmailListView.OnHide();
                EmailContentView.transform.localPosition = ContentRightPos;
                break;
            case Page.Content:
                EmailContentView.OnHide();
                EmailContentView.transform.localPosition = ContentCenterPos;
                break;
        }
    }

    private void switchToPage(Page page)
    {
        if (m_currentPage == page)
            return;
        switch (page)
        {
            case Page.List:
                EmailListView.OnShowBeforeTransition();
                EmailContentView.OnHide();
                break;
            case Page.Content:
                EmailListView.OnHide();
                EmailContentView.OnShowBeforeTransition();
                break;
        }
        m_currentPage = page;
        if (m_pageSwitchCoroutine != null)
            StopCoroutine(m_pageSwitchCoroutine);
        m_pageSwitchCoroutine = StartCoroutine(switch2Page(page));
    }
    IEnumerator switch2Page(Page page, UnityAction postTransition = null)
    {
        float progress = 0f;

        Vector3 startPos, endPos;
        switch (page)
        {
            case Page.List:
                startPos = ContentCenterPos;
                endPos = ContentRightPos;
                break;
            case Page.Content:
                startPos = ContentRightPos;
                endPos = ContentCenterPos;
                break;
            default:
                startPos = ContentCenterPos;
                endPos = ContentRightPos;
                break;
        }
        EmailContentView.transform.localPosition = startPos;
        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, m_lerpSpeed * Time.deltaTime);
            EmailContentView.transform.localPosition = Vector3.Lerp(startPos, endPos, progress);
            if (progress > m_lerpThreshold)
            {
                EmailContentView.transform.localPosition = endPos;
                if (postTransition != null)
                    postTransition();
                yield break;
            }
            yield return null;
        }
    }
    public void OpenEmail(int situationId)
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);

        if (m_currentPage != Page.List)
            return;
        EmailContentView.SetCurrentSituation(situationId);
        switchToPage(Page.Content);
    }

    public void BackToListView()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);

        switchToPage(Page.List);
    }
}
