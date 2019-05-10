using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class NotificationController : MonoBehaviour
{
    #region Singleton
    private static NotificationController m_instance;
    public static NotificationController Instance { get { return m_instance; } }
    private void Awake()
    {
        SetInstance();
    }
    public void SetInstance()
    {
        m_instance = this;

    }
    #endregion

    public string PrefabPath;

    [SerializeField]
    NotificationViewPrefabManager m_prefabManager;

    [Space,SerializeField]
    RectTransform m_notificationContainer;

    [Space, Header("Show Notification Lerp Params"), SerializeField]
    float m_lerpSpeed, m_lerpThreshold;

    public RectTransform NotificationContainer { get { return m_notificationContainer; } }
    public NotificationViewPrefabManager PrefabManager { get { return m_prefabManager; } }

    [System.Serializable]
    public struct NotificationId
    {
        public Utility.App app;
        public string id;
        public NotificationId(Utility.App app, string id)
        {
            this.app = app;
            this.id = id;
        }
        public static bool operator ==(NotificationId a,NotificationId b)
        {
            return a.app == b.app && a.id == b.id;
        }
        public static bool operator !=(NotificationId a, NotificationId b)
        {
            return !(a == b);
        }
    }

    public void PushNotification(NotificationData data, string notificationIdentifier = "")
    {
        if (data.TemplateId == "")
        {
            Debug.LogError("[ERROR] TemplateId Not Set");
            return;
        }
        if (!PrefabManager.IsContainPrefab(data.App, data.TemplateId))
        {
            Debug.LogError("[ERROR] TemplateId Not Found");
            return;
        }

        NotificationId id = new NotificationId(data.App, notificationIdentifier);
        NotificationView notificationView;
        if (!tryGetNotificationView(id, out notificationView))
        {
            notificationView = NotificationView.CreateNotificationView(data.TemplateId, id);
        }
        loadNewTemplate(ref notificationView, data.TemplateId);
        notificationView.ResolveNotificationData(data);
        notificationView.transform.SetAsFirstSibling();
        StartCoroutine(notificationView.ShowTransition(m_lerpSpeed, m_lerpThreshold));
    }

    public void HideNotification(Utility.App app, string notificationIdentifier)
    {
        NotificationId id = new NotificationId(app, notificationIdentifier);
        NotificationView notificationView;
        if (tryGetNotificationView(id, out notificationView))
        {
            StartCoroutine(notificationView.HideTransition(m_lerpSpeed, m_lerpThreshold, destroyNotification,notificationView));
        }
    }

    void destroyNotification(NotificationView notificationView)
    {
        Destroy(notificationView.gameObject);
    }

    private void loadNewTemplate(ref NotificationView view, string templateId)
    {
        NotificationId tempId = view.Id;
        Destroy(view.gameObject);
        view = NotificationView.CreateNotificationView(templateId, tempId);
    }

    public bool tryGetNotificationView(NotificationId id, out NotificationView view)
    {
        view = null;
        NotificationView[] viewList = NotificationContainer.GetComponentsInChildren<NotificationView>();
        for (int i = 0; i < viewList.Length; i++)
        {
            if (viewList[i].Id == id)
            {
                view = viewList[i];
                return true;
            }
        }
        return false;
    }

    public NotificationView[] GetNotificationViewList()
    {
        return NotificationContainer.GetComponentsInChildren<NotificationView>();
    }

    public void LoadViewPrefabs()
    {
        PrefabManager.LoadViewPrefabs(PrefabPath);
    }

    public NotificationData CreateDataInstance(Utility.App app, string templateId)
    {
        GameObject prefab;
        if (PrefabManager.TryGetPrefab(app, templateId, out prefab))
        {
            System.Type dataType = System.Type.GetType(prefab.GetComponent<NotificationView>().NotificationDataReflection);
            NotificationData data = (NotificationData)System.Activator.CreateInstance(dataType);
            data.App = app;
            data.TemplateId = templateId;
            return data;
        }
        return null;
    }
    private void Start()
    {
        LoadViewPrefabs();
    }

}
