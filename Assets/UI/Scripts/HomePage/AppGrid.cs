using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManaging;
[ExecuteInEditMode]
public class AppGrid : MonoBehaviour
{
    [Header("App Info")]
    [SerializeField]
    AppInfo AppInfo;

    [Header("UI Objects Reference")]
    [SerializeField]
    Image AppIcon;
    [SerializeField]
    TMPro.TextMeshProUGUI AppName;

    public void LoadInfo()
    {
        if (!AppInfo)
            return;
        if (AppIcon)
            AppIcon.sprite = AppInfo.AppIcon;
        if (AppName)
            AppName.text = AppInfo.AppName;
    }

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("start");
        LoadInfo();
        AppInfo.LoadAppInfo();

    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        LoadInfo();
#endif
    }

    public void OnClick()
    {
        ViewController.Instance.ClickApp(AppInfo);
        AudioManager.Instance.Play(AudioEnum.Button_Default);
    }
}
