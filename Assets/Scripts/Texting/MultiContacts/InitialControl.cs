using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class InitialControl : MonoBehaviour
{
    TMPro.TextMeshProUGUI m_text { get { return GetComponent<TMPro.TextMeshProUGUI>(); } }

    [SerializeField]
    TMPro.TextMeshProUGUI m_fullName;

    [SerializeField, Range(0, 10)]
    int m_maxCount;

    public  void SetInitial()
    {
        if (!m_fullName)
            return;
        string[] names = m_fullName.text.Split(' ');
        m_text.text = "";
        for (int i = 0; i < Mathf.Min(names.Length, m_maxCount); i++)
        {
            m_text.text += names[i][0];
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        SetInitial();
#endif
    }
}
