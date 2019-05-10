using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class AppInfoLoading : MonoBehaviour
{
    [SerializeField]
    AppInfo AppInfo;

    private void Start()
    {
        AppInfo.LoadAppInfo();
    }
}
