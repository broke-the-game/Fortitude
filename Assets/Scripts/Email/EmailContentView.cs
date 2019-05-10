using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManaging;

public class EmailContentView : MonoBehaviour
{
    public RectTransform RectTransform => (RectTransform)transform;

    private int m_currentSituationId;

    [SerializeField]
    float m_lerpSpeed = 10f, m_lerpThreshold = 0.997f;

    [SerializeField]
    TMPro.TextMeshProUGUI Subject, Option1, Option2, ReplyToWhom, ReplyContent;

    [SerializeField]
    RectTransform ReplyWindow, EmailContentContainer, ReplyOptionPanel;

    [SerializeField]
    RectTransform BackPanel;

    [SerializeField]
    GameObject EmailPrefab;

    [SerializeField]
    ScrollRect MainScroll;

    private Vector3 m_replyPopUpCenterPos;
    private Vector3 m_replyPopUpDownPos => m_replyPopUpCenterPos - ReplyWindow.rect.height * ReplyWindow.up;

    private int m_currentSelection;
    private bool isReplyWndOn;
    private Coroutine m_replyWindowCoroutine;

    List<EmailContentControl> EmailList = new List<EmailContentControl>();

    EmailDataManager.EmailGroupInfo GroupInfo => EmailAppController.Instance.EmailDataManager.GetEmailGroup(CurrentSituationId);
    EmailExecutionObj ExeObj => EmailAppController.Instance.EmailExeManager.GetEmailExe(CurrentSituationId);

    Coroutine m_waitForNextEmail;

    public int CurrentSituationId { get => m_currentSituationId;}

    public void OnInit()
    {
        m_replyPopUpCenterPos = ReplyWindow.localPosition;
        ReplyWindow.localPosition = m_replyPopUpDownPos;
        isReplyWndOn = false;
        BackPanel.gameObject.SetActive(false);
    }

    public void UpdateView()
    {
        int emailCount = 0;

        if (GroupInfo != null)
        {
            Subject.text = GroupInfo.Subject;
            EmailDataManager.EmailGroupInfo.EmailInfo emailInfo;
            for (int i = 0; i < GroupInfo.GetEmailCount(); i++)
            {
                emailInfo = GroupInfo.GetEmailInfo(i);
                if (emailCount > EmailList.Count - 1)
                {
                    EmailList.Add(Instantiate(EmailPrefab, EmailContentContainer).GetComponent<EmailContentControl>());
                }
                EmailList[emailCount].SetInfo(GroupInfo.Sender, emailInfo.sentByPlayer, emailInfo.content);
                emailCount++;
            }
        }
        if (ExeObj != null)
        {
            Subject.text = ExeObj.Subject;
            if (emailCount > EmailList.Count - 1)
            {
                EmailList.Add(Instantiate(EmailPrefab, EmailContentContainer).GetComponent<EmailContentControl>());
            }
            EmailList[emailCount].SetInfo(ExeObj.WithWho, ExeObj.IsPlayerTalking, ExeObj.Content);

            emailCount++;

            if (ExeObj.NextEmailCallback != null && ExeObj.NextEmailCallback.Length == 2)
            {
                ReplyOptionPanel.gameObject.SetActive(true);
                Option1.text = ExeObj.OptionDescription[0];
                Option2.text = ExeObj.OptionDescription[1];
            }
            else
            {
                ReplyOptionPanel.gameObject.SetActive(false);
            }
        }
        else
        {
            ReplyOptionPanel.gameObject.SetActive(false);
        }
        EmailContentControl emailToDelete;
        for (; EmailList.Count > emailCount;)
        {
            emailToDelete = EmailList[EmailList.Count - 1];
            EmailList.RemoveAt(EmailList.Count - 1);
            DestroyImmediate(emailToDelete.gameObject);
        }
    }

    public void ScrollToBottom()
    {
        float defaultPos = MainScroll.verticalNormalizedPosition;
        float targetPos = defaultPos - Mathf.Clamp01(MainScroll.viewport.rect.height/(MainScroll.content.rect.height - MainScroll.viewport.rect.height));
        Utility.ScrollTo(MainScroll, Mathf.Clamp01(targetPos));
    }

    public void OnShowBeforeTransition()
    {
        UpdateView();
        m_currentSelection = 0;
        Utility.SetScroll(MainScroll, 0f);
        if (ExeObj != null)
        {
            if (EmailAppController.Instance.IsShowBeforeTransition)
            {
                StartNextEmailCountDown();
            }
        }
    }
    public void OnHide()
    {
        if (m_waitForNextEmail != null)
        {
            StopCoroutine(m_waitForNextEmail);
            m_waitForNextEmail = null;
            if (ExeObj != null && ExeObj.NextEmailCallback != null && ExeObj.NextEmailCallback.Length == 1)
            {
                if (ExeObj.NextEmailCallback[0].AppExecution == null && NotificationController.Instance)
                {
                    NotificationController.Instance.HideNotification(Utility.App.Mail, m_currentSituationId.ToString());
                }
                AppCallbackModule.Instance.Execute(ExeObj.NextEmailCallback[0]);
                EmailAppController.Instance.EmailExeManager.OnFinish(CurrentSituationId);
            }
        }
    }

    public void SetCurrentSituation(int situationId)
    {
        m_currentSituationId = situationId;
    }

    public void OnClick(int selection)
    {
        m_currentSelection = selection;
        switch (m_currentSelection)
        {
            case 1:
                updateReplyWindowView(ExeObj.NextEmail[0]);
                switchReplyPanel(true);
                break;
            case 2:
                updateReplyWindowView(ExeObj.NextEmail[1]);
                switchReplyPanel(true);
                break;
        }
        AudioManager.Instance.Play(AudioEnum.Button_Default);
    }

    private void updateReplyWindowView(string replyContent)
    {
        ReplyToWhom.text = "To: " + ExeObj.WithWho;
        ReplyContent.text = replyContent;
    }

    public void OnBack()
    {
        switchReplyPanel(false);
        m_currentSelection = 0;
        AudioManager.Instance.Play(AudioEnum.Button_Default);

    }

    public void OnSend()
    {
        switchReplyPanel(false);
        AppCallback next = ExeObj.NextEmailCallback[m_currentSelection - 1];
        EmailAppController.Instance.EmailExeManager.OnFinish(CurrentSituationId);
        m_currentSelection = 0;
        UpdateView();
        AppCallbackModule.Instance.Execute(next);
        AudioManager.Instance.Play(AudioEnum.Button_Default);

    }

    private void switchReplyPanel(bool on)
    {
        if (on == isReplyWndOn)
        {
            return;
        }
        isReplyWndOn = on;
        BackPanel.gameObject.SetActive(on);
        if (m_replyWindowCoroutine != null)
            StopCoroutine(m_replyWindowCoroutine);
        m_replyWindowCoroutine = StartCoroutine(switchReplyPanelCor(on));
    }

    IEnumerator switchReplyPanelCor(bool on)
    {
        float progress = 0;
        Vector3 startPos, endPos;
        switch (on)
        {
            case true:
                startPos = m_replyPopUpDownPos;
                endPos = m_replyPopUpCenterPos;
                break;

            case false:
                startPos = m_replyPopUpCenterPos;
                endPos = m_replyPopUpDownPos;
                break;
            default:
                startPos = m_replyPopUpCenterPos;
                endPos = m_replyPopUpDownPos;
                break;
        }

        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, m_lerpSpeed * Time.deltaTime);
            ReplyWindow.localPosition = Vector3.Lerp(startPos, endPos, progress);
            if (progress > m_lerpThreshold)
            {
                ReplyWindow.localPosition = endPos;
                yield break;
            }
            yield return null;
        }
    }

    public void StartNextEmailCountDown()
    {
        if (m_waitForNextEmail != null)
            StopCoroutine(m_waitForNextEmail);
        m_waitForNextEmail = StartCoroutine(waitForNextEmail());
    }

    IEnumerator waitForNextEmail()
    {
        yield return new WaitForSeconds(2f);
        if (ExeObj != null && ExeObj.NextEmailCallback != null && ExeObj.NextEmailCallback.Length == 1)
        {
            if (ExeObj.NextEmailCallback[0].AppExecution == null && NotificationController.Instance)
            {
                NotificationController.Instance.HideNotification(Utility.App.Mail, m_currentSituationId.ToString());
            }
            AppCallback next = ExeObj.NextEmailCallback[0];
            EmailAppController.Instance.EmailExeManager.OnFinish(CurrentSituationId);
            UpdateView();
            AppCallbackModule.Instance.Execute(next);
        }
        m_waitForNextEmail = null;
    }

    public void OnExecuteObj(EmailExecutionObj exeObj)
    {
        if (EmailAppController.Instance.IsShowBeforeTransition)
        {
            StartNextEmailCountDown();
        }
    }
}
