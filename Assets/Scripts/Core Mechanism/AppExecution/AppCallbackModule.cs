using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AppCallbackModule : MonoBehaviour
{
    #region Singleton
    private static AppCallbackModule m_instance;
    public static AppCallbackModule Instance { get { return m_instance; } }
    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    public void Execute(AppCallback appCallback)
    {
        if (appCallback.AppExecution)
        {
            AppExecutionModule.Instance.Execute(appCallback.AppExecution);
        }
        if (appCallback.CallbackFuntion != null)
        {
            appCallback.CallbackFuntion.Invoke(appCallback.CallbackFuntion.Args);
        }
    }
}
