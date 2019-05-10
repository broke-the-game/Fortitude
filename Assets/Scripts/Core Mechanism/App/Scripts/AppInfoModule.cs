using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class AppInfoModule : MonoBehaviour
{
    #region Singleton
    private static AppInfoModule m_instance;
    public static AppInfoModule Instance { get { return m_instance; } }
    private void Awake()
    {
    }

    #endregion

    [SerializeField]
    AppInfo[] AppInfoList;

    private void OnEnable()
    {
        m_instance = this;

        if (AppInfoList == null)
        {
            AppInfoList = new AppInfo[(int)Utility.App.COUNT];
        }
    }

    public void SetValue(Utility.App app, AppInfo value)
    {
        AppInfoList[(int)app] = value;
    }

    public AppInfo[] GetAppInfos()
    {
        AppInfo[] appInfos = new AppInfo[0];
        AppInfoList.CopyTo(appInfos, 0);
        return appInfos;
    }

    public AppInfo GetAppInfo(Utility.App app)
    {
        return AppInfoList[(int)app];
    }
}
