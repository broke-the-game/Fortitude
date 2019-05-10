using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NewsDataManager : MonoBehaviour
{
    List<NewsContent> NewsData = new List<NewsContent>();

    public class NewsContent
    {
        public string title { get; }
        public string description { get; }
        public string iconPath { get; }
        public int indexInDatabase { get; }
        public NewsContent (string title, string description, string iconPath, int index)
        {
            this.title = title;
            this.description = description;
            this.iconPath = iconPath;
            this.indexInDatabase = index;
        }

        public class Comparation : IComparer<NewsContent>
        {
            private Comparation() { }

            private static Comparation m_instance;
            public static Comparation Instance
            {
                get
                {
                    if (m_instance == null)
                        m_instance = new Comparation();
                    return m_instance;
                }
            }

            public int Compare(NewsContent x, NewsContent y)
            {
                return x.indexInDatabase.CompareTo(y.indexInDatabase);
            }
        }
    }

    public int GetNewsContentCount() => NewsData.Count;

    public NewsContent GetNews(int index)
    {
        if (index < NewsData.Count)
        {
            return NewsData[index];
        }

        return null;
    }

    public class NewsDataDesc: AppDataManager.DataDesc
    {
        public string title;
        public string description;
        public string iconPath;
        public NewsDataDesc (int index, string title, string iconPath, string description)
            :base(index)
        {
            this.title = title;
            this.description = description;
            this.iconPath = iconPath;
        }
    }

    public void AcquiredLatestNewsContent (List<NewsDataDesc> dataDesc)
    {
        NewsData.Clear();
        for (int i = 0; i < dataDesc.Count; i++)
        {
            NewsData.Add(new NewsContent(dataDesc[i].title, dataDesc[i].description, dataDesc[i].iconPath, dataDesc[i].id));
        }
        NewsData.Sort(NewsContent.Comparation.Instance);
    }

    public void AppendPreviousNewsContent(List<NewsDataDesc> dataDesc)
    {
        for (int i = 0; i < dataDesc.Count; i++)
        {
            if (!NewsData.Any(d => d.indexInDatabase == dataDesc[i].id))
            {
                NewsData.Add(new NewsContent(dataDesc[i].title, dataDesc[i].description, dataDesc[i].iconPath, dataDesc[i].id));
            }
        }
        NewsData.Sort(NewsContent.Comparation.Instance);
    }
}
