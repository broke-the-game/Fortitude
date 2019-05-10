using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingAppController : AppController
{
    public static SettingAppController Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    SettingViewController ViewController;
    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        throw new System.NotImplementedException();
    }

    public override void Init()
    {
        ViewController.OnInit();
    }

    public override void OnHide()
    {
    }

    public override void OnShow()
    {

    }

    public void EndGame(bool goodEnding)
    {
        ViewController.GameEnd(goodEnding);
    }

    public override void OnShowBeforeTransition()
    {
        ViewController.OnShowBeforeTransition();
    }
    public void OpenSummary()
    {
        ViewController.NeedToOpenGameLog = true;
    }
}
