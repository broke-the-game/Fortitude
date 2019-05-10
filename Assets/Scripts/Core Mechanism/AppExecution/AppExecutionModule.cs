using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppExecutionModule : MonoBehaviour
{
    #region Singleton
    private static AppExecutionModule m_instance;
    public static AppExecutionModule Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    public void Execute(AppExecution appExe)
    {
        if(StateLoadingModule.Instance)
            StateLoadingModule.Instance.RegisterToCurrentState(appExe);
        Utility.GetAppController(appExe.ExecutingApp).ExecuteScriptableObject(appExe);
    }
}
