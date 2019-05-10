using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using AudioManaging;

public class TeamOperations : MonoBehaviour
{
    public static TeamOperations Instance { get; private set; }
    [SerializeField]
    private TMPro.TextMeshProUGUI numOfMembers;
    [SerializeField]
    private TMPro.TextMeshProUGUI members;

    [SerializeField]
    private RectTransform messageContainer;
    [SerializeField]
    private RectTransform messageHolder;

    public UnityAction OnTeamMemberUpdated;

    public Profile Profile { get => Profile.Instance; }

    private void Awake()
    {
        Instance = this;
    }

    public class TeamHistory
    {
        public string sender;
        public string message;
        public float value;

        public TeamHistory(string sender, string message, float value)
        {
            this.sender = sender;
            this.message = message;
            this.value = value;
        }
    }

    List<TeamHistory> teamHistory = new List<TeamHistory>();

    List<string> currentMembers = new List<string>();

    [SerializeField]
    public TeamDataManager TeamDataManager;

    // Start is called before the first frame update
    void Start()
    {
        //updateMembers("Old Friend", 1);
        //updateMembers("Mom", 1);
        //updateMembers("Sister", 1);
        //showMembers();
        //addMessage("Old Friend", "Thank you for supporting me!");
        //StartCoroutine(newUpdate());
    }

    //IEnumerator newUpdate()
    //{
    //    yield return new WaitForSeconds(3);
    //    AddRemoveTeamMembers("Sister", -1);
    //    showMembers();
    //    addMessage("Sister", "I dont want to talk to you ever again!");
    //    yield break;
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateMembers(string member, float value)
    {
        AddRemoveTeamMembers(member, value);
        numOfMembers.text =currentMembers.Count.ToString();
    }

    /// <summary>
    /// Args: string[]{sender, message, value}
    /// </summary>
    /// <param name="args"></param>
    public void addTeamActivity(AppCallbackEvent.EventData args)
    {
        try
        {
            string[] teamInfo = args.GetEventData<string[]>(addTeamActivity);
            if (teamInfo.Length != 3)
            {
                throw new System.Exception("length of the array not 3");
            }
            string sender = teamInfo[0];
            string message = teamInfo[1];
            int value = int.Parse(teamInfo[2]);
            //AddRemoveTeamMembers(sender, value);
            //showMembers();
            //addMessage(sender, message);
            if (TeamAppController.Instance.IsShowBeforeTransition)
            {
                TeamAppController.Instance.ShowTeamActivities();
            }
            TeamHistory act = new TeamHistory(sender, message, value);
            teamHistory.Add(act);
            WriteActivityToDb(act);
            TeamAppController.Instance.RequestLatestTeamActivities();
            OnTeamMemberUpdated();
            if (TeamAppController.Instance.IsShowBeforeTransition)
            {
                TeamAppController.Instance.ShowTeamActivities();
            }
            if (NotificationController.Instance)
            {
                NewTeam_4_NotificationData data = (NewTeam_4_NotificationData)NotificationController.Instance.CreateDataInstance(Utility.App.Team, "NewTeam");
                data.text = sender;
                data.join = value;
                NotificationController.Instance.PushNotification(data, TeamAppController.Instance.NotificationCount.ToString());
                TeamAppController.Instance.NotificationCount++;
                AudioManager.Instance.Play(AudioEnum.Circle_Notifi);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void AddRemoveTeamMembers(string member, float value)
    {
        if (value < 0)
        {
            currentMembers.Remove(member);
        } else if(value > 0)
        {
            currentMembers.Add(member);
        }
    }

    public void showMembers(List<string> currentMems)
    {
        string listToShow = "";
        foreach(var member in currentMems)
        {
            if (listToShow.Length == 0)
            {
                listToShow += member;
            }
            else
            {
                listToShow += ", " + member;
            }
        }
        members.text = listToShow;
        numOfMembers.text = currentMems.Count.ToString();
    }

    List<GameObject> messageList = new List<GameObject>();
    public void addMessage(string sender, string message)
    {
        RectTransform newMessageContainer_ = Instantiate(messageHolder, messageContainer);
        newMessageContainer_.gameObject.SetActive(true);
        RectTransform newMessageContainer = (RectTransform)newMessageContainer_.GetChild(0);
        RectTransform senderAndTime = newMessageContainer.Find("SenderAndTime").GetComponent<RectTransform>();
        RectTransform senderField = senderAndTime.Find("Sender").GetComponent<RectTransform>();
        senderField.GetComponent<TMPro.TextMeshProUGUI>().text = sender;
        //RectTransform timeField = senderAndTime.Find("Time").GetComponent<RectTransform>();
        //timeField.GetComponent<TMPro.TextMeshProUGUI>().text = System.DateTime.Now.ToString("hh:mm tt");
        RectTransform messageField = newMessageContainer.Find("Message").GetComponent<RectTransform>();
        messageField.GetComponent<TMPro.TextMeshProUGUI>().text = message;
        messageList.Add(newMessageContainer_.gameObject);
    }

    public void Init()
    {
        currentMembers = Profile.getTeamMembers();
        for (int i = 0; i < TeamDataManager.GetTeamActivityCount(); i++)
        {
            TeamDataManager.TeamActivity data = TeamDataManager.GetTeamActivity(i);
            AddRemoveTeamMembers(data.member, data.value);
        }
    }

    public void onShow()
    {
        teamHistory.Clear();
    }

    public void WriteActivityToDb(TeamHistory data)
    {
        List<AppDataManager.DataDesc> dataDescList = new List<AppDataManager.DataDesc>();
        dataDescList.Add(new TeamDataManager.TeamDataDesc(-1, data.sender, data.value.ToString(), data.message));
        AppDataManager.SetData(AppDataManager.Protocol.TEAM_WRITE_TO_HISTORY, dataDescList);
    }
    public void ClearMessage()
    {
        for (; messageList.Count>0 ; )
        {
            GameObject go = messageList[messageList.Count - 1];
            messageList.RemoveAt(messageList.Count - 1);
            DestroyImmediate(go);
        }
    }
}
