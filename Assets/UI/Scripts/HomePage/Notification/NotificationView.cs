using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManaging;
//using UnityEditor;

[RequireComponent(typeof(NotificationViewInfo))]
public abstract class NotificationView : MonoBehaviour
{
    [SerializeField, HideInInspector]
    private NotificationController.NotificationId m_id;

    [SerializeField,HideInInspector]
    private string m_templateId;

    bool willBeDestroyed;

    public Image Icon { get { return GetComponent<NotificationViewInfo>().Icon; } }

    public TMPro.TextMeshProUGUI Title { get { return GetComponent<NotificationViewInfo>().Title; } }

    public NotificationViewInfo NotificationViewInfo { get { return GetComponent<NotificationViewInfo>(); } }

    public NotificationController.NotificationId Id { get { return m_id; } }

    [SerializeField, HideInInspector]
    public string NotificationDataReflection;

    public string TemplateId { get { return m_templateId; } set { m_templateId = value; } }

    public abstract void ResolveNotificationData(NotificationData data);

    protected virtual void OnClick() { }

    public float Height { get { return NotificationViewInfo.AspectRatio; } set { NotificationViewInfo.AspectRatio = value; } }

    public RectTransform CustomizablePanel { get { return NotificationViewInfo.CustomPanel; } }

    //public void ShowNotification(NotificationData data)
    //{
    //    loadTemplate(data.TemplateId);
    //    ResolveNotificationData(data);
    //}

    //private void loadTemplate(string templateId)
    //{
    //}

    private void OnEnable()
    {
        GetComponent<Button>().onClick.AddListener(OpenNotification);
    }

    public void SetIcon(Sprite image)
    {
        if (!Icon || !image)
            return;
        Icon.sprite = image;
    }
    public void SetTitle(string title)
    {
        if (!Title)
            return;
        Title.text = title;
    }

    public static NotificationView CreateNotificationView(string templateId, NotificationController.NotificationId id)
    {
        NotificationController notificationController;
#if UNITY_EDITOR
        if (NotificationEditingBehavior.Instance)
        {
            notificationController = NotificationEditingBehavior.Instance.NotificationController;
            GameObject prefab;
            if (notificationController.PrefabManager.TryGetPrefab(id.app, templateId, out prefab))
            {
                GameObject viewObject = UnityEditor.PrefabUtility.InstantiatePrefab(prefab) as GameObject;
                viewObject.transform.SetParent(notificationController.NotificationContainer);
                viewObject.transform.localScale = Vector3.one;
                NotificationView view = viewObject.GetComponent<NotificationView>();
                view.SetIdentifier(id.id);
                return view;
            }
        }
#endif
        if(NotificationController.Instance)
        {
            notificationController = NotificationController.Instance;
            GameObject prefab;
            if (notificationController.PrefabManager.TryGetPrefab(id.app, templateId, out prefab))
            {
                GameObject viewObject = Instantiate(prefab, notificationController.NotificationContainer);
                NotificationView view = viewObject.GetComponent<NotificationView>();
                view.SetIdentifier(id.id);
                return view;
            }
        }

        return null;
    }
    public NotificationData CreateDataInstance()
    {
        NotificationData data = (NotificationData)System.Activator.CreateInstance(System.Type.GetType(NotificationDataReflection));
        data.App = Id.app;
        data.TemplateId = TemplateId;
        return data;
    }

    public void Initialize(Utility.App app, string templateId, string notificationIdentifier)
    {
        m_id.app = app;
        TemplateId = templateId;
        m_id.id = notificationIdentifier;
        AppInfo info = Utility.GetAppInfo(app);
        if (!info)
            return;
        SetIcon(info.AppIcon);
        SetTitle(info.AppName);
    }

    public void OpenNotification()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);
        OnClick();
        if (ViewController.Instance)
        {
            ViewController.Instance.OpenApp(Id.app);
        }
    }

    public void SetIdentifier(string identifier)
    {
        m_id.id = identifier;
    }

    public IEnumerator ShowTransition(float speed, float threshold)
    {
        float progress = 0;
        float targetHeight = Height;
        transform.localScale = Vector3.zero;
        Height = 0f;
        while (true)
        {
            if (willBeDestroyed)
            {
                yield break;
            }
            progress = Mathf.Lerp(progress, 1f, speed * Time.deltaTime);
            if (progress < threshold / 2f)
            {
                Height = Mathf.Lerp(0f, targetHeight, progress * 2f);
            }
            if (progress > threshold/2f)
            {
                Height = targetHeight;
                transform.localScale = (progress - threshold / 2f) * 2f * Vector3.one;
                if (progress > threshold)
                {
                    transform.localScale = Vector3.one;
                    yield break;
                }
                //Debug.Log(progress);
            }
            yield return null;
        }
    }
    public IEnumerator HideTransition(float speed, float threshold, UnityEngine.Events.UnityAction<NotificationView> afterHide, NotificationView notificationView)
    {
        willBeDestroyed = true;
        float progress = 0;
        float targetHeight = Height;
        transform.localScale = Vector3.one;
        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, speed * Time.deltaTime);

            transform.localScale = Vector3.Lerp(Vector3.one, Vector3.zero, progress * 2f);
            if (progress > threshold/2f)
            {
                transform.localScale = Vector3.zero;
                afterHide(notificationView);
                yield break;
            }
            Debug.Log(progress);
            yield return null;
        }
    }
}
