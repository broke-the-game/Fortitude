using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotificationEditingBehavior))]
public class NotificationEditor : Editor
{


    NotificationEditingBehavior Target { get { return (NotificationEditingBehavior)target; } }
    public NotificationController NotificationController { get { return Target.NotificationController; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
    }
}
