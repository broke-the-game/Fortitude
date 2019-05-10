using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailContentControl : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI FromWhom, ToWhom, Content;

    public void SetInfo(string sender, bool isPlayerSending, string content)
    {
        if (isPlayerSending)
        {
            FromWhom.text = "Me";
            ToWhom.text = "To: " + sender;
        }
        else
        {
            FromWhom.text = sender;
            ToWhom.text = "To: Me";
        }
        Content.text = content;
    }
}
