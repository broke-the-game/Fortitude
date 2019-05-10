using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewEmail_1_NotificationData : NotificationData
{
    public string FromWhom;
    public string Subject;
     //Auto generated content
     protected override void initialize()
    {
        int appInt = 1;
        app = (Utility.App)appInt;
        templateId = "NewEmail";
    }
 
}
