using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TeamDataManager : MonoBehaviour
{
    List<TeamActivity> TeamData = new List<TeamActivity>();

    public class TeamActivity
    {
        public string member { get; }
        public float value { get; }
        public string message { get; }
        public int indexInDatabase { get; }
        public TeamActivity(string member, float value, string message, int index)
        {
            this.member = member;
            this.value = value;
            this.message = message;
            this.indexInDatabase = index;
        }

        public class Comparation : IComparer<TeamActivity>
        {
            private Comparation() { }

            private static Comparation m_instance;
            public static Comparation Instance
            {
                get
                {
                    if (m_instance == null)
                        m_instance = new Comparation();
                    return m_instance;
                }
            }

            public int Compare(TeamActivity x, TeamActivity y)
            {
                return x.indexInDatabase.CompareTo(y.indexInDatabase);
            }
        }
    }

    public class TeamDataDesc: AppDataManager.DataDesc
    {
        public string member;
        public string value;
        public string message;
        public TeamDataDesc(int index, string member, string value, string message)
            : base(index)
        {
            this.member = member;
            this.value = value;
            this.message = message;
        }
    }

    public int GetTeamActivityCount() => TeamData.Count;

    public TeamActivity GetTeamActivity(int index)
    {
        if (TeamData.Count > index)
        {
            return TeamData[index];
        }
        return null;
    }

    public void AcquiredLatestTeamActivity(List<TeamDataDesc> dataDesc)
    {
        TeamData.Clear();
        for (int i = 0; i < dataDesc.Count; i++)
        {
            TeamData.Add(new TeamActivity(dataDesc[i].member, float.Parse(dataDesc[i].value), dataDesc[i].message, dataDesc[i].id));
        }

        TeamData.Sort(TeamActivity.Comparation.Instance);
    }

    public void AppendPreviousTeamActivity(List<TeamDataDesc> dataDesc)
    {
        for (int i = 0; i < dataDesc.Count; i++)
        {
            if (!TeamData.Any(d => d.indexInDatabase == dataDesc[i].id))
            {
                TeamData.Add(new TeamActivity(dataDesc[i].member, float.Parse(dataDesc[i].value), dataDesc[i].message, dataDesc[i].id));
            }
        }
        TeamData.Sort(TeamActivity.Comparation.Instance);
    }
}
