using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreManager : MonoBehaviour
{
    #region Singleton
    private static CoreManager m_instance;
    public static CoreManager Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(checkLoadFinish());
    }

    IEnumerator checkLoadFinish()
    {
        yield return null;
        while (true)
        {
            if (AppSceneLoader.Instance.ScenesLoaded())
            {
                //Cluster Assign
                yield break;
            }
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
