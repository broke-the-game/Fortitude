using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class NewsAppController : AppController
{
    [SerializeField]
    public NewsDataManager NewsDataManager;

    [SerializeField]
    public NewsOperations NewsOperations;

    [SerializeField, Range(0, 100)]
    int NewsObtainedCount;

    [SerializeField]
    ScrollRect scroll;

    List<NewsExec> currentlyExecutingNews = new List<NewsExec>();
    List<string> notifiId = new List<string>();

    public override void ExecuteScriptableObject(AppExecution scriptable)
    {
        NewsExec newsExec = (NewsExec)scriptable;
        if (NotificationController.Instance && !IsShowBeforeTransition)
        {
            NewNews_2_NotificationData data = (NewNews_2_NotificationData)NotificationController.Instance.CreateDataInstance(Utility.App.News, "NewNews");
            data.Title = newsExec.title;
            data.Preview = newsExec.description;
            NotificationController.Instance.PushNotification(data, data.Title);
            notifiId.Add(data.Title);
        }
        currentlyExecutingNews.Add(newsExec);
    }

    public override void Init()
    {
        RequestLatestNewsContents();
    }

    public override void OnHide()
    {
        // Write executed news to the history and set the state to finished
        List<AppDataManager.DataDesc> dataDescList = new List<AppDataManager.DataDesc>();
        for (int i = 0; i < currentlyExecutingNews.Count; i++)
        {
            dataDescList.Add(new NewsDataManager.NewsDataDesc(-1, currentlyExecutingNews[i].title, currentlyExecutingNews[i].iconPath, currentlyExecutingNews[i].description));
            AppCallbackModule.Instance.Execute(currentlyExecutingNews[i].appCallback);
        }
        AppDataManager.SetData(AppDataManager.Protocol.NEWS_WRITE_TO_HISTORY, dataDescList);

        // NewsDataManager list up-to-date with the db
        RequestLatestNewsContents();
        currentlyExecutingNews.Clear();
    }

    public override void OnShow()
    {
        
    }

    public override void OnShowBeforeTransition()
    {
        NewsOperations.ClearNews();
        int historyCount = NewsDataManager.GetNewsContentCount();
        Debug.Log("Before Updating View");
        for (int i = 0; i < historyCount; i++)
        {
            NewsDataManager.NewsContent news = NewsDataManager.GetNews(i);
            NewsOperations.UpdateNewsUI(news.title, news.description, news.iconPath);
        }
        Debug.Log("After Updating View");

        for (int i = 0; i < currentlyExecutingNews.Count; i++)
        {
            NewsOperations.UpdateNewsUI(currentlyExecutingNews[i].title, currentlyExecutingNews[i].description, currentlyExecutingNews[i].iconPath);
        }
        for (int i = 0; i < notifiId.Count; i++)
        {
            NotificationController.Instance.HideNotification(Utility.App.News, notifiId[i]);
            notifiId.Remove(notifiId[i]);
        }
        Utility.ScrollTo(scroll, 1f);
    }

    public void RequestLatestNewsContents()
    {
        List<NewsDataManager.NewsDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.NEWS_GET_LATEST, new string[] { NewsObtainedCount.ToString() }).Cast<NewsDataManager.NewsDataDesc>().ToList();
        NewsDataManager.AcquiredLatestNewsContent(data);
    }

    public void RequestNewsFromIndex(int fromIndex)
    {
        List<NewsDataManager.NewsDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.NEWS_GET_FROM_INDEX, new string[] { fromIndex.ToString(), NewsObtainedCount.ToString() }).Cast<NewsDataManager.NewsDataDesc>().ToList();
        NewsDataManager.AppendPreviousNewsContent(data);
    }

    public void RequestNewsFromSituation(int situationId)
    {
        List<NewsDataManager.NewsDataDesc> data = AppDataManager.RequestData(AppDataManager.Protocol.NEWS_GET_LATEST_BY_SITUATION, new string[] { situationId.ToString() }).Cast<NewsDataManager.NewsDataDesc>().ToList();
        NewsDataManager.AppendPreviousNewsContent(data);
    }
    
}
