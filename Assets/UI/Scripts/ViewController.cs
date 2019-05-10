using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManaging;

public class ViewController : MonoBehaviour
{
    #region Singleton
    private static ViewController m_instance;
    public static ViewController Instance { get { return m_instance; } }


    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    [SerializeField]
    AppPanelView m_appPanelView;

    AppController m_currentAppController;
    public AppController CurrentAppController { get => m_currentAppController; set => m_currentAppController = value; }

    private void Start()
    {
        m_appPanelView.OnStart();
    }

    public void ClickApp(AppInfo appInfo)
    {
        if (CurrentAppController && CurrentAppController.AppInfo.App == appInfo.App)
        {
            return;
        }
        m_appPanelView.ShowAppContent(appInfo.App);
        Debug.Log("App Clicked: " + appInfo.AppName);
    }



    public void OpenApp(Utility.App app)
    {
        if (CurrentAppController && CurrentAppController.AppInfo.App == app)
        {
            return;
        }
        m_appPanelView.ShowAppContent(app);
    }

    public void InitAppScene(Utility.App app)
    {
        AppController appController = Utility.GetAppController(app);
        if (!appController)
            return;
        m_appPanelView.SetInAppPanel(appController.RootUI);
    }

    public void SetAppContentTitle(string title)
    {
        m_appPanelView.SetAppContentTitle(title);
    }

    public void CloseAppWindow()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);
        m_appPanelView.HideAppContent();
    }

}
