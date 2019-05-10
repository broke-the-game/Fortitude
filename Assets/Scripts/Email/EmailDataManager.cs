using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EmailDataManager : MonoBehaviour
{
    public class EmailGroupInfo {
        public class EmailInfo {
            public string content { get; private set; }
            public bool sentByPlayer { get; private set; }
            public int indexInDatabase { get; private set; }
            public EmailInfo(string content, bool sentByPlayer, int indexInDatabase)
            {
                this.content = content;
                this.sentByPlayer = sentByPlayer;
                this.indexInDatabase = indexInDatabase;
            }
            public class Comparation : IComparer<EmailInfo>
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
                public int Compare(EmailInfo x, EmailInfo y)
                {
                    return x.indexInDatabase.CompareTo(y.indexInDatabase);
                }
            }
        }
        private List<EmailInfo> EmailList = new List<EmailInfo>();
        public string Sender { get; private set; }
        public string Subject { get; private set; }
        public int SituationId { get; private set; }
        public EmailGroupInfo(string sender, string subject, int situationId)
        {
            Sender = sender;
            Subject = subject;
            SituationId = situationId;
        }
        public void AddEmail(string content, bool sentByPlayer, int indexInDatabase)
        {
            EmailList.Add(new EmailInfo(content, sentByPlayer, indexInDatabase));
        }
        public int GetEmailCount()
        {
            return EmailList.Count;
        }
        public EmailInfo GetEmailInfo(int index)
        {
            if (index > EmailList.Count - 1)
                return null;
            return EmailList[index];
        }
        public void Sort()
        {
            EmailList.Sort(EmailInfo.Comparation.Instance);
        }
    }

    private Dictionary<int, EmailGroupInfo> EmailGroupList = new Dictionary<int, EmailGroupInfo>();

    public class EmailDataDesc : AppDataManager.DataDesc
    {
        public string sender { get; }
        public bool sentByPlayer { get; }
        public string subject { get; }
        public string content { get; }
        public int situationId { get; }
        public EmailDataDesc(int index, string sender, bool sentByPlayer, string subject, string content, int situationId)
            :base(index)
        {
            this.sender = sender;
            this.sentByPlayer = sentByPlayer;
            this.subject = subject;
            this.content = content;
            this.situationId = situationId;
        }
    }

    public void AcquiredLatestEmail(List<EmailDataDesc> data)
    {
        if (data != null && data.Count > 0)
        {
            EmailGroupList.Clear();
            int key;
            EmailGroupInfo emailGroupInfo;
            for (int i = 0; i < data.Count; i++)
            {
                key = data[i].situationId;
                if (EmailGroupList.TryGetValue(key, out emailGroupInfo))
                {
                    emailGroupInfo.AddEmail(data[i].content, data[i].sentByPlayer, data[i].id);
                }
                else
                {
                    emailGroupInfo = new EmailGroupInfo(data[i].sender,data[i].subject, data[i].situationId);
                    EmailGroupList.Add(emailGroupInfo.SituationId, emailGroupInfo);
                    emailGroupInfo.AddEmail(data[i].content, data[i].sentByPlayer, data[i].id);
                }
            }

            foreach (var groupInfo in EmailGroupList)
            {
                groupInfo.Value.Sort();
            }
        }
    }

    public int[] GetKeyList()
    {
        return EmailGroupList.Keys.ToArray();
    }

    public EmailGroupInfo GetEmailGroup(int situationId)
    {
        EmailGroupInfo emailGroup;
        if (EmailGroupList.TryGetValue(situationId, out emailGroup))
        {
            return emailGroup;
        }
        return null;
    }
}
