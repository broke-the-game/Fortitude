using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppExecution : ScriptableObject
{
    [SerializeField]
    public string AppExe_Id;
    [SerializeField]
    public Utility.App ExecutingApp;
    [SerializeField]
    public int Situation_Id = -1;
    [SerializeField]
    public int Cluster_Id = -1;
}
