using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MessageFrom_0_NotificationView : NotificationView
{
    [SerializeField]
    TMPro.TextMeshProUGUI FromWho, Message;
     public override void ResolveNotificationData(NotificationData notifiData)
    {
        MessageFrom_0_NotificationData data = (MessageFrom_0_NotificationData)notifiData;
        FromWho.text = data.FromWho;
        Message.text = data.Message;

    }
}
