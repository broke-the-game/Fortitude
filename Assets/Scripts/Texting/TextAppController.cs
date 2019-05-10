using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using AudioManaging;

public class TextAppController : AppController
{
    #region Singleton
    private static TextAppController m_instance;
    public static TextAppController Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    [SerializeField]
    public TextDataManager TextDataManager;

    [SerializeField]
    public TextMsgObjManager TextMsgObjManager;

    [SerializeField]
    public TextViewController TextViewController;

    [SerializeField, Range(0, 100)]
    int TextObtainedCount;

    private List<string> conversationFinished = new List<string>();

    public UnityAction OnDataUpdate;

    public TextProgression TextProg { get { return TextViewController.TextProg; } }

    public ContactsController ContactsController { get { return TextViewController.ContactsController; } }

    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        TextMsgObj textMsgObj = (TextMsgObj)scriptable;
        //TextProg.SetText(textMsgObj);
        TextMsgObjManager.AddMsgObj(textMsgObj);
        TextViewController.OnTextMsgObjExecute(textMsgObj.speaker);

        if (NotificationController.Instance)
        {
            MessageFrom_0_NotificationData data = (MessageFrom_0_NotificationData) NotificationController.Instance.CreateDataInstance(Utility.App.Text, "MessageFrom");
            data.FromWho = textMsgObj.speaker;
            data.Message = textMsgObj.message[0];
            NotificationController.Instance.PushNotification(data, textMsgObj.speaker);
            if (!TextProg.IsShow || TextProg.CurrentSpeaker != textMsgObj.speaker && !textMsgObj.playerTalking)
            {
                AudioManager.Instance.Play(AudioEnum.Text_Notifi_B);
            }
        }
    }

    public override void OnShow()
    {
        //TextProg.showConversation = true;
    }

    public override void OnShowBeforeTransition()
    {

        TextViewController.OnShowBeforeTransition();
        //TextProg.StartToShowText();
    }

    public override void OnHide()
    {
        //TextProg.StopShowTexting();
        TextViewController.OnHide();
    }

    public override void Init()
    {
        TextViewController.SetDefaultTextProgPos();
        RequestLatestMessages();
        TextViewController.OnInit();
    }

    //require latest messages from beginning
    public void RequestLatestMessages()
    {
        List<TextDataManager.TextDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.TEXT_GET_LATEST, new string[] { TextObtainedCount.ToString() }).Cast<TextDataManager.TextDataDesc>().ToList();
        TextDataManager.AcquiredLatestMessage(data);
    }

    public void RequestMessagesFromIndex(int fromIndex, string sender)
    {
        List<TextDataManager.TextDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.TEXT_GET_FROM_INDEX, new string[] { fromIndex.ToString(), TextObtainedCount.ToString(), sender }).Cast<TextDataManager.TextDataDesc>().ToList();
        TextDataManager.AppendPreviousMessages(data, sender);
    }

    public void RequestMessagesBySituation(int situationId, string sender)
    {
        List<TextDataManager.TextDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.TEXT_GET_LATEST_BY_SITUATION, new string[] { situationId.ToString(), sender }).Cast<TextDataManager.TextDataDesc>().ToList();
        TextDataManager.AcquiredMessageBySituation(data, sender);
    }
    public void RequestMessagesBySenders(string[] sender)
    {
        List<TextDataManager.TextDataDesc> data;
        for (int i = 0; i < sender.Length; i++)
        {
            data = AppDataManager.RequestData(AppDataManager.Protocol.TEXT_GET_LATEST_BY_SPEAKER, new string[] { TextObtainedCount.ToString(), sender[i] }).Cast<TextDataManager.TextDataDesc>().ToList();
            TextDataManager.AcquiredLatestMessageForConversation(data);
        }
    }

    public void FinishedConversationWith(string sender)
    {
        conversationFinished.Add(sender);
    }

    public string[] GetFinishedConversationAndClear()
    {
        string[] value = conversationFinished.ToArray();
        conversationFinished.Clear();
        return value;
    }
}
