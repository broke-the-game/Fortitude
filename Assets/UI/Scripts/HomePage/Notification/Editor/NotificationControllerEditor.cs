using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(NotificationController))]
public class NotificationControllerEditor : Editor
{
    NotificationController Target { get { return (NotificationController)target; } }

    bool showPathError = false;

    private void OnEnable()
    {
        Target.LoadViewPrefabs();
    }

    public override void OnInspectorGUI()
    {
        if (showPathError)
        {
            EditorGUILayout.LabelField("[ERROR] Not in Project Data Folder");
        }

        if (GUILayout.Button("Select Resource Folder"))
        {
            string absolutepath = EditorUtility.OpenFolderPanel("Notification Resources Folder", Application.dataPath, "");
            if (absolutepath.StartsWith(Application.dataPath))
            {
                showPathError = false;
                Target.PrefabPath = absolutepath.Substring(Application.dataPath.Length);
                Target.LoadViewPrefabs();
            }
            else {
                showPathError = true;
            }
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        DrawDefaultInspector();

        //string prefabPath = EditorUtility.OpenFolderPanel("Notification Resources Folder", );
    }
}
