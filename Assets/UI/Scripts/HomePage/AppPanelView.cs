using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class AppPanelView
{

    [SerializeField]
    RectTransform AppWindow;

    [SerializeField]
    RectTransform AppWindowDown;

    private Vector3 m_appWindowTargetPos;

    [SerializeField]
    TMPro.TextMeshProUGUI Title;

    [SerializeField]
    Image Icon;

    [SerializeField]
    RectTransform AppContentPanel;

    [SerializeField, Range(0f,100f)]
    float m_lerpSpeed;

    [SerializeField, Range(0.8f, 1f)]
    float m_lerpThreshold;


    Coroutine m_appPanelShowTransition;

    public void SetInAppPanel(RectTransform rootUI)
    {
        AppWindow.gameObject.SetActive(false);
        if (!rootUI)
            return;
        rootUI.SetParent(AppContentPanel);
        rootUI.anchorMin = Vector2.zero;
        rootUI.anchorMax = Vector2.one;
        rootUI.anchoredPosition = Vector2.zero;
        rootUI.sizeDelta = Vector2.zero;
        rootUI.localScale = Vector3.one;
        rootUI.gameObject.SetActive(false);
    }

    public void ShowAppContent(Utility.App app)
    {
        hidePreApp();
        ViewController.Instance.CurrentAppController = Utility.GetAppController(app);
        if (!ViewController.Instance.CurrentAppController)
            return;
        if (!ViewController.Instance.CurrentAppController.RootUI)
            return;
        SetAppContentInfo(ViewController.Instance.CurrentAppController.AppInfo);
        ViewController.Instance.CurrentAppController.RootUI.gameObject.SetActive(true);
        AppWindow.gameObject.SetActive(true);
        if (m_appPanelShowTransition != null)
        {
            ViewController.Instance.StopCoroutine(m_appPanelShowTransition);
            m_appPanelShowTransition = null;
        }
        m_appPanelShowTransition = ViewController.Instance.StartCoroutine(showTransition());
    }

    public void HideAppContent()
    {
        if (m_appPanelShowTransition != null)
        {
            ViewController.Instance.StopCoroutine(m_appPanelShowTransition);
            m_appPanelShowTransition = null;
        }
        m_appPanelShowTransition = ViewController.Instance.StartCoroutine(hideTransition());

    }

    public void SetAppContentTitle(string title)
    {
        Title.text = title;
    }

    public void SetAppContentInfo(AppInfo info)
    {
        Title.text = info.AppName;
        Icon.sprite = info.AppIcon;
    }

    private void hideAllApps()
    {
        AppController[] appController = Utility.GetAppControllers();
        for (int i = 0; i < appController.Length; i++)
        {
            appController[i].RootUI.gameObject.SetActive(false);
            appController[i].IsShow = false;
            appController[i].IsShowBeforeTransition = false;
            appController[i].OnHide();
        }
    }

    private void hidePreApp()
    {
        if (!ViewController.Instance.CurrentAppController)
            return;
        ViewController.Instance.CurrentAppController.RootUI.gameObject.SetActive(false);
        ViewController.Instance.CurrentAppController.IsShow = false;
        ViewController.Instance.CurrentAppController.IsShowBeforeTransition = false;
        ViewController.Instance.CurrentAppController.OnHide();
        ViewController.Instance.CurrentAppController = null;
    }

    IEnumerator showTransition()
    {
        float progress = 0;
        preShowApp();
        ViewController.Instance.CurrentAppController.OnShowBeforeTransition();
        ViewController.Instance.CurrentAppController.IsShowBeforeTransition = true;
        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, m_lerpSpeed * Time.deltaTime);
            onShowApp(progress);
            if (progress > m_lerpThreshold)
            {
                postShowApp();
                ViewController.Instance.CurrentAppController.IsShow = true;
                ViewController.Instance.CurrentAppController.OnShow();
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator hideTransition()
    {
        float progress = 0;
        AppWindow.position = m_appWindowTargetPos;
        ViewController.Instance.CurrentAppController.IsShowBeforeTransition = false;
        ViewController.Instance.CurrentAppController.IsShow = false;
        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, m_lerpSpeed * Time.deltaTime);
            AppWindow.position = Vector3.Lerp(m_appWindowTargetPos, AppWindowDown.position, progress);
            if (progress > m_lerpThreshold)
            {
                AppWindow.position = AppWindowDown.position;
                ViewController.Instance.CurrentAppController.RootUI.gameObject.SetActive(false);
                ViewController.Instance.CurrentAppController.OnHide();
                ViewController.Instance.CurrentAppController = null;
                AppWindow.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    void preShowApp()
    {
        AppWindow.position = AppWindowDown.position;
    }

    void onShowApp(float progress)
    {
        AppWindow.position = Vector3.Lerp(AppWindowDown.position, m_appWindowTargetPos, progress);
        AppWindow.gameObject.SetActive(false);
        AppWindow.gameObject.SetActive(true);
    }

    void postShowApp()
    {
        AppWindow.position = m_appWindowTargetPos;
    }


    public void OnStart()
    {
        m_appWindowTargetPos = AppWindow.position;

    }
}
