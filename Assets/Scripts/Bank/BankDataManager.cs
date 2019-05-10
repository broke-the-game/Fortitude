using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BankDataManager : MonoBehaviour
{
    List<BankActivity> BankingData = new List<BankActivity>();

    public class BankActivity
    {
        public float amount { get; }
        public string activitySummary { get; }
        public int indexInDatabase { get; }
        public float balance { get; }
        public BankActivity (float amount, string activitySummary, float balance, int index)
        {
            this.amount = amount;
            this.activitySummary = activitySummary;
            this.balance = balance;
            this.indexInDatabase = index;
        }

        public class Comparation : IComparer<BankActivity>
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

            public int Compare(BankActivity x, BankActivity y)
            {
                return x.indexInDatabase.CompareTo(y.indexInDatabase);
            }
        }
    }

    public class BankDataDesc: AppDataManager.DataDesc
    {
        public string amount;
        public string activitySummary;
        public string balance;
        public BankDataDesc (int index, string amount, string activitySummary, string balance)
            : base(index)
        {
            this.amount = amount;
            this.activitySummary = activitySummary;
            this.balance = balance;
        }
    }

    public int GetBankActivityCount() => BankingData.Count;

    public BankActivity GetBankActivity(int index)
    {
        if (BankingData.Count > index)
        {
            return BankingData[index];
        }
        return null;
    }

    public void AcquiredLatestBankActivity (List<BankDataDesc> dataDesc)
    {
        BankingData.Clear();
        for (int i = 0; i < dataDesc.Count; i++)
        {
            BankingData.Add(new BankActivity(float.Parse(dataDesc[i].amount), dataDesc[i].activitySummary, float.Parse(dataDesc[i].balance), dataDesc[i].id));
        }

        BankingData.Sort(BankActivity.Comparation.Instance);
    }

    public void AppendPreviousBankActivity (List<BankDataDesc> dataDesc)
    {
        for (int i = 0; i < dataDesc.Count; i++)
        {
            if (!BankingData.Any(d => d.indexInDatabase == dataDesc[i].id))
            {
                BankingData.Add(new BankActivity(float.Parse(dataDesc[i].amount), dataDesc[i].activitySummary, float.Parse(dataDesc[i].balance), dataDesc[i].id));
            }
        }
        BankingData.Sort(BankActivity.Comparation.Instance);
    }
}
