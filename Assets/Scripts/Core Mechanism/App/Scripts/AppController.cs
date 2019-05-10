using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class AppController : MonoBehaviour
{
    public RectTransform RootUI;

    public AppInfo AppInfo;

    private bool m_isShow;

    private bool m_isShowBeforeTransition = false;

    public bool IsShow { get { return m_isShow; } set { m_isShow = value; } }

    public bool IsShowBeforeTransition { get => m_isShowBeforeTransition; set => m_isShowBeforeTransition = value; }

    public abstract void ExecuteScriptableObject(AppExecution scriptable);
    public abstract void OnShow();

    public abstract void OnShowBeforeTransition();
    public abstract void OnHide();
    public abstract void Init();

    private void Start()
    {
        Camera[] cameras = Camera.allCameras;
        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] != Camera.main)
            {
                DestroyImmediate(cameras[i].gameObject);
            }
        }
        loadAppController();
        if(ViewController.Instance)
            ViewController.Instance.InitAppScene(AppInfo.App);
        Init();
    }

    private void loadAppController()
    {
        if (m_appControllers.ContainsKey(AppInfo.App))
            m_appControllers.Remove(AppInfo.App);
        m_appControllers.Add(AppInfo.App, this);
    }

    public void SetAppInfo(AppInfo appInfo)
    {
        AppInfo = appInfo;
    }

    private static Dictionary<Utility.App,AppController> m_appControllers = new Dictionary<Utility.App, AppController>();

    public static AppController[] GetAppControllers()
    {
        return FindObjectsOfType<AppController>();
    }

    public static AppController GetAppController(Utility.App appType)
    {
        AppController appController = null;
        m_appControllers.TryGetValue(appType, out appController);
        return appController;
    }

}
