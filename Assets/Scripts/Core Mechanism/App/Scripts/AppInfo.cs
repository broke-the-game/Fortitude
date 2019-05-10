using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.UI;

[CreateAssetMenu(fileName = "AppInfo", menuName = "Fortitude/App", order = 1)]

public class AppInfo : ScriptableObject
{
    [Header("Basic Info")]
    public Utility.App App;

    [Header("Homepage Button")]
    public Sprite AppIcon;
    public string AppName;

    [Header("App Scene")]
    public string SceneName;


    public void LoadAppInfo()
    {
        if (!AppInfoModule.Instance)
            return;
        AppInfoModule.Instance.SetValue(App, this);

        if (!AppSceneLoader.Instance)
            return;
        AppSceneLoader.Instance.LoadAppScene(App, SceneName);
    }
}
