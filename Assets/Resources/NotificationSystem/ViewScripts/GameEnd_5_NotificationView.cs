using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class GameEnd_5_NotificationView : NotificationView
{
    
    public override void ResolveNotificationData(NotificationData notifiData)
    {
        GameEnd_5_NotificationData data = (GameEnd_5_NotificationData)notifiData;
    }

    protected override void OnClick()
    {
        SettingAppController.Instance.OpenSummary();
    }
}
