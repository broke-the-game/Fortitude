using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using AudioManaging;

public class BankOperations : MonoBehaviour
{
    public static BankOperations Instance { get; private set; }
    [SerializeField]
    private RectTransform activityContainer;
    [SerializeField]
    private RectTransform activity;

    [SerializeField]
    private RectTransform graphMaxBound;
    [SerializeField]
    private RectTransform graphMinBound;
    [SerializeField]
    private RectTransform backgroundMaxBound;
    [SerializeField]
    private RectTransform backgroundMinBound;
    [SerializeField]
    private RectTransform background;
    [SerializeField]
    private RectTransform labelTemplateX;
    [SerializeField]
    private RectTransform labelTemplateY;
    [SerializeField]
    private GameObject graphLineRenderer;
    LineRenderer lineRenderer;

    [SerializeField]
    float verticalOffset;

    [SerializeField]
    RectTransform StencilBox;

    [SerializeField]
    Sprite up_arrow;
    [SerializeField]
    Sprite down_arrow;

    [SerializeField]
    float maxXSize;

    [SerializeField, Range(0, 20)]
    int xSepCount, ySepCount;

    GameObject[] xSeparator;
    GameObject[] ySeparator;

    public float currentBalance;

    [SerializeField]
    public BankDataManager BankDataManager;

    public UnityAction<float> OnBankAmountUpdated;

    List<GameObject> BankActList = new List<GameObject>();

    public class BankingHistory
    {
        public string activitySummary;
        public float activityAmount;
        public float balance;

        public BankingHistory(string activitySummary, float activityAmount, float balance)
        {
            this.activitySummary = activitySummary;
            this.activityAmount = activityAmount;
            this.balance = balance;
        }
    }

    private void Awake()
    {
        Instance = this;
    }

    // keeps track of the history
    List<BankingHistory> bankingActivity = new List<BankingHistory>();
    // keeping varying amounts to plot graph
    List<float> amountList = new List<float>();

    public Profile Profile { get => Profile.Instance; }
    public TMPro.TextMeshProUGUI AmountText;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = graphLineRenderer.GetComponent<LineRenderer>();
        AmountText.text = "$" + Profile.getAmountInBank();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /// <summary>
    /// Args: string[]{amount, summary}
    /// </summary>
    /// <param name="args"></param>
    public void addBankActivity(AppCallbackEvent.EventData args)
    {
      
        try
        {
            string[] bankInfo = args.GetEventData<string[]>(addBankActivity);
            if (bankInfo.Length != 2)
            {
                throw new System.Exception("Length of the incoming array is not 2");
            }

            float amount = float.Parse(bankInfo[0]);
            string activity = bankInfo[1];
            updateAmount(amount, activity);
            if (NotificationController.Instance)
            {
                AmountChange_3_NotificationData data = NotificationController.Instance.CreateDataInstance(Utility.App.Bank, "AmountChange") as AmountChange_3_NotificationData;
                data.amount = amount;
                data.summary = activity;
                NotificationController.Instance.PushNotification(data, BankAppController.Instance.NotificationCount.ToString());
                BankAppController.Instance.NotificationCount++;
                AudioManager.Instance.Play(AudioEnum.Bank_Notifi);
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    // Add the activity and update the amount in profile.
    public void updateAmount(float amt, string activity)
    {

        //AmountText.text = "$" + value;
        //amountList.Add(value);
        //showActivity(activity, amt);
        //ShowGraph(amountList);
        float value = Profile.getAmountInBank();
        value += amt;
        BankingHistory act = new BankingHistory(activity, amt, value);
        WriteActivityToDb(act);
        BankAppController.Instance.RequestLatestBankActivities();
        OnBankAmountUpdated(amt);
        if (BankAppController.Instance.IsShowBeforeTransition)
        {
            BankAppController.Instance.ShowActivity();
        }
    }

    public void showActivity(string act, float amt)
    {
        RectTransform newActivity = Instantiate(activity, activityContainer);
        newActivity.gameObject.SetActive(true);
        RectTransform panel = newActivity.Find("Panel").GetComponent<RectTransform>();
        RectTransform middlePanel = panel.Find("MiddlePanel").GetComponent<RectTransform>();
        RectTransform description = middlePanel.Find("Description").GetComponent<RectTransform>();
        RectTransform descContent = description.Find("GameObject").GetComponent<RectTransform>();
        descContent.GetComponent<TMPro.TextMeshProUGUI>().text = act;
        //RectTransform timePanel = middlePanel.Find("Time").GetComponent<RectTransform>();
        //RectTransform timeGO = timePanel.Find("GameObject").GetComponent<RectTransform>();
        //timeGO.GetComponent<TMPro.TextMeshProUGUI>().text = System.DateTime.Now.ToString("MMMM dd");
        //RectTransform amountPanel = panel.Find("AmountPanel").GetComponent<RectTransform>();
        //RectTransform amountValue = amountPanel.Find("Text").GetComponent<RectTransform>();

        RectTransform icon = panel.Find("Icon").GetComponent<RectTransform>();
        RectTransform iconMask = icon.Find("IconMask").GetComponent<RectTransform>();
        RectTransform originalIcon = iconMask.Find("OriginalIcon").GetComponent<RectTransform>();
        
        if (amt < 0)
        {
            originalIcon.GetComponent<Image>().sprite = down_arrow;
        } else
        {
            originalIcon.GetComponent<Image>().sprite = up_arrow;
        }
        
        string amtSign = "";
        if (amt < 0)
        {
            amtSign = "-";
        }
        //amountValue.GetComponent<TMPro.TextMeshProUGUI>().text = amtSign + " $" + Mathf.Abs(amt);
        BankActList.Add(newActivity.gameObject);
    }
    
    public void ShowGraph(List<float> amounts)
    {
        Vector2 graphSize = new Vector2(graphMaxBound.localPosition.x - graphMinBound.localPosition.x, graphMaxBound.localPosition.y - graphMinBound.localPosition.y);
        Vector2 backgroundSize = new Vector2(backgroundMaxBound.localPosition.x - backgroundMinBound.localPosition.x, backgroundMaxBound.localPosition.y - backgroundMinBound.localPosition.y);
        Debug.Log("Graph height: " + graphSize.y);
        float xSize = Mathf.Min(maxXSize, graphSize.x / amounts.Count);
        float max = 0f, min = 0f;
        for (int i = 0; i < amounts.Count; i++)
        {
            if (max < amounts[i])
                max = amounts[i];
            if (min > amounts[i])
                min = amounts[i];
        }

        float diff = max - min;

        float scaledD = diff / (ySepCount - 3f) / 10f;

        int interval;
        if (scaledD <= 0.5f)
        {
            if (scaledD <= 0.1f)
            {
                interval = 1;
            }
            else
            {
                interval = 5;
            }
        }
        else
        {
            interval = Mathf.CeilToInt(scaledD) * 10;  
        }
        int maxGap = (interval * (ySepCount - 1) - Mathf.CeilToInt(diff)) / interval;
        int yMinimum = (Mathf.FloorToInt(min) / interval - maxGap / 2) * interval;
        int yMaximum = yMinimum + (ySepCount - 1) * interval;
        Debug.Log("List size: " + amounts.Count);
        for (int i = 0; i < amounts.Count; i++)
        {
            float xPosition = i * xSize;
            float yPosition = ((amounts[i] - yMinimum) / (yMaximum - yMinimum)) * graphSize.y;
            lineRenderer.positionCount = i + 1;
            lineRenderer.SetPosition(i, new Vector3(xPosition, yPosition, 0) + graphMinBound.localPosition);
        }
        StencilBox.sizeDelta = new Vector2(StencilBox.sizeDelta.x, ((0f - yMinimum) / (yMaximum - yMinimum)) * graphSize.y);
        /* for (int i = 0; i < ySepCount;  i++)
        {
            Transform labelY = ySeparator[i].transform;
            labelY.gameObject.SetActive(true);
            float normalizedValue = (float)i / (ySepCount - 1);
            labelY.localPosition = new Vector3((backgroundMinBound.localPosition.x + graphMinBound.localPosition.x) / 2f, normalizedValue * graphSize.y + graphMinBound.localPosition.y + verticalOffset, 0f);
            labelY.GetComponent<TMPro.TextMeshProUGUI>().text = (yMinimum + i * interval).ToString();
        } */
    }

    public void DrawLabel()
    {
        if (xSeparator != null)
            for (int i = 0; i < xSeparator.Length; i++)
            {
                DestroyImmediate(xSeparator[i]);
            }

        if (ySeparator != null)
            for (int i = 0; i < ySeparator.Length; i++)
            {
                DestroyImmediate(ySeparator[i]);
            }

        xSeparator = new GameObject[xSepCount];
        /* ySeparator = new GameObject[ySepCount];
        for (int i = 0; i < xSepCount; i++)
        {
            xSeparator[i] = Instantiate(labelTemplateX.gameObject);
            xSeparator[i].SetActive(true);
            xSeparator[i].transform.SetParent(background, false);
        }
        for (int i = 0; i < ySepCount; i++)
        {
            ySeparator[i] = Instantiate(labelTemplateY.gameObject);
            ySeparator[i].SetActive(true);
            ySeparator[i].transform.SetParent(background, false);

        } */
    }

    public void onShow()
    {
        bankingActivity.Clear();
        amountList.Clear();
        AmountText.text = "$" + Profile.getAmountInBank();
    }

    public void Init()
    {
        //currentBalance = Profile.getAmountInBank();
        //for (int i = 0; i < BankDataManager.GetBankActivityCount() - 1; i++)
        //{
        //    currentBalance += BankDataManager.GetBankActivity(i).amount;
        //}
    }

    public void WriteActivityToDb(BankingHistory data)
    {
        List<AppDataManager.DataDesc> dataDescList = new List<AppDataManager.DataDesc>();
        dataDescList.Add(new BankDataManager.BankDataDesc(-1, data.activityAmount.ToString(), data.activitySummary, data.balance.ToString()));
        AppDataManager.SetData(AppDataManager.Protocol.BANK_WRITE_TO_HISTORY, dataDescList);
    }

    public void ClearBankActList()
    {
        for (; BankActList.Count > 0; )
        {
            GameObject go = BankActList[BankActList.Count - 1];
            BankActList.RemoveAt(BankActList.Count - 1);
            DestroyImmediate(go);
        }
    }
}
