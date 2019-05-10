using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class NewNews_2_NotificationData : NotificationData
{
    public string Title;
    public string Preview;
     //Auto generated content
     protected override void initialize()
    {
        int appInt = 2;
        app = (Utility.App)appInt;
        templateId = "NewNews";
    }
 
}
