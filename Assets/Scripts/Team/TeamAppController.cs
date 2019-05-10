using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamAppController : AppController
{
    #region Singleton
    public static TeamAppController Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion
    [SerializeField]
    public TeamOperations TeamOperations;

    [SerializeField]
    public Profile Profile { get => Profile.Instance; }

    [SerializeField]
    public TeamDataManager TeamDataManager;

    [SerializeField, Range(0, 100)]
    int ActivitiesObtainedCount;

    List<string> currentMembers;

    public int NotificationCount = 0;

    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        TeamExec teamExec = (TeamExec)scriptable;

    }

    public override void Init()
    {
        // TeamOperations.showMembers();
        RequestLatestTeamActivities();
        TeamOperations.Init();
    }

    public override void OnHide()
    {

    }

    public override void OnShow()
    {
        
       
    }

    public void ShowTeamActivities()
    {
        TeamOperations.ClearMessage();
        TeamOperations.showMembers(Profile.getTeamMembers());
        int historyCount = TeamDataManager.GetTeamActivityCount();
        for (int i = historyCount - 1; i >= 0; i--)
        {
            TeamDataManager.TeamActivity activity = TeamDataManager.GetTeamActivity(i);
            TeamOperations.addMessage(activity.member, activity.message);
        }
    }

    public override void OnShowBeforeTransition()
    {
        for (NotificationCount--;  NotificationCount>= 0; NotificationCount--)
        {
            NotificationController.Instance.HideNotification(Utility.App.Team, NotificationCount.ToString());
        }
        NotificationCount = 0;
        TeamOperations.onShow();
        ShowTeamActivities();
    }
 
    public void RequestLatestTeamActivities()
    {
        List<TeamDataManager.TeamDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.TEAM_ACT_GET_LATEST, new string[] { ActivitiesObtainedCount.ToString() }).Cast<TeamDataManager.TeamDataDesc>().ToList();
        TeamDataManager.AcquiredLatestTeamActivity(data);
    }

    public void RequestActivitiesFromIndex(int fromIndex)
    {
        List<TeamDataManager.TeamDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.TEAM_GET_FROM_INDEX, new string[] { fromIndex.ToString(), ActivitiesObtainedCount.ToString() }).Cast<TeamDataManager.TeamDataDesc>().ToList();
        TeamDataManager.AppendPreviousTeamActivity(data);
    }
}
