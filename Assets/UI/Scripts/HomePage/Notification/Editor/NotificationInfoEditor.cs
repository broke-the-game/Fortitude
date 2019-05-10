using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotificationViewInfo))]
public class NotificationInfoEditor : Editor
{
    NotificationViewInfo Target { get { return (NotificationViewInfo)target; } }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Target.UpdateAspectRatio();

    }
}
