using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClusterModule : MonoBehaviour, Initializable
{
    public static ClusterModule Instance { get; private set; }

    public SituationModule SituationModule => SituationModule.Instance;

    [SerializeField, Range(0, 10)]
    public int maxInTimeRange;

    private void Awake()
    {
        Instance = this;
    }

    List<int> m_clusterToExecute = new List<int>();
    List<int> m_situationToExecute = new List<int>();
    List<int> m_situationInProgress = new List<int>();
    public void executeNextSituations()
    {
        // Get list of situations from cluster module and start execution
        //StartCoroutine(ExecuteCluster(getNextCluster()));
    }

    //IEnumerator ExecuteCluster(List<Situation> situations)
    //{
      
    //    bool executedSocialImpact = false;
    //    if (situations != null && situations.Count > 0)
    //    {
    //        while (situations.Count > 0)
    //        {
    //            int numOfSeconds = Mathf.FloorToInt(Random.Range(0, maxInTimeRange));
    //            yield return new WaitForSeconds(numOfSeconds);
    //            AppExecution appExecution;

    //            // check if social impact situation is executed. If not, execute that.
    //            if (!executedSocialImpact)
    //            {
    //                appExecution = SituationModule.getSituationContent(situations[0]);
    //                AppExecutionModule.Instance.Execute(appExecution);
    //                situations.RemoveAt(0);
    //                executedSocialImpact = true;
    //                continue;
    //            }

    //            // Randomly select a situation and execute.
    //            int index = Mathf.FloorToInt(Random.Range(0, situations.Count - 1));
    //            appExecution = SituationModule.getSituationContent(situations[index]);
    //            AppExecutionModule.Instance.Execute(appExecution);
    //            situations.RemoveAt(index);
    //        }
    //        executeNextSituations();
    //    }
    //    yield break;
    //}

    public List<Situation> getNextCluster()
    {
        // Get cluster from db and return
        return null;
    }
    public void FinishedSituation(int situationId, int clusterId)
    {
        m_situationToExecute = DbBehaviourModule.Instance.RequestSituationFromCluster(clusterId);
        m_situationInProgress.Remove(situationId);
        if (m_situationInProgress.Count > 0)
        {
            return;
        }
        if (m_situationToExecute == null || m_situationToExecute.Count < 1)
        {
            FinishedCluster(clusterId);
        }
        else
        {
            StartCoroutine(assignSituation(clusterId));
        }
    }
    public void FinishedCluster(int clusterId)
    {
        m_clusterToExecute.Remove(clusterId);
        DbBehaviourModule.Instance.MarkClusterFinish(clusterId);
        if (m_clusterToExecute.Count < 1)
        {
            EndingManager.Instance.EndPlaythrough();
        }
        else
        {
            m_situationToExecute = DbBehaviourModule.Instance.RequestSituationFromCluster(m_clusterToExecute[0]);
            if (m_situationToExecute != null && m_situationToExecute.Count < 1)
            {
                FinishedCluster(m_clusterToExecute[0]);
                return;
            }
            //Execute Social Impact
            AppExecution socialImpact = DbBehaviourModule.Instance.FetchSituation(m_clusterToExecute[0], m_situationToExecute[0]);
            AppExecutionModule.Instance.Execute(socialImpact);
            m_situationInProgress.Add(socialImpact.Situation_Id);
        }
    }

    private IEnumerator assignSituation(int clusterId)
    {
        yield return new WaitForSeconds(maxInTimeRange);
        int situationId = m_situationToExecute[Mathf.FloorToInt(Random.Range(0f, m_situationToExecute.Count - Mathf.Epsilon))];
        AppExecution appExe = DbBehaviourModule.Instance.FetchSituation(clusterId, situationId);
        AppExecutionModule.Instance.Execute(appExe);
        m_situationInProgress.Add(situationId);
    }

    public void Init()
    {
        StartCoroutine(wait4SceneInit());
    }

    IEnumerator wait4SceneInit()
    {
        while (!AppSceneLoader.Instance.ScenesLoaded())
            yield return null;
        var unfinishedExes = StateLoadingModule.Instance.GetUnFinishedAppExeIds();
        if (unfinishedExes.Length < 1)
        {
            if (m_clusterToExecute.Count > 0)
            {
                m_situationToExecute = DbBehaviourModule.Instance.RequestSituationFromCluster(m_clusterToExecute[0]);
                if (m_situationToExecute != null && m_situationToExecute.Count > 0)
                {
                    AppExecution socialImpact = DbBehaviourModule.Instance.FetchSituation(m_clusterToExecute[0], m_situationToExecute[0]);
                    AppExecutionModule.Instance.Execute(socialImpact);
                    m_situationInProgress.Add(socialImpact.Situation_Id);
                }
                else
                {
                    FinishedCluster(m_clusterToExecute[0]);
                }
            }
            else
            {
                EndingManager.Instance.EndPlaythrough();
            }
            yield break;
        }
        else
        {
            m_situationToExecute = DbBehaviourModule.Instance.RequestSituationFromCluster(unfinishedExes[0].ClusterId);
        }
        for (int i = 0; i < unfinishedExes.Length; i++)
        {
            AppExecution temp = DbBehaviourModule.Instance.FetchSituation(unfinishedExes[i].ClusterId, unfinishedExes[i].SitId, unfinishedExes[i].AppExeId);
            AppExecutionModule.Instance.Execute(temp);
            m_situationInProgress.Add(temp.Situation_Id);
        }
    }

    public void SetUnfinishedClusterIds(List<int> clusterIds)
    {
        if (clusterIds != null)
        {
            m_clusterToExecute = clusterIds;
        }
    }
}
