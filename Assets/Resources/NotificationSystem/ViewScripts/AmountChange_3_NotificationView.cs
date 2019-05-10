using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
public class AmountChange_3_NotificationView : NotificationView
{
    [SerializeField]
    Sprite upImage, downImage;
    [SerializeField]
    Image upOrDown;
    [SerializeField]
    TMPro.TextMeshProUGUI AmountText;
    [SerializeField]
    TMPro.TextMeshProUGUI Summary;

     public override void ResolveNotificationData(NotificationData notifiData)
    {
        AmountChange_3_NotificationData data = (AmountChange_3_NotificationData)notifiData;
        upOrDown.sprite = data.amount > 0f ? upImage : downImage;
        AmountText.text = "$" + data.amount;
        Summary.text = data.summary;
    }
}
