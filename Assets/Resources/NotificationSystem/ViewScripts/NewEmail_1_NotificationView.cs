using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewEmail_1_NotificationView : NotificationView
{
    [SerializeField]
    TMPro.TextMeshProUGUI FromWhom;

    [SerializeField]
    TMPro.TextMeshProUGUI Subject;
     public override void ResolveNotificationData(NotificationData notifiData)
    {
        NewEmail_1_NotificationData data = (NewEmail_1_NotificationData)notifiData;
        FromWhom.text = data.FromWhom;
        Subject.text = data.Subject;
    }
}
