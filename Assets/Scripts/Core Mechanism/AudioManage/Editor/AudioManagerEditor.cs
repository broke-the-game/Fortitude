using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace AudioManaging
{
    [CustomEditor(typeof(AudioManager))]
    public class AudioManagerEditor : Editor
    {
        AudioManager AudioManager => (AudioManager)target;
        Dictionary<int, AudioClip> HashMap => AudioManager.AudioClipMap;
        bool showPathError = false;
        bool hashmapFolded;
        bool fileWriting = false;
        bool justOnEnabled = false;

        string[] EditableKeys { get => AudioManager.EditableKeys; set => AudioManager.EditableKeys = value; }

        private void OnEnable()
        {
            LoadAudioFiles(AudioManager.AudioPath);
            justOnEnabled = true;

        }

        public override void OnInspectorGUI()
        {
            bool GUIEnabled = true;
            if (fileWriting)
            {
                if (UnityEditor.VersionControl.Provider.activeTask != null || EditorApplication.isUpdating || EditorApplication.isCompiling)
                {

                    EditorGUILayout.Space();
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUILayout.LabelField("Wait to compile");
                    EditorGUILayout.Space();
                    EditorGUILayout.EndHorizontal();
                    GUI.enabled = false;
                    GUIEnabled = false;
                }
                else
                {
                    fileWriting = false;
                    SetHashMap();
                    LoadHashMap();
                    EditorUtility.DisplayDialog("AudioClip Hashmap", "Hashmap has been saved", "OK");
                    GUI.enabled = true;
                }
            }
            MonoScript ms = MonoScript.FromMonoBehaviour(AudioManager);
            string m_ScriptFilePath = AssetDatabase.GetAssetPath(ms);

            string m_ScriptFolder = m_ScriptFilePath.Remove(m_ScriptFilePath.Length - "/AudioManager.cs".Length).Substring("Assets".Length);
            if (!AudioManagerFileWriter.IsFileCheckedOut(m_ScriptFolder))
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("File Read-Only");
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                GUI.enabled = false;
                GUIEnabled = false;
            }
            if (justOnEnabled)
            {
                LoadHashMap();
                justOnEnabled = false;
            }
            if (showPathError)
            {
                EditorGUILayout.LabelField("[ERROR] Not in Project Data Folder");
            }

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Select Audio Files Folder", GUILayout.MinWidth(100f)))
            {
                string absolutepath = EditorUtility.OpenFolderPanel("Audio Files Path", Application.dataPath, "");
                if (absolutepath.StartsWith(Application.dataPath))
                {
                    showPathError = false;
                    AudioManager.SetPath_Editor(absolutepath.Substring(Application.dataPath.Length));
                    LoadAudioFiles(AudioManager.AudioPath);
                    LoadHashMap();
                }
                else
                {
                    showPathError = true;
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();


            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reload Audio Files", GUILayout.MinWidth(80f)))
            {
                LoadAudioFiles(AudioManager.AudioPath);
                LoadHashMap();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Audio Path", GUILayout.MaxWidth(100f));
            GUI.enabled = false;
            EditorGUILayout.TextField(AudioManager.AudioPath);
            GUI.enabled = GUIEnabled;
            EditorGUILayout.EndHorizontal();


            hashmapFolded = EditorGUILayout.Foldout(hashmapFolded, "Audio Enumeration Define", true);
            if (hashmapFolded)
            {
                AudioClip audioClip;
                for (int i = 0; i < AudioManager.AudioClipList.Count; i++)
                {
                    audioClip = AudioManager.AudioClipList[i];
                    EditorUtility.SetDirty(audioClip);
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.Space();
                    EditorGUI.BeginChangeCheck();
                    string temp = EditorGUILayout.TextField(EditableKeys[i], GUILayout.MinWidth(50f));
                    if (EditorGUI.EndChangeCheck())
                    {
                        Undo.RegisterUndo(AudioManager, "Editable Enum");
                        EditableKeys[i] = temp;
                    }
                    GUI.enabled = false;
                    EditorGUILayout.ObjectField(audioClip, typeof(AudioClip), GUILayout.MinWidth(100f));
                    GUI.enabled = GUIEnabled;
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Save To HashMap", GUILayout.MinWidth(80f)))
                {
                    List<string> validKeys = new List<string>();
                    foreach (var key in EditableKeys)
                    {
                        if (key != "")
                        {
                            validKeys.Add(key);
                        }
                    }
                    AudioManagerFileWriter.WriteEnumFile(m_ScriptFolder, validKeys.ToArray());
                    AssetDatabase.Refresh();

                    fileWriting = true;
                }
                EditorGUILayout.Space();
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space();
            }
        }

        public void LoadAudioFiles(string path)
        {
            string assetsPath = "Assets" + path;
            string[] audioGuids = AssetDatabase.FindAssets("t:AudioClip", new[] { assetsPath });
            string filePath;
            AudioClip tempClip;
            Utility.App app;
            AudioManager.AudioClipList.Clear();
            foreach (string guid in audioGuids)
            {
                filePath = AssetDatabase.GUIDToAssetPath(guid);
                //Debug.Log(filePath);

                tempClip = AssetDatabase.LoadAssetAtPath<AudioClip>(filePath);
                AudioManager.AudioClipList.Add(tempClip);
            }
        }

        public void LoadHashMap()
        {
            EditableKeys = new string[AudioManager.AudioClipList.Count];
            AudioEnum tempKey;
            AudioClip audioClip;
            for (int i = 0; i < AudioManager.AudioClipList.Count; i++)
            {
                audioClip = AudioManager.AudioClipList[i];
                if (HashMap.ContainsValue(audioClip))
                {
                    foreach (var key in HashMap.Keys)
                    {
                        if (HashMap[key] == audioClip)
                        {
                            tempKey = (AudioEnum)key;
                            EditableKeys[i] = System.Enum.GetName(typeof(AudioEnum), tempKey);
                            break;
                        }
                    }
                }
                else
                {
                    EditableKeys[i] = "";
                }
            }
        }

        public void SetHashMap()
        {
            AudioClip audioClip;
            AudioEnum[] audioEnums = System.Enum.GetValues(typeof(AudioEnum)) as AudioEnum[]; ;
            string enumName;
            HashMap.Clear();
            for (int i = 0; i < AudioManager.AudioClipList.Count; i++)
            {
                audioClip = AudioManager.AudioClipList[i];
                foreach (var audioEnum in audioEnums)
                {
                    if (System.Enum.GetName(typeof(AudioEnum), audioEnum) == EditableKeys[i])
                    {
                        HashMap.Add((int)audioEnum, audioClip);
                        break;
                    }
                }
            }
        }
    }
}
