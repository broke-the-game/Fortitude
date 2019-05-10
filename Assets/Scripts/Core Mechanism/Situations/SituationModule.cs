using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SituationModule : MonoBehaviour
{
    private static SituationModule _instance;
    public static SituationModule Instance { get { return _instance; } }

    [SerializeField]
    private float timeBetweenSituations = 5;
    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        //StartCoroutine(startSituations());
    }

    //IEnumerator startSituations()
    //{
    //    yield return new WaitForSeconds(timeBetweenSituations);

    //    //Situation newSituation = getNextSituationToExecute();
    //    Situation newSituation = new Situation("2" ,Utility.App.Mail);
    //    AppExecution appToExecute = getSituationContent(newSituation);
    //    // execute the appToExecute
    //    AppExecutionModule.Instance.Execute(appToExecute);
    //    yield break;

    //}

    private List<Situation> situations = new List<Situation>();
    
    public Situation getNextSituationToExecute()
    {
        List<Situation> newList = new List<Situation>();
        for (int i = 0; i < situations.Count; i++)
        {
            if (situations[i].getDependentSituations().Count == 0)
            {
                newList.Add(situations[i]);
            }
        }
        int situationIndex = Random.Range(0, newList.Count - 1);
        Situation selectedSituation = newList[situationIndex];
        situations.Remove(selectedSituation);
        return selectedSituation;
    }

    //public AppExecution getSituationContent(Situation situation)
    //{
    //    //Multithread?
    //    //Debug.Log(((TextMsgObj)(((TextMsgObj)((TextMsgObj)appToExecute).nextText[0].AppExecution)).nextText[0].AppExecution).speaker);
    //    var FetchedList = DbBehaviourModule.Instance.FetchSituation(situation);
    //    return FetchedList;
    //}

    /// <summary>
    /// Args: string[] {int situationId, int clusterId}
    /// </summary>
    /// <param name="args"></param>
    public void SituationOnFinish(AppCallbackEvent.EventData args)
    {
        string[] result = args.GetEventData<string[]>(SituationOnFinish);
        //DBMark as Done
        
        int sitId = int.Parse(result[0]);
        int clusterId = int.Parse(result[1]);
        DbBehaviourModule.Instance.MarkSitFinish(sitId);
        ClusterModule.Instance.FinishedSituation(sitId, clusterId);
    }
}
