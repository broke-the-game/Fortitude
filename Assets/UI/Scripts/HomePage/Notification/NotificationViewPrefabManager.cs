using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

[System.Serializable]
public class NotificationViewPrefabManager
{
    [SerializeField]
    GameObject m_defaultViewPrefab;

    [SerializeField]
    List<GameObject>[] ViewTemplatePrefab;

    public GameObject DefaultViewPrefab { get { return m_defaultViewPrefab; } }

    public void LoadViewPrefabs(string path)
    {
        ViewTemplatePrefab = new List<GameObject>[(int)Utility.App.COUNT];

        for (int i = 0; i < ViewTemplatePrefab.Length; i++)
        {
            ViewTemplatePrefab[i] = new List<GameObject>();
        }

        string assetsPath = path.Substring("/Resources/".Length);
        Object[] prefabs =Resources.LoadAll(assetsPath,typeof(GameObject));
        NotificationView prefabView;
        for (int i = 0; i < prefabs.Length; i++)
        {
            prefabView = ((GameObject)prefabs[i]).GetComponent<NotificationView>();
            if (prefabs[i].name == "DefaultNotificationView")
            {
                m_defaultViewPrefab = (GameObject)prefabs[i];
                continue;
            }
            if(prefabView)
                ViewTemplatePrefab[(int)prefabView.Id.app].Add((GameObject)prefabs[i]);
        }


        //string[] prefabGuids = AssetDatabase.FindAssets("NotificationView t:prefab", new[] { assetsPath });
        //string filePath;
        //GameObject prefab;
        //NotificationView prefabViewScript;
        //Utility.App app;
        //foreach (string guid in prefabGuids)
        //{
        //    filePath = AssetDatabase.GUIDToAssetPath(guid);
        //    //Debug.Log(filePath);
        //    prefab = AssetDatabase.LoadAssetAtPath<GameObject>(filePath);
        //    prefabViewScript = prefab.GetComponent<NotificationView>();
        //    if (!prefabViewScript)
        //    {
        //        m_defaultViewPrefab = prefab;
        //        continue;
        //    }
        //    app = prefabViewScript.Id.app;
        //    ViewTemplatePrefab[(int)app].Add(prefab);
        //}
    }

    public bool TryGetPrefab(Utility.App app, string templateId, out GameObject prefab)
    {
        GameObject temp;
        for (int i = 0; i < ViewTemplatePrefab[(int)app].Count; i++)
        {
            temp = ViewTemplatePrefab[(int)app][i];
            if (temp.GetComponent<NotificationView>().TemplateId == templateId)
            {
                prefab = temp;
                return true;
            }
        }
        prefab = null;
        return false;
    }

    public bool IsContainPrefab(Utility.App app, string templateId)
    {
        if (ViewTemplatePrefab == null)
            return false;
        if (ViewTemplatePrefab[(int)app] == null)
            return false;
        GameObject temp;
        for (int i = 0; i < ViewTemplatePrefab[(int)app].Count; i++)
        {
            temp = ViewTemplatePrefab[(int)app][i];
            if (temp.GetComponent<NotificationView>().TemplateId == templateId)
            {
                return true;
            }
        }
        return false;
    }

    public GameObject[] GetPrefabList(Utility.App app)
    {
        if (ViewTemplatePrefab == null || ViewTemplatePrefab[(int)app]==null)
            return new GameObject[0];
        return ViewTemplatePrefab[(int)app].ToArray();
    }

    public string[] GetTemplateOption(Utility.App app)
    {
        GameObject[] prefabs = GetPrefabList(app);
        string[] option = new string[prefabs.Length];
        for (int i = 0; i < option.Length; i++)
        {
            option[i] = prefabs[i].GetComponent<NotificationView>().TemplateId;
        }
        return option;
    }
}
