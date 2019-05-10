using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public abstract class NotificationData: ScriptableObject
{
    [SerializeField, HideInInspector]
    protected Utility.App app;
    [SerializeField, HideInInspector]
    protected string templateId;
    public Utility.App App { get { return app; } set { app = value; } }
    public string TemplateId { get { return templateId; } set { templateId = value; } }

    protected abstract void initialize();
    public NotificationData()
    {
        initialize();
    }
}

