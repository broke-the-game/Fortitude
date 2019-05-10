using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[ExecuteInEditMode]
public class NotificationEditingBehavior : MonoBehaviour
{
    #region Singleton
    private static NotificationEditingBehavior m_instance;
    public static NotificationEditingBehavior Instance { get { return m_instance; }}

    #endregion

    [SerializeField]
    NotificationController m_notificationController;

    public NotificationController NotificationController { get { return m_notificationController; } }

    private void OnEnable()
    {
        m_instance = this;
    }

    public void ClearNotificationSpace()
    {
        int count = NotificationController.NotificationContainer.childCount;
        for (int i = 0; i < count; i++)
        {
            DestroyImmediate(NotificationController.NotificationContainer.GetChild(i).gameObject);
        }
    }
}
