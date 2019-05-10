using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataBank;

[CustomEditor(typeof(SettingViewController))]
public class SettingViewControllerEditor : Editor
{
    SettingViewController ViewController => (SettingViewController)target;
    bool hashmapFolded;
    Dictionary<Color, Sprite> hashmap => ViewController.IconHashmap;
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space();
        hashmapFolded = EditorGUILayout.Foldout(hashmapFolded, "Profile Icons", true);
        if (hashmapFolded)
        {
            //red
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Red))
            {
                hashmap.Add(ProfileEntity.ColorPool.Red, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Red, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            Sprite temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Red], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Red] = temp;
            }
            EditorGUILayout.EndHorizontal();

            //purple
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Purple))
            {
                hashmap.Add(ProfileEntity.ColorPool.Purple, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Purple, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Purple], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Purple] = temp;
            }
            EditorGUILayout.EndHorizontal();

            //Green
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Green))
            {
                hashmap.Add(ProfileEntity.ColorPool.Green, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Green, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Green], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Green] = temp;
            }
            EditorGUILayout.EndHorizontal();

            //Blue
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Blue))
            {
                hashmap.Add(ProfileEntity.ColorPool.Blue, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Blue, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Blue], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Blue] = temp;
            }
            EditorGUILayout.EndHorizontal();

            //Yellow
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Yellow))
            {
                hashmap.Add(ProfileEntity.ColorPool.Yellow, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Yellow, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Yellow], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Yellow] = temp;
            }
            EditorGUILayout.EndHorizontal();

            //Orange
            if (!hashmap.ContainsKey(ProfileEntity.ColorPool.Orange))
            {
                hashmap.Add(ProfileEntity.ColorPool.Orange, null);
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            GUI.enabled = false;
            EditorGUILayout.ColorField(ProfileEntity.ColorPool.Orange, GUILayout.MinWidth(100f));
            GUI.enabled = true;
            EditorGUI.BeginChangeCheck();
            temp = EditorGUILayout.ObjectField(hashmap[ProfileEntity.ColorPool.Orange], typeof(Sprite), GUILayout.MinWidth(100f)) as Sprite;
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RegisterUndo(ViewController, "hashmap");
                hashmap[ProfileEntity.ColorPool.Orange] = temp;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
