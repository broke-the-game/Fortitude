using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ButtonControl : MonoBehaviour
{
    public Button Button => GetComponent<Button>();

    public TMPro.TextMeshProUGUI Text => GetComponentInChildren<TMPro.TextMeshProUGUI>();

    public int ProfileId { get; private set; }

    public void SetContent(int profileId, Color fontColor, string buttonName)
    {
        ProfileId = profileId;
        Text.color = fontColor;
        Text.text = buttonName;
    }
    public void OnClick()
    {
        StartingModule.Instance.CheckProfileInfo(ProfileId);
    }
}
