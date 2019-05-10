using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelResizer : MonoBehaviour
{
    #region Singleton
    private static PanelResizer m_instance;
    public static PanelResizer Instance { get { return m_instance; } }
    private void Awake()
    {
        m_instance = this;
    }
    #endregion
    [SerializeField] ResizeChoiceButtons rcb;

}
