using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailExecutionObj : AppExecution
{
    public string WithWho;
    public bool IsPlayerTalking;
    public string Subject;
    public string Content;
    public string[] NextEmail;
    public string[] OptionDescription;
    public AppCallback[] NextEmailCallback;
}
