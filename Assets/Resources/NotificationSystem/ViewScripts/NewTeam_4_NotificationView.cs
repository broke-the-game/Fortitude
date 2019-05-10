using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewTeam_4_NotificationView : NotificationView
{
    [SerializeField]
    TMPro.TextMeshProUGUI textfield;
     public override void ResolveNotificationData(NotificationData notifiData)
    {
        NewTeam_4_NotificationData data = (NewTeam_4_NotificationData)notifiData;
        if (data.join > 0f)
        {
            textfield.text = "<b><size=+2>" + data.text + "</size></b> joined your circle.";
        }
        else if(data.join < 0f)
        {
            textfield.text = "<b><size=+2>" + data.text + "</size></b> left your circle.";
        }
        else
        {
            textfield.text = "<b><size=+2>" + data.text + "</size></b> sent you a message.";
        }
    }
}
