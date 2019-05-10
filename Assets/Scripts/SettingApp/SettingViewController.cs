using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DataBank;
using UnityEngine.UI;
using AudioManaging;

public class SettingViewController : MonoBehaviour
{
    [SerializeField]
    ProfileInfoController ProfileInfoControl;

    [SerializeField]
    RectTransform ProfileInfoPage, About, ProfileSwitchPage, Help, GameLogGE, GameLogBE, GameLogButton;

    [SerializeField]
    TMPro.TextMeshProUGUI ProfileName;

    [SerializeField]
    Image Icon;

    [System.Serializable]
    public class SerializableDictColorSprite : SerializableDictionary<Color, Sprite> { }

    [SerializeField, HideInInspector ]
    public SerializableDictColorSprite IconHashmap = new SerializableDictColorSprite();

    public bool NeedToOpenGameLog = false;

    bool m_goodEnding;
    public void OnInit()
    {
        GameLogButton.gameObject.SetActive(false);
        ProfileEntity pe = ProfileInfo.Instance.ProfileEntity;
        ProfileName.text = pe.GetField(ProfileEntity.DataField.Name) + " <sprite=0>";
        Icon.sprite = IconHashmap[ProfileEntity.GetColor(pe.GetField(ProfileEntity.DataField.Color))];
        ProfileInfoControl.ParseProfile(pe);
    }

    public void GameEnd(bool goodEnding)
    {
        m_goodEnding = goodEnding;
        GameLogButton.gameObject.SetActive(true);
    }

    public void OnShowBeforeTransition()
    {
        CloseSwitchWindow();
        HideAllPages();
        if (NeedToOpenGameLog)
        {
            OpenPage("GameLog");
            NeedToOpenGameLog = false;
        }
    }

    public void OpenPage(string param)
    {
        switch (param)
        {
            case "Profile":
                HideAllPages();
                ProfileInfoPage.gameObject.SetActive(true);
                break;
            case "About":
                HideAllPages();
                About.gameObject.SetActive(true);
                break;

            case "Help":
                HideAllPages();
                Help.gameObject.SetActive(true);
                break;
            case "GameLog":
                HideAllPages();
                if(m_goodEnding)
                    GameLogGE.gameObject.SetActive(true);
                else
                    GameLogBE.gameObject.SetActive(true);
                break;
            case "Switch":
                ProfileSwitchPage.gameObject.SetActive(true);
                break;
        }
        AudioManager.Instance.Play(AudioEnum.Button_Default);
    }

    public void CloseSwitchWindow()
    {
        ProfileSwitchPage.gameObject.SetActive(false);
        AudioManager.Instance.Play(AudioEnum.Button_Default);
    }

    public void StartNewProfile()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);
        SceneManager.LoadScene("Start");
    }

    public void HideAllPages()
    {
        ProfileInfoPage.gameObject.SetActive(false);
        About.gameObject.SetActive(false);
        ProfileSwitchPage.gameObject.SetActive(false);
        Help.gameObject.SetActive(false);
        GameLogGE.gameObject.SetActive(false);
        GameLogBE.gameObject.SetActive(false);
        AudioManager.Instance.Play(AudioEnum.Button_Default);

    }

}
