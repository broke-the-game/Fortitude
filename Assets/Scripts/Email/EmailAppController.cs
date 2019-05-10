using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EmailAppController : AppController
{
    #region Singleton
    public static EmailAppController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    [SerializeField]
    private EmailViewController m_emailViewController;
    public EmailViewController EmailViewController => m_emailViewController;

    [SerializeField]
    private EmailDataManager m_emailDataManager;
    public EmailDataManager EmailDataManager => m_emailDataManager;

    [SerializeField]
    private EmailExecutionManager m_emailExeManager;
    public EmailExecutionManager EmailExeManager => m_emailExeManager;

    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        EmailExeManager.AddExeObj((EmailExecutionObj)scriptable);
        EmailViewController.OnExecute((EmailExecutionObj)scriptable);
    }

    public override void Init()
    {
        RequestLatestEmails();
        EmailViewController.OnInit();
    }

    public override void OnHide()
    {
        EmailViewController.OnHide();
    }

    public override void OnShow()
    {
    }

    public override void OnShowBeforeTransition()
    {
        EmailViewController.OnShowBeforeTransition();
    }

    public void RequestLatestEmails()
    {
        //request data with protocol
        List<EmailDataManager.EmailDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.EMAIL_GET_LATEST,new string[0]).Cast<EmailDataManager.EmailDataDesc>().ToList();
        EmailDataManager.AcquiredLatestEmail(data);
    }
}