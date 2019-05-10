using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewNews_2_NotificationView : NotificationView
{
    [SerializeField]
    TMPro.TextMeshProUGUI Title, Preview;
     public override void ResolveNotificationData(NotificationData notifiData)
    {
        NewNews_2_NotificationData data = (NewNews_2_NotificationData)notifiData;
        Title.text = data.Title;
        Preview.text = data.Preview;
    }
}
