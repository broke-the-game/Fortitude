using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class StateLoadingModule : MonoBehaviour, Initializable
{
    [SerializeField]
    bool m_startFromBegin;
    #region Singleton
    private static StateLoadingModule m_instance;
    public static StateLoadingModule Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion
    [System.Serializable]
    public class StateLoadingObject {
        [System.Serializable]
        public struct SerializedAppExe
        {
            [SerializeField]
            public string AppExeId;
            [SerializeField]
            public int SitId;
            [SerializeField]
            public int ClusterId;
            public SerializedAppExe(string exeId, int sitId, int clusterId)
            {
                AppExeId = exeId;
                SitId = sitId;
                ClusterId = clusterId;
            }
        }
        public List<SerializedAppExe> AppExeIdList = new List<SerializedAppExe>();
        public void AddAppExe(string appExeId, int sitId, int clusterId)
        {
            for (int i = 0; i < AppExeIdList.Count; i++)
            {
                if (AppExeIdList[i].AppExeId == appExeId)
                    return;
            }
            AppExeIdList.Add(new SerializedAppExe(appExeId, sitId, clusterId));
        }
        public void RemoveAppExe(string appExeId)
        {
            for (int i = 0; i < AppExeIdList.Count; i++)
            {
                if (AppExeIdList[i].AppExeId == appExeId)
                {
                    AppExeIdList.RemoveAt(i);
                }
            }
        }
    }

    private bool m_stateLoaded;

    public bool StateLoaded { get => m_stateLoaded; }
    public bool StartFromBegin { get => m_startFromBegin;}

    StateLoadingObject serialzedObject;

    public void Init()
    {
        loadData();
        m_stateLoaded = true;
    }

    public StateLoadingObject.SerializedAppExe[] GetUnFinishedAppExeIds()
    {
        if (serialzedObject != null && serialzedObject.AppExeIdList.Count > 0)
        {
            return serialzedObject.AppExeIdList.ToArray();
        }
        return new StateLoadingObject.SerializedAppExe[0];
    }

    public void RegisterToCurrentState(AppExecution appExe)
    {
        serialzedObject.AddAppExe(appExe.AppExe_Id, appExe.Situation_Id, appExe.Cluster_Id);
        saveData();
        //appExeList.Sort();
    }

    ///<summary>
    /// Arg: AppExecution appExe
    ///</summary>
    public void FinishCurrentState(AppCallbackEvent.EventData args)
    {
        AppExecution appExe = args.GetEventData<AppExecution>(FinishCurrentState);
        serialzedObject.RemoveAppExe(appExe.AppExe_Id);
        saveData();
        //int index = appExeList.BinarySearch(appExe.AppExe_Id);
        //appExeList.RemoveAt(index);
    }

    private void saveData()
    {
        string FullFilePath = Application.persistentDataPath + "/StateLoading.json";
        StreamWriter writer = new StreamWriter(FullFilePath, false);
        writer.Write(JsonUtility.ToJson(serialzedObject));
        writer.Close();
    }
    private void loadData()
    {
        string FullFilePath = Application.persistentDataPath + "/StateLoading.json";
        if (File.Exists(FullFilePath) == false)
        {
            //File.Create(FullFilePath);
            serialzedObject = new StateLoadingObject();
            return;
        }
        else if (StartFromBegin)
        {
            File.Delete(FullFilePath);
            serialzedObject = new StateLoadingObject();
            return;
        }
        StreamReader reader = new StreamReader(FullFilePath);
        serialzedObject = JsonUtility.FromJson<StateLoadingObject>(reader.ReadToEnd());
        reader.Close();
    }

}
