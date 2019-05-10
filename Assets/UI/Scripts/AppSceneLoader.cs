using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AppSceneLoader : MonoBehaviour
{
    #region Singleton
    private static AppSceneLoader m_instance;
    public static AppSceneLoader Instance { get { return m_instance; } }

    private void Awake()
    {
        m_instance = this;
    }
    #endregion

    private class SceneLoadObject
    {
        public string sceneName { get; private set; }
        public Utility.App app { get; private set; }
        public SceneLoadObject(Utility.App app, string sceneName)
        {
            this.sceneName = sceneName;
            this.app = app;
        }
    }

    private AsyncOperation async;

    private Coroutine m_loadSceneCoroutine;

    private Queue<SceneLoadObject> m_sceneToLoad = new Queue<SceneLoadObject>();

    public void LoadAppScene(Utility.App app, string sceneName)
    {
        SceneLoadObject sceneLoadObject = new SceneLoadObject(app, sceneName);
        m_sceneToLoad.Enqueue(sceneLoadObject);
        if (m_loadSceneCoroutine == null)
        {
            m_loadSceneCoroutine = StartCoroutine(sceneLoading());
        }
    }

    private bool LoadSceneAdditive(string sceneName)
    {
        if (sceneName == "")
            return false;
        async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        return true;
    }

    public bool ScenesLoaded()
    {
        return async == null && m_sceneToLoad.Count == 0;
    }

    IEnumerator sceneLoading()
    {
        SceneLoadObject sceneLoadObject;
        while (m_sceneToLoad.Count > 0)
        {
            sceneLoadObject = m_sceneToLoad.Dequeue();
            if (LoadSceneAdditive(sceneLoadObject.sceneName))
            {
                while (async != null && !async.isDone)
                {
                    yield return null;
                }
            }
        }
        m_loadSceneCoroutine = null;
        async = null;
    }
}
