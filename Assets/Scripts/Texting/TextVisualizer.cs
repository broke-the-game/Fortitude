using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextVisualizer : MonoBehaviour
{
    //aspect ratio values
    [HideInInspector] public float screenWidth;
    [HideInInspector] public float screenHeight;

    //scrolling values
    //[HideInInspector] public RectTransform rt;
    //public float minScroll = 0;
    //public float maxScroll = 0;
    public RectTransform panel;
    public RectTransform header;

    public ScrollRect scroll;

    //text locations
    [HideInInspector] public float vertSpace; //vertical space between two consecutive texts
    [HideInInspector] public float indent; //horizontal space between text edge and closest edge of screen

    //typing bubble
    public float typingBubbleSizeConstant; //screen width / typing bubble height; keeps bubble the correct size

    //text progression
    private TextProgression textProgression => GetComponent<TextProgression>();

    //panel collapsing
    public float panelCollapseTime;
    public float defaultPanelSize;

    [SerializeField]
    float smoothTime = 0.3f;

    [SerializeField]
    RectTransform Header;

    //Text Bubble Data Structure
    List<GameObject> BubbleList = new List<GameObject>();

    Coroutine PageScrollingCoroutine;

    public void Init()
    {
        textProgression.nextTextLoc = -Header.rect.height * 1.2f;
        textProgression.endLoc = new Vector2(0f, textProgression.nextTextLoc);
        Utility.SetScroll(scroll, 0f);
        SetPanelDown();
        textProgression.HideButtons();
    }

    // Start is called before the first frame update
    void Start()
    {

        //rt = GetComponent<RectTransform>();
        screenWidth = transform.parent.GetComponent<RectTransform>().rect.width;
        screenHeight = transform.parent.GetComponent<RectTransform>().rect.height;

        //Init();

        textProgression.moveSpeed = screenHeight / textProgression.screenTravelTime;

        //TestMessageLoading();
    }

    // Update is called once per frame
    void Update()
    {
        //float y = Mathf.Clamp(rt.localPosition.y, minScroll, maxScroll);
        //rt.localPosition = new Vector3(rt.localPosition.x, y, 0f);
    }

    public void SetUpTypingBubble(RectTransform typingBubble)
    {
        float heightWidthRatio = typingBubble.sizeDelta.y / typingBubble.sizeDelta.x;
        if (screenWidth == 0)
        {
            screenWidth = transform.parent.GetComponent<RectTransform>().rect.width;
            screenHeight = transform.parent.GetComponent<RectTransform>().rect.height;
        }
        float height = screenWidth / typingBubbleSizeConstant;
        float width = height / heightWidthRatio;
        typingBubble.sizeDelta = new Vector2(width, height);
    }
    //    //find indent and vertical spacing values
    //    while (transform.childCount < 2) //wait until a text object has been created
    //    {
    //        yield return null;
    //    }

    //    yield return null;
    //    TextMessageController bubble = transform.GetChild(1).GetComponent<TextMessageController>();
    //    vertSpace = bubble.vertSpace;
    //    indent = bubble.edgeSpace;

    //    yield return null;
    //}

    public void SetTextBubbleAppearance(GameObject bubble, TextMessageController bubbleController)
    {
        bubble.transform.localScale = new Vector3(1f, 1f, 1f);
        bubbleController.ResizeBubble();

        if (vertSpace == 0)
            vertSpace = bubbleController.vertSpace;

        if (indent == 0)
            indent = bubbleController.edgeSpace;
    }

    IEnumerator scrollToBottom()
    {
        float defaultPos = scroll.verticalNormalizedPosition;
        float yVelocity = 0.0f;
        while (true)
        {
            scroll.verticalNormalizedPosition = Mathf.Lerp(scroll.verticalNormalizedPosition, 0f, 15f * Time.deltaTime);
            if (scroll.verticalNormalizedPosition < 0.002f)
            {
                scroll.verticalNormalizedPosition = 0f;
                yield break;
            }
            yield return null;
        }
    }

    public void AdjustScrolling(Vector2 endLoc, float height, bool inertia = true)
    {
        //advance how far the player can scroll
        float maxScroll = -endLoc.y + height + vertSpace + panel.rect.height; /*+ (screenHeight / 2f);*/

        maxScroll = Mathf.Max(((RectTransform)(scroll.transform)).rect.height, maxScroll);
        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, maxScroll);
        if (PageScrollingCoroutine != null)
            StopCoroutine(PageScrollingCoroutine);
        if (inertia)
            PageScrollingCoroutine = StartCoroutine(scrollToBottom());
        else
            Utility.SetScroll(scroll, 0f);
    }

    public void SetPanelDown()
    {
        panel.sizeDelta = new Vector2(panel.sizeDelta.x, 0f);
    }

    public IEnumerator CollapsePanel(bool collapsing)
    {
        float startTime = Time.time;
        //float prevPanelSize;
        //Debug.Log("Starting collapse at " + Time.time);
        
        while (startTime + panelCollapseTime > Time.time)
        {
            float prog = (Time.time - startTime) / panelCollapseTime;

            //change panel size
            if (collapsing)
                panel.sizeDelta = new Vector2(panel.sizeDelta.x, defaultPanelSize * (1 - prog));
            else
                panel.sizeDelta = new Vector2(panel.sizeDelta.x, defaultPanelSize * prog);

            //adjust max scrolling
            AdjustScrolling(textProgression.endLoc, 
                transform.GetChild(transform.childCount - 1).GetComponent<TextMessageController>().bubbleHeight, false);

            //avoid panel covering texts
            //if (!collapsing)
            //    rt.localPosition = new Vector3(rt.localPosition.x, maxScroll, 0);
            //if (!textProgression.NewTextVisible(textProgression.endLoc.y))
            //    rt.localPosition = new Vector3(rt.localPosition.x, maxScroll, 0);

            yield return null;
        }

        //make sure panel ending size is correct
        if (collapsing)
        {
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, 0);
        }
        else
        {
            panel.sizeDelta = new Vector2(panel.sizeDelta.x, defaultPanelSize);
        }

        //adjust max scrolling
        AdjustScrolling(textProgression.endLoc,
            transform.GetChild(transform.childCount - 1).GetComponent<TextMessageController>().bubbleHeight);

        yield return null;
    }


    //ignore this function. it's just for testing purposes
    //void TestMessageLoading()
    //{
    //    Dictionary<string, List<TextDataManager.Conversation>> dataHashMap = new Dictionary<string, List<TextDataManager.Conversation>>();

    //    List<TextDataManager.Conversation> conversation = new List<TextDataManager.Conversation>();

    //    TextDataManager.Conversation message1 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.NPC, "Oh my god did you hear about Brittney??? ", 0);
    //    TextDataManager.Conversation message2 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.Player, "", 1);
    //    TextDataManager.Conversation message3 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.Player, "OMG yes!", 2);
    //    TextDataManager.Conversation message4 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.Player, "Sooo gross lmfao", 3);
    //    TextDataManager.Conversation message5 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.NPC, "It makes me wonder if man has a higher purpose in this grand universe. In the eyes of whatever god may live, are we not just ants on an infinite plane? Are we not replaceable and miniscule?", 4);
    //    TextDataManager.Conversation message6 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.Player, "wtf?", 5);
    //    TextDataManager.Conversation message7 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.NPC, "Philosophers have pondered it for millenia, but what grand importance makes them powerful enough to decide the ways of all of creation? What gives them the right to such arrogance? We are but atoms, Erika. To assume otherwise is a lunatic's dream.", 6);
    //    TextDataManager.Conversation message8 = new TextDataManager.Conversation(TextDataManager.Conversation.Sender.Player, "You're like so lucky you're pretty or I wouldn't be friends with you.", 7);

    //    conversation.Add(message1);
    //    conversation.Add(message2);
    //    conversation.Add(message3);
    //    conversation.Add(message4);
    //    conversation.Add(message5);
    //    conversation.Add(message6);
    //    conversation.Add(message7);
    //    conversation.Add(message8);

    //    dataHashMap["existentialism"] = conversation;

    //    RecallConversation(dataHashMap, "existentialism");
    //}

    public void UpdateHistoryMessagesView(string sender)
    {
        //create bubbles with history
        int messageLength = TextAppController.Instance.TextDataManager.GetMessageCount(sender);
        TextDataManager.Conversation conversation;
        GameObject bubble = null;
        for (int i = 0; i < messageLength; i++)
        {
            conversation = TextAppController.Instance.TextDataManager.GetMessage(sender, i);
            switch (conversation.speaker)
            {
                case TextDataManager.Conversation.Sender.Player:
                    bubble = Instantiate(textProgression.sentText);
                    AddBubble(bubble);
                    break;
                case TextDataManager.Conversation.Sender.NPC:
                    bubble = Instantiate(textProgression.receivedText);
                    AddBubble(bubble);
                    break;
            }
            if (bubble == null)
                continue;
            bubble.transform.SetParent(transform);
            bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().text = conversation.message;
            TextMessageController bubbleController = bubble.GetComponent<TextMessageController>();
            SetTextBubbleAppearance(bubble, bubbleController);

            if (transform.childCount < 3) //no texts created except the one that was just created
            {
                Debug.Log("--------------------------------------------------------------");
                Debug.Log(textProgression.nextTextLoc);
                //textProgression.nextTextLoc -= (vertSpace + header.sizeDelta.y);
                Debug.Log(textProgression.nextTextLoc);
                Debug.Log("~");
                Debug.Log(vertSpace);
                Debug.Log(header.sizeDelta.y);
                Debug.Log("--------------------------------------------------------------");
            }

            //move the bubble to the correct location
            bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, textProgression.nextTextLoc);

            //switch (conversation.speaker)
            //{
            //    case TextDataManager.Conversation.Sender.Player:
            //        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, textProgression.nextTextLoc);
            //        //bubble.transform.localPosition = (new Vector2(screenWidth / 2f, textProgression.nextTextLoc));
            //        break;
            //    case TextDataManager.Conversation.Sender.NPC:
            //        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, textProgression.nextTextLoc);
            //        //bubble.transform.localPosition = (new Vector2(-screenWidth / 2f, textProgression.nextTextLoc));
            //        break;
            //}
            textProgression.endLoc = bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition;

            textProgression.nextTextLoc -= (bubbleController.bubbleHeight + vertSpace);

        }

        //create bubble with TxtMsgObj
        string[] oldMessages = null;
        if (TextAppController.Instance.TextMsgObjManager.TryGetOldMessages(sender, out oldMessages))
        {
            for (int i = 0; i < oldMessages.Length; i++)
            {
                bool isPlayerSpeaking;
                if (!TextAppController.Instance.TextMsgObjManager.TryGetIsPlayerSpeaking(sender, out isPlayerSpeaking))
                    continue;
                if (isPlayerSpeaking)
                {
                    bubble = Instantiate(textProgression.sentText);
                    AddBubble(bubble);
                }
                else
                {
                    bubble = Instantiate(textProgression.receivedText);
                    AddBubble(bubble);
                }
                //switch (conversation.speaker)
                //{
                //    case TextDataManager.Conversation.Sender.Player:
                //        bubble = Instantiate(textProgression.sentText);
                //        AddBubble(bubble);
                //        break;
                //    case TextDataManager.Conversation.Sender.NPC:
                //        bubble = Instantiate(textProgression.receivedText);
                //        AddBubble(bubble);
                //        break;
                //}
                if (bubble == null)
                    continue;

                bubble.transform.SetParent(transform);
                bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().text = oldMessages[i];
                TextMessageController bubbleController = bubble.GetComponent<TextMessageController>();
                SetTextBubbleAppearance(bubble, bubbleController);

                if (transform.childCount < 3) //no texts created except the one that was just created
                {
                    Debug.Log("--------------------------------------------------------------");
                    Debug.Log(textProgression.nextTextLoc);
                    //textProgression.nextTextLoc -= (vertSpace + header.sizeDelta.y);
                    Debug.Log(textProgression.nextTextLoc);
                    Debug.Log("~");
                    Debug.Log(vertSpace);
                    Debug.Log(header.sizeDelta.y);
                    Debug.Log("--------------------------------------------------------------");
                }

                //move the bubble to the correct location
                bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, textProgression.nextTextLoc);

                //if (isPlayerSpeaking)
                //{
                //    bubble.transform.localPosition = (new Vector2(screenWidth / 2f, textProgression.nextTextLoc));
                //}
                //else
                //{
                //    bubble.transform.localPosition = (new Vector2(-screenWidth / 2f, textProgression.nextTextLoc));
                //}
                //switch (conversation.speaker)
                //{
                //    case TextDataManager.Conversation.Sender.Player:
                //        bubble.transform.localPosition = (new Vector2(screenWidth / 2f, textProgression.nextTextLoc));
                //        break;
                //    case TextDataManager.Conversation.Sender.NPC:
                //        bubble.transform.localPosition = (new Vector2(-screenWidth / 2f, textProgression.nextTextLoc));
                //        break;
                //}
                textProgression.endLoc = bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition;
                textProgression.nextTextLoc -= (bubbleController.bubbleHeight + vertSpace);
            }
        }
        if(transform.GetChild(transform.childCount - 1).GetComponent<TextMessageController>())
            AdjustScrolling(textProgression.endLoc, transform.GetChild(transform.childCount - 1).GetComponent<TextMessageController>().bubbleHeight, false);
    }

    //public void RecallConversation (Dictionary<string, List<TextDataManager.Conversation>> dataHashMap, string key)
    //{
    //    //find all relevant messages
    //    List<TextDataManager.Conversation> prevMessages = dataHashMap[key];

    //    foreach (TextDataManager.Conversation message in prevMessages)
    //    {
    //        if (message.message.Length > 0) { 

    //            //create the bubble object
    //            GameObject bubble = Instantiate(message.speaker == 
    //                TextDataManager.Conversation.Sender.Player ? 
    //                textProgression.sentText : textProgression.receivedText);
    //            bubble.transform.SetParent(transform);

    //            //give the bubble the correct appearance
    //            bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().text = message.message;
    //            TextMessageController bubbleController = bubble.GetComponent<TextMessageController>();
    //            SetTextBubbleAppearance(bubble, bubbleController);

    //            if (transform.childCount < 3) //no texts created except the one that was just created
    //            {
    //                Debug.Log("--------------------------------------------------------------");
    //                Debug.Log(textProgression.nextTextLoc);
    //                textProgression.nextTextLoc -= (vertSpace + header.sizeDelta.y);
    //                Debug.Log(textProgression.nextTextLoc);
    //                Debug.Log("~");
    //                Debug.Log(vertSpace);
    //                Debug.Log(header.sizeDelta.y);
    //                Debug.Log("--------------------------------------------------------------");
    //            }

    //            //move the bubble to the correct location
    //            bubble.transform.localPosition = (message.speaker == TextDataManager.Conversation.Sender.Player ?
    //                new Vector2(screenWidth / 2f, textProgression.nextTextLoc) :
    //                new Vector2(-screenWidth / 2f, textProgression.nextTextLoc));
    //            textProgression.endLoc = bubble.transform.localPosition;

    //            textProgression.nextTextLoc -= (bubbleController.bubbleHeight + vertSpace);

    //        }
    //    }

    //    AdjustScrolling(textProgression.endLoc,
    //            transform.GetChild(transform.childCount - 1).GetComponent<TextMessageController>().bubbleHeight);

    //    rt.localPosition = Vector3.down * maxScroll;
    //}

    public void AddBubble(GameObject bubble)
    {
        BubbleList.Add(bubble);
    }

    public void RemoveBubble(GameObject bubble)
    {
        BubbleList.Remove(bubble);
        DestroyImmediate(bubble.gameObject);
    }

    public void ClearAllBubbles()
    {
        for (int i = 0; i < BubbleList.Count; i++)
        {
            DestroyImmediate(BubbleList[i].gameObject);
        }
        BubbleList.Clear();
    }
}