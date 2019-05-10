using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AmountChange_3_NotificationData : NotificationData
{
    public float amount;
    public string summary;
     //Auto generated content
     protected override void initialize()
    {
        int appInt = 3;
        app = (Utility.App)appInt;
        templateId = "AmountChange";
    }
 
}
