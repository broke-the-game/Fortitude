using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class MessageFrom_0_NotificationData : NotificationData
{
    public string FromWho, Message;
     //Auto generated content
     protected override void initialize()
    {
        int appInt = 0;
        app = (Utility.App)appInt;
        templateId = "MessageFrom";
    }
 
}
