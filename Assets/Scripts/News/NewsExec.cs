using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "News Activity", menuName = "Fortitude/News/NewsActivity", order = 1)]
public class NewsExec : AppExecution
{
    public string title;
    public string description;
    public string iconPath;
    public AppCallback appCallback;
}
