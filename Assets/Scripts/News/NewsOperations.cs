using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewsOperations : MonoBehaviour
{
    public class NewsContent {

        public NewsContent(string Title, string Description, string IconPath)
        {
            this.Title = Title;
            this.Description = Description;
            this.IconPath = IconPath;
        }

        public string Title { get; }
        public string Description { get; }
        public string IconPath { get; }
    }

    List<GameObject> newsListObject = new List<GameObject>();

    [SerializeField]
    private RectTransform contents;

    [SerializeField]
    private RectTransform contentContainer;
    

    List<NewsContent> newsHistory = new List<NewsContent>();
    // Start is called before the first frame update
    void Start()
    {
        //UpdateNews("State Votes to Decrease Public Transportation Funding Due to Budget Cuts", "Concerned citizens say this cut  will increase the cost for all bus/subway tickets and many routes will be slashed making it more difficult to commute for work.", "Images/drough_icon");
        //StartCoroutine(NextNews());
        
    }
    IEnumerator NextNews()
    {
        yield return new WaitForSeconds(3);
        UpdateNews("Government declares a state of emergency following an unprecendented drought in the region", "The drought has had the greatest impact on local farmers but the skyrocketing prices in produce have been affecting all citizens", "Images/drough_icon");
        yield break;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddNewContent(string[] data)
    {
        string title = data[0];
        string desc = data[1];
        string iconPath = data[2];
        float bankImpact = float.Parse(data[3]);
        string bankSummary = data[4];
        UpdateNews(title, desc, iconPath);
    }

    public void UpdateNews(string title, string desc, string iconPath) 
    {
        newsHistory.Add(new NewsContent(title, desc, iconPath));
        UpdateNewsUI(title, desc, iconPath);
    }

    public void UpdateNewsUI(string title, string desc, string iconPath)
    {
        RectTransform newContent = Instantiate(contents, contentContainer);
        newContent.gameObject.SetActive(true);
        newContent.SetAsFirstSibling();
        newsListObject.Add(newContent.gameObject);
        //RectTransform contentPanel = newContent.Find("Content").GetComponent<RectTransform>();
        RectTransform titleGameObj = newContent.Find("Title and Icon").GetComponent<RectTransform>().Find("Title").GetComponent<RectTransform>();
        RectTransform titleFill = titleGameObj.Find("TitleFill").GetComponent<RectTransform>();
        titleFill.GetComponent<TMPro.TextMeshProUGUI>().text = title;
        RectTransform description = newContent.Find("Description").GetComponent<RectTransform>();
        description.GetComponent<TMPro.TextMeshProUGUI>().text = desc;
        //RectTransform datePanel = newContent.Find("Date").GetComponent<RectTransform>();
        //datePanel.GetComponent<TMPro.TextMeshProUGUI>().text = System.DateTime.Now.ToString("MMMM dd");
        RectTransform iconFill = newContent.Find("Title and Icon").GetComponent<RectTransform>().Find("IconContainer").Find("Icon").GetComponent<RectTransform>();
        string iconFullPath;
#if UNITY_IOS
            iconFullPath = "file://" + Application.dataPath + "/Raw/news_pic/" + iconPath + ".png";
#endif
#if UNITY_ANDROID
        iconFullPath = "jar:file://" + Application.dataPath + "!/assets/news_pic/" + iconPath + ".png";
#endif
#if UNITY_EDITOR
        iconFullPath = "file:///" + Application.dataPath + "/StreamingAssets/news_pic/" + iconPath + ".png";
#endif
        StartCoroutine(loadIcon(iconFullPath, iconFill, newContent));
        //newContent.gameObject.SetActive(false);
    }

    IEnumerator<WWW> loadIcon(string path, RectTransform iconFill, RectTransform newContent)
    {
        Texture2D tex = new Texture2D(512, 512);

        WWW www = new WWW(path);
        yield return www;

        Debug.Log("Tex yet Loaded " + www.error);
        tex.LoadImage(www.bytes);
        Debug.Log("Tex Loaded");
        iconFill.GetComponent<Image>().sprite = Sprite.Create(tex as Texture2D, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        newContent.gameObject.SetActive(true);
    }

    public void ClearNews()
    {
        for (int i = newsListObject.Count - 1; i >= 0 ; i--)
        {
            GameObject go = newsListObject[i];
            newsListObject.RemoveAt(newsListObject.Count - 1);
            DestroyImmediate(go);
        }
    }

}
