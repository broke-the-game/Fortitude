using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NotificationEditorWindow : EditorWindow
{

    Utility.App m_selectedApp;
    int m_selectedTemplate;
    string[] m_templateOption = new string[0];
    GameObject NotificationViewObject;
    NotificationData m_notificationData;
    bool IsCreating = false;
    string m_templateID2Create;
    bool EditingBehaviorFound = false;

    NotificationView NotificationView { get { if (!NotificationViewObject) return null; return NotificationViewObject.GetComponent<NotificationView>(); } }

    public NotificationController NotificationController { get { return NotificationEditingBehavior.Instance.NotificationController; } }

    [MenuItem("Window/Fortitude/Notification Editor")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        NotificationEditorWindow window = (NotificationEditorWindow)GetWindow(typeof(NotificationEditorWindow));
        window.Show();
        window.UpdateInfo();
    }

    private void OnGUI()
    {
        //Debug.Log(NotificationController.PrefabManager.GetPrefabList(Utility.App.Text).Length);
        //Debug.Log(m_templateOption[0]);
        if (!NotificationEditingBehavior.Instance)
        {
            EditorGUILayout.LabelField("Notification Editor Not Found", EditorStyles.boldLabel);
            EditingBehaviorFound = false;
            return;
        }
        if (!EditingBehaviorFound)
        {
            EditingBehaviorFound = true;
            UpdateInfo();
        }

        if (m_fileWritten || m_fileDeleting)
        {
            EditorGUILayout.LabelField("Wait to compile");
            return;
        }

        GUIStyle onRight = new GUIStyle(GUI.skin.button);
        onRight.alignment = TextAnchor.MiddleRight;
        

        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("App: ", GUILayout.Width(150));
        Utility.App tempApp = (Utility.App)EditorGUILayout.EnumPopup(m_selectedApp);
        if (tempApp != m_selectedApp)
        {
            m_selectedApp = tempApp;
            m_selectedTemplate = 0;
            UpdateInfo();
        }
        EditorGUILayout.EndHorizontal();

        if (!IsCreating)
        {
            if (m_templateOption.Length == 0)
            {
                m_templateOption = new[] { "" };
            }
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Template: ", GUILayout.Width(150));
            int tempTemplate = EditorGUILayout.Popup(m_selectedTemplate, m_templateOption);
            if (tempTemplate != m_selectedTemplate)
            {
                m_selectedTemplate = tempTemplate;
                UpdateInfo();
            }
            EditorGUILayout.EndHorizontal();
        }

        if (!IsCreating)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Reload Notification View", GUILayout.Width(150)))
            {
                UpdateInfo();
            }
            EditorGUILayout.Space();

            EditorGUILayout.EndHorizontal();
        }

        if (!IsCreating)
        {
            if (NotificationViewObject)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("    Notification View Editor    ",GUILayout.Width(150));
                Editor viewEditor = Editor.CreateEditor(NotificationView);
                viewEditor.DrawDefaultInspector();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Height", GUILayout.Width(150));
                EditorGUI.BeginChangeCheck();
                NotificationView.Height = EditorGUILayout.Slider(NotificationView.Height, 0f, 2f);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RegisterCompleteObjectUndo(NotificationView, "Change Height");
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                if (GUILayout.Button("Customizable Panel", GUILayout.Width(210)))
                {
                    Selection.activeGameObject = NotificationView.CustomizablePanel.gameObject;
                }
                EditorGUILayout.Space();

                EditorGUILayout.EndHorizontal();

            }
        }

        if (!IsCreating)
        {
            if (NotificationViewObject)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                if (GUILayout.Button("Save",GUILayout.Width(60)))
                {
                    PrefabUtility.ApplyPrefabInstance(NotificationViewObject,InteractionMode.UserAction);
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Delete", GUILayout.Width(60)))
                {
                    if (EditorUtility.DisplayDialog("Delete Notification Template", "Are you sure you want to delete template: " + m_templateOption[m_selectedTemplate], "Delete", "Cancel"))
                    {
                        deleteFiles();
                    }
                }
                EditorGUILayout.Space();
                EditorGUILayout.Space();

                EditorGUILayout.EndHorizontal();
            }
        }

        if (!IsCreating)
        {
            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();

            if (GUILayout.Button("Create new Template",GUILayout.Width(150)))
            {
                IsCreating = true;
                UpdateInfo();
                m_templateID2Create = "";
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }

        if (!IsCreating)
        {
            if (m_notificationData)
            {
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("    Test View with Notification Data");
                Editor dataEditor = Editor.CreateEditor(m_notificationData);
                dataEditor.DrawDefaultInspector();
            }
        }

        if (IsCreating)
        {
            bool containedError = false;
            bool emptyError = false;
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Template ID : ",GUILayout.Width(100));
            m_templateID2Create = EditorGUILayout.TextField(m_templateID2Create, GUILayout.Width(100));
            EditorGUILayout.Space();
            if (GUILayout.Button("Create", GUILayout.Width(70)))
            {
                if (m_templateID2Create == "")
                {
                    emptyError = true;
                }
                else if (NotificationEditingBehavior.Instance.NotificationController.PrefabManager.IsContainPrefab(m_selectedApp, m_templateID2Create))
                {
                    containedError = true;
                }
                createTemplate();
                containedError = false;
                emptyError = false;
            }
            if (GUILayout.Button("Cancel", GUILayout.Width(70)))
            {
                IsCreating = false;
                UpdateInfo();
            }
            EditorGUILayout.EndHorizontal();
            if (containedError)
            {
                EditorGUILayout.LabelField("[ERROR] Template ID already EXISTS");
            }
            if (emptyError)
            {
                EditorGUILayout.LabelField("[ERROR] Template ID cannot be EMPTY");
            }
        }
        if (!IsCreating)
        {
            if (m_notificationData)
            {
                EditorGUILayout.Space();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.Space();

                if (GUILayout.Button("Reload Data"))
                {
                    ReloadData();
                }
                EditorGUILayout.Space();

                EditorGUILayout.EndHorizontal();
            }
        }
        //EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.Space();
        //if (GUILayout.Button("Refresh", GUILayout.Width(70)))
        //{
        //    UpdateInfo();
        //}
        //EditorGUILayout.Space();
        //EditorGUILayout.EndHorizontal();
    }

    void ReloadData()
    {
        try
        {
            NotificationView.ResolveNotificationData(m_notificationData);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.GetType().Name + " Found: " + e.Message);
        }
        NotificationView.gameObject.SetActive(false);
        NotificationView.gameObject.SetActive(true);
    }

    void UpdateInfo()
    {
        if (!NotificationEditingBehavior.Instance)
        {
            return;
        }
        NotificationController.LoadViewPrefabs();
        //Debug.Log(NotificationController.PrefabManager.GetPrefabList(Utility.App.Text).Length);
        if (!NotificationViewObject)
        {
            NotificationEditingBehavior.Instance.ClearNotificationSpace();
        }
        if (!IsCreating)
        {
            if (m_selectedApp != Utility.App.COUNT)
            {
                m_templateOption = NotificationEditingBehavior.Instance.NotificationController.PrefabManager.GetTemplateOption(m_selectedApp);
            }
            else
            {
                m_templateOption = new string[0];
            }
            if (m_templateOption.Length == 0)
            {
                m_templateOption = new[] { "" };
                if (NotificationViewObject)
                {
                    GameObject.DestroyImmediate(NotificationViewObject);
                    NotificationViewObject = null;
                    m_notificationData = null;
                }
            }
            else
            {
                if (NotificationViewObject)
                {
                    GameObject.DestroyImmediate(NotificationViewObject);
                    NotificationViewObject = null;
                    m_notificationData = null;
                }
                NotificationView notificationView = NotificationView.CreateNotificationView(m_templateOption[m_selectedTemplate], new NotificationController.NotificationId(m_selectedApp, "Editor"));
                NotificationViewObject = notificationView.gameObject;
                m_notificationData = NotificationView.CreateDataInstance();
            }
        }
        else {
            if (NotificationViewObject)
            {
                GameObject.DestroyImmediate(NotificationViewObject);
                NotificationViewObject = null;
                m_notificationData = null;
            }
        }
    }

    #region WritePrefab

    void createTemplate()
    {
        if (NotificationViewObject)
        {
            GameObject.DestroyImmediate(NotificationViewObject);
            NotificationViewObject = null;
            m_notificationData = null;
        }
        NotificationViewObject = Instantiate(NotificationController.PrefabManager.DefaultViewPrefab, NotificationController.NotificationContainer);
        //Debug.Log(NotificationViewObject);
        createScript();
    }

    bool m_fileWritten = false;
    bool m_fileDeleting = false;
    string m_viewTypeName;
    string m_dataTypeName;

    private void Update()
    {
        if (m_fileWritten)
        {
            if (UnityEditor.VersionControl.Provider.activeTask != null)
            {
                return;
            }
            if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                return;
            }
            m_fileWritten = false;

            saveToPrefab();
        }
        if (m_fileDeleting)
        {
            if (UnityEditor.VersionControl.Provider.activeTask != null)
            {
                return;
            }
            AssetDatabase.Refresh();
            if (EditorApplication.isUpdating || EditorApplication.isCompiling)
            {
                return;
            }
            if (--m_selectedTemplate < 0)
            {
                m_selectedTemplate = 0;
            }
            //Debug.Log("here");
            UpdateInfo();
            m_fileDeleting = false;
        }
    }
    private void createScript()
    {
        m_viewTypeName = NotificationFileWriter.WriteViewClass(m_selectedApp, m_templateID2Create, NotificationEditingBehavior.Instance.NotificationController.PrefabPath);
        m_dataTypeName = NotificationFileWriter.WriteDataClass(m_selectedApp, m_templateID2Create, NotificationEditingBehavior.Instance.NotificationController.PrefabPath);
        AssetDatabase.Refresh();
        m_fileWritten = true;
        //Debug.Log(NotificationViewObject);
    }

    private void saveToPrefab()
    {
        var assemblies = System.AppDomain.CurrentDomain.GetAssemblies();
        System.Type viewType = null;
        System.Type dataType = null;
        foreach (var assembly in assemblies)
        {
            viewType = assembly.GetType(m_viewTypeName);
            if (viewType != null)
            {
                dataType = assembly.GetType(m_dataTypeName);
                break;
            }
        }
        //Debug.Log(viewType);

        NotificationView notificationView = NotificationViewObject.AddComponent(viewType) as NotificationView;
        notificationView.Initialize(m_selectedApp, m_templateID2Create, "Editor");
        notificationView.NotificationDataReflection = dataType.AssemblyQualifiedName;
        //MonoScript[] scripts = (MonoScript[])Resources.FindObjectsOfTypeAll(typeof(MonoScript));

        //foreach (MonoScript script in scripts)
        //{
        //    if (script.GetClass() != null && script.GetClass() == dataType)
        //    {
        //        notificationView.NotificationData = script;
        //        break;
        //    }
        //}
        PrefabUtility.SaveAsPrefabAssetAndConnect(NotificationViewObject, "Assets" + NotificationController.PrefabPath + "/" + m_templateID2Create + "_" + (int)m_selectedApp + "_" + "NotificationView.prefab", InteractionMode.AutomatedAction);
        m_selectedTemplate = m_templateOption.Length - 1;
        IsCreating = false;
        UpdateInfo();
    }
    #endregion

    void deleteFiles()
    {
        if (NotificationViewObject)
        {
            DestroyImmediate(NotificationViewObject);
            NotificationViewObject = null;
            m_notificationData = null;
        }
        NotificationFileWriter.DeleteViewClass(m_selectedApp, m_templateOption[m_selectedTemplate], NotificationEditingBehavior.Instance.NotificationController.PrefabPath);
        NotificationFileWriter.DeleteDataClass(m_selectedApp, m_templateOption[m_selectedTemplate], NotificationEditingBehavior.Instance.NotificationController.PrefabPath);
        NotificationFileWriter.DeletePrefab(m_selectedApp, m_templateOption[m_selectedTemplate], NotificationEditingBehavior.Instance.NotificationController.PrefabPath);
        m_fileDeleting = true;
    }

    GameObject Instantiate(Object gameObject, Transform parentTransform)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(gameObject) as GameObject;
        go.transform.SetParent(parentTransform);
        go.transform.localScale = Vector3.one;
        return go;
    }
}
