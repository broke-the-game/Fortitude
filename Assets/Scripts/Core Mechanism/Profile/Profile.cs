using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

public class Profile : MonoBehaviour, Initializable
{
    public static Profile Instance { get; private set; }
    public int ProfileId { get; private set; }
    int children = 0;
    string occupation = null;
    string education = null;
    string residence = null;
    string challenges = null;
    public float defaultBankAmt { get; private set; }
    float currentBankAmt;
    string[] defaultTeamMembers;
    //List<string> currentTeamMembers;

    public BankDataManager BankDataManager => BankAppController.Instance.BankDataManager;

    public TeamDataManager TeamDataManager => TeamAppController.Instance.TeamDataManager;

    //Profile(int children, string occupation, string education, string residence, string challenges, float initialAmount)
    //{
    //    this.children = children;
    //    this.occupation = occupation;
    //    this.education = education;
    //    this.residence = residence;
    //    this.challenges = challenges;
    //    amountInBank = initialAmount;
    //    teamMembers = new List<string>();
    //}

    void Awake()
    {
        Instance = this;

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void setCurrentBankAmt(float val) => currentBankAmt = val;

    public float getAmountInBank()
    {

        float balance = defaultBankAmt;
        for (int i = 0; i < BankDataManager.GetBankActivityCount(); i++)
        {
            balance += BankDataManager.GetBankActivity(i).amount;
        }
        return balance;
    }

    public int getNumberOfMembers()
    {
        return getTeamMembers().Count;
    }

    public List<string> getTeamMembers()
    {
        List<string> members = defaultTeamMembers.ToList();
        for (int i = 0; i < TeamDataManager.GetTeamActivityCount(); i++)
        {
            TeamDataManager.TeamActivity data = TeamDataManager.GetTeamActivity(i);
            if (data.value < 0)
            {
                members.Remove(data.member);
            }
            else if (data.value > 0)
            {
                members.Add(data.member);
            }
        }
        return members;
    }

    public void Init()
    {
        // fetch default bank amount, team members and cluster ids from db
        ProfileId = PlayerPrefs.GetInt("Profile");
        defaultBankAmt = DbBehaviourModule.Instance.RequestInitialBankBalance(ProfileId);
        defaultTeamMembers = DbBehaviourModule.Instance.RequestTeamMember(ProfileId);
        // set cluster id list in cluster module    
        List<int> clusterIDs = DbBehaviourModule.Instance.RequestClusterList(ProfileId);
        ClusterModule.Instance.SetUnfinishedClusterIds(clusterIDs);
    }
}
