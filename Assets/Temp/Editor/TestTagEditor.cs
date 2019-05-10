using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TestTag))]
public class TestTagEditor : Editor
{
    TestTag TestTag => (TestTag)target;

    GUIStyle style;

    GUIStyle ButtonStyle;

    private void OnEnable()
    {

    }

    public override void OnInspectorGUI()
    {
        style = new GUIStyle(GUI.skin.textArea);
        ButtonStyle = new GUIStyle(GUI.skin.button);
        style.fontSize = 20;
        ButtonStyle.fontSize = 20;

        EditorGUI.BeginChangeCheck();

        string temp =  EditorGUILayout.TextArea(TestTag.inputText, style, new GUILayoutOption[] { GUILayout.Height(500f) });
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterUndo(TestTag, "input Field");
            TestTag.inputText = temp;
            TestTag.SetText();

        }

        //EditorGUILayout.Space();
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.Space();

        //if (GUILayout.Button("SetText", ButtonStyle, new GUILayoutOption[] { GUILayout.Height(40f), GUILayout.Width(120f) }))
        //{
        //    TestTag.SetText();
        //}
        //EditorGUILayout.Space();
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.Space();
    }
}
