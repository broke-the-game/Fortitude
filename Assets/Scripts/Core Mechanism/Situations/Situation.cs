using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Situation : MonoBehaviour
{
    private string id = "";
    public int ClusterId = -1;
    private Utility.App app;
    private List<int> dependentSituations = new List<int>();

    public Situation(string id, Utility.App app)
    {
        this.id = id;
        this.app = app;
    }

    public void addDependentSituations(int sitId)
    {
        dependentSituations.Add(sitId);
    }

    public List<int> getDependentSituations()
    {
        return dependentSituations;
    }
    public string getID()
    {
        return this.id;
    }
    public Utility.App getApp()
    {
        return this.app;
    }
}
