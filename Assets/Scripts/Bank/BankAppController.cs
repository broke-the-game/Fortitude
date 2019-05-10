using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;

public class BankAppController : AppController
{
    #region Singleton
    private static BankAppController m_instance;
    public static BankAppController Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    [SerializeField]
    public Profile Profile { get => Profile.Instance; }

    [SerializeField]
    public BankOperations BankOperations;
    
    [SerializeField]
    public BankDataManager BankDataManager;

    [SerializeField, Range(0, 100)]
    int ActivitiesObtainedCount;

    float currentBalance;

    public int NotificationCount = 0;

    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        
    }

    public override void Init()
    {
        BankOperations.DrawLabel();
        RequestLatestBankActivities();
        BankOperations.Init();
    }

    public override void OnHide()
    {

    }

    public override void OnShow()
    {

    }

    public void ShowActivity()
    {
        BankOperations.ClearBankActList();
        int historyCount = BankDataManager.GetBankActivityCount();
        List<float> balanceList = new List<float>();
        for (int i = historyCount - 1; i >= 0; i--)
        {
            BankDataManager.BankActivity activity = BankDataManager.GetBankActivity(i);
            BankOperations.showActivity(activity.activitySummary, activity.amount);
            balanceList.Add(activity.balance);
        }
        balanceList.Add(Profile.defaultBankAmt);
        balanceList.Reverse();
        BankOperations.ShowGraph(balanceList);
    }

    public override void OnShowBeforeTransition()
    {
        BankOperations.onShow();
        ShowActivity();
        if (NotificationController.Instance)
        {
            for (NotificationCount--; NotificationCount >= 0; NotificationCount--)
            {
                NotificationController.Instance.HideNotification(Utility.App.Bank, NotificationCount.ToString());
            }
            NotificationCount = 0;
        }
    }

    public void RequestLatestBankActivities()
    {
        List<BankDataManager.BankDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.BANK_GET_LATEST, new string[] { ActivitiesObtainedCount.ToString() }).Cast<BankDataManager.BankDataDesc>().ToList();
        BankDataManager.AcquiredLatestBankActivity(data);
    }

    public void RequestActivitiesFromIndex(int fromIndex)
    {
        List<BankDataManager.BankDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.BANK_GET_FROM_INDEX, new string[] { fromIndex.ToString(), ActivitiesObtainedCount.ToString() }).Cast<BankDataManager.BankDataDesc>().ToList();
        BankDataManager.AppendPreviousBankActivity(data);
    }
}
