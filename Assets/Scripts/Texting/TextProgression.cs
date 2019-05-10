using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManaging;

public class TextProgression : MonoBehaviour {

    public RectTransform rectTransform { get { return (RectTransform)transform; } }

    //public bool showConversation; //off by default, but having it on may make testing easier

    //big picture
    //private TextAppController textAppController;

    //text objects
    //public TextMsgObj firstText;
    //[HideInInspector] private TextMsgObj currText;
    private GameObject bubble;
    private GameObject prevBubble;

    //prefabs
    public GameObject sentText;
    public GameObject receivedText;

    //text movement values
    bool firstTextDisplayed = false;
    [HideInInspector] public float nextTextLoc;
    public float screenTravelTime; //time taken for a text to move from the bottom of the screen to the top
    [HideInInspector] public float moveSpeed; //speed at which new texts travel in pixels per second
    [HideInInspector] public Vector2 endLoc;

    //typing bubble
    public RectTransform typingBubble;
    [SerializeField] private float typingBubbleAppearTime; //time taken for typing bubble to appear on screen
    [SerializeField] private TextingProfile[] textingProfiles; //everyone's typing speeds in words per minute
    private float typingTimePlayer = 0; //time taken for player to type one CHARACTER
    private float typingTimeNPC = 0; //time taken for NPC to type one CHARACTER
    [SerializeField] private float bubbleDisappearTime; //if typing bubble disappears, it waits this long before appearing again
    [SerializeField] Text headText;
    public bool IsShow;

    public string CurrentSpeaker { get; private set; }

    //choice buttons
    [SerializeField] private GameObject choiceObj1;
    [SerializeField] private GameObject choiceObj2;

    [SerializeField]
    private Text choiceText1;
    [SerializeField]
    private Text choiceText2;
    [SerializeField]
    private Button choiceButton1;
    [SerializeField]
    private Button choiceButton2;
    private bool justMadeChoice = false;

    private bool needDecisionMaking = false;

    //visuals
    [SerializeField]
    public TextVisualizer textVisualizer;

    //Control Coroutine
    Coroutine showText;

    //show Progress
    //int currentIndex = 0;
    public TextMsgObjManager TextMsgObjManager { get { return TextAppController.Instance.TextMsgObjManager; } }

    void Start()
    {
        //textVisualizer = GetComponent<TextVisualizer>();
        //textAppController = transform.parent.GetComponent<TextAppController>();

        //choiceText1 = choiceObj1.transform.GetChild(0).gameObject.GetComponent<Text>();
        //choiceText2 = choiceObj2.transform.GetChild(0).gameObject.GetComponent<Text>();
        //choiceButton1 = choiceObj1.GetComponent<Button>();
        //choiceButton2 = choiceObj2.GetComponent<Button>();

        typingTimePlayer = GetTypingSpeed("Player");

        textVisualizer.SetUpTypingBubble(typingBubble);

        //if (!AppCallbackModule.Instance)
        //{
        //    currText = firstText;
        //    StartToShowText();
        //}
        //StartCoroutine(ShowText(currText));
    }

    IEnumerator ShowText()
    {
        bool wasOpened = false;
        while (true)
        {
            //wait for permission from TextAppController to show conversation
            //bool textWasNull = false;
            string thisMessage;
            bool isPlayerTalking;
            bool hasNextMessage = true;
            firstTextDisplayed = false;

            while (!TextMsgObjManager.TryGetCurrentMsg(CurrentSpeaker, out thisMessage) || (AppCallbackModule.Instance != null && !TextAppController.Instance.IsShowBeforeTransition) || needDecisionMaking)
            {
                yield return null;
                //textWasNull = true;
            }
            //TextMsgObj text = currText;

            //if (textWasNull)
            //    text = GetText(firstText);

            //process changes that affect player profile here

            //display messages
            if (typingTimeNPC == 0)
                typingTimeNPC = GetTypingSpeed(CurrentSpeaker);
            //for (int i = currentIndex; i < text.message.Length; i++)
            while (hasNextMessage)
            {
                if (bubble != null)
                    prevBubble = bubble;

                if (!TextMsgObjManager.TryGetIsPlayerSpeaking(CurrentSpeaker, out isPlayerTalking))
                    yield break;

                if (thisMessage.Length > 0)
                {
                    //create text bubble object
                    if (!isPlayerTalking)
                    {
                        bubble = Instantiate(receivedText);
                        bubble.transform.SetParent(this.transform);
                        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, nextTextLoc);
                        //bubble.transform.localPosition = new Vector2(textVisualizer.screenWidth, -textVisualizer.screenHeight / 2f);
                    }
                    else
                    {
                        bubble = Instantiate(sentText);
                        bubble.transform.SetParent(this.transform);
                        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, nextTextLoc);

                        //bubble.transform.localPosition = new Vector2(-textVisualizer.screenWidth, -textVisualizer.screenHeight / 2f);
                    }
                    textVisualizer.AddBubble(bubble);
                    bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().text = thisMessage;
                    yield return null;
                    TextMessageController bubbleController = bubble.GetComponent<TextMessageController>();
                    textVisualizer.SetTextBubbleAppearance(bubble, bubbleController);

                    //if (transform.childCount < 3) //no texts on screen yet other than the one we just created

                    //move bubble to the start position
                    if (isPlayerTalking)
                    {
                        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, nextTextLoc) - new Vector2(-textVisualizer.indent, textVisualizer.indent);

                        //bubble.transform.localPosition = new Vector2(textVisualizer.screenWidth / 2f, nextTextLoc)
                        //    - new Vector2(-textVisualizer.indent, textVisualizer.indent);
                    }
                    else
                    {
                        bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = new Vector2(0f, nextTextLoc) - new Vector2(textVisualizer.indent, textVisualizer.indent);

                        //bubble.transform.localPosition = new Vector2(-textVisualizer.screenWidth / 2f, nextTextLoc)
                        //    - new Vector2(textVisualizer.indent, textVisualizer.indent);
                    }

                    //make text invisible
                    Color bubbleColor = bubble.transform.GetChild(0).GetComponent<Image>().color;
                    Color textColor = bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().color;
                    SetBubbleOpacity(bubble, bubbleColor, textColor, 0);

                    //move bubble to correct location and state
                    Vector2 startLoc = bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition;
                    endLoc = new Vector2(0f, nextTextLoc);
                    //if (isPlayerTalking)
                    //    endLoc = new Vector3(textVisualizer.screenWidth / 2f, nextTextLoc);
                    //else
                    //    endLoc = new Vector3(-textVisualizer.screenWidth / 2f, nextTextLoc);

                    textVisualizer.AdjustScrolling(endLoc, typingBubble.sizeDelta.y);

                    //first text automatically on screen, others must appear over time
                    if (firstTextDisplayed || wasOpened)
                    {
                        float startTime;

                        //typing bubble appears if player didn't just make a choice
                        if (!justMadeChoice)
                        {
                            //typing bubble
                            typingBubble.anchoredPosition = endLoc;
                            Image typingBubbleImg = typingBubble.GetComponent<Image>();
                            typingBubbleImg.color = new Color(1, 1, 1, 0);

                            //flip depending on speaker
                            if (isPlayerTalking)
                                typingBubble.transform.localScale = new Vector3(-1f, 1f, 1f);
                            else
                                typingBubble.transform.localScale = new Vector3(1f, 1f, 1f);

                            //figure out if new typing bubble is visible to the player and go there if not
                            //bool zoomToBubble = !NewTextVisible(endLoc.y);
                            //float endScrollBubble = -endLoc.y + typingBubble.sizeDelta.y + textVisualizer.vertSpace +
                            //    textVisualizer.panel.sizeDelta.y - (textVisualizer.screenHeight / 2f);
                            //float startScrollBubble = textVisualizer.rt.localPosition.y;

                            //make typing bubble appear on screen
                            startTime = Time.time;
                            float endTime = startTime + typingBubbleAppearTime;
                            while (Time.time < endTime)
                            {
                                float prog = (Time.time - startTime) / (endTime - startTime);
                                if (isPlayerTalking)
                                    typingBubble.anchoredPosition = new Vector2(endLoc.x - textVisualizer.indent * prog, endLoc.y);
                                else
                                    typingBubble.anchoredPosition = new Vector2(endLoc.x + textVisualizer.indent * prog, endLoc.y);
                                typingBubbleImg.color = new Color(1, 1, 1, prog);

                                //if (zoomToBubble)
                                //{
                                //    float y = prog * endScrollBubble + (1 - prog) * startScrollBubble;
                                //    textVisualizer.rt.localPosition = new Vector3(textVisualizer.rt.localPosition.x, y, 0f);
                                //}

                                yield return null;
                            }
                            if (isPlayerTalking)
                                typingBubble.anchoredPosition = new Vector2(endLoc.x - textVisualizer.indent, endLoc.y);
                            else
                                typingBubble.anchoredPosition = new Vector2(endLoc.x + textVisualizer.indent, endLoc.y);
                            typingBubbleImg.color = new Color(1, 1, 1, 1);

                            //if (zoomToBubble)
                            //{
                            //    textVisualizer.rt.localPosition = new Vector3(textVisualizer.rt.localPosition.x, endScrollBubble, 0f);
                            //}

                            //typing bubble stays on screen
                            if (thisMessage.Length > 0)
                            {
                                if (isPlayerTalking)
                                    yield return new WaitForSeconds(typingTimePlayer * thisMessage.Length);
                                else
                                    yield return new WaitForSeconds(typingTimeNPC * thisMessage.Length);
                            }
                            else
                            {
                                yield return new WaitForSeconds(bubbleDisappearTime);
                                //textVisualizer.maxScroll -= typingBubble.sizeDelta.y;
                            }

                            //make typing bubble leave screen
                            startTime = Time.time;
                            endTime = startTime + typingBubbleAppearTime;
                            while (Time.time < endTime)
                            {
                                float prog = (Time.time - startTime) / (endTime - startTime);
                                if (isPlayerTalking)
                                    typingBubble.anchoredPosition = new Vector2(endLoc.x - textVisualizer.indent * (1 - prog), endLoc.y);
                                else
                                    typingBubble.anchoredPosition = new Vector2(endLoc.x + textVisualizer.indent * (1 - prog), endLoc.y);
                                typingBubbleImg.color = new Color(1, 1, 1, 1 - prog);
                                yield return null;
                            }
                            typingBubble.anchoredPosition = endLoc;
                            typingBubbleImg.color = new Color(1, 1, 1, 0);
                        }

                        //figure out if new text is visible to the player and go there if not
                        //bool zoomToText = !NewTextVisible(endLoc.y);
                        //float endScroll = -endLoc.y + bubbleController.bubbleHeight + textVisualizer.vertSpace +
                        //    textVisualizer.panel.sizeDelta.y - (textVisualizer.screenHeight / 2f);
                        //float startScroll = textVisualizer.rt.localPosition.y;

                        //advance how far the player can scroll
                        textVisualizer.AdjustScrolling(endLoc, bubbleController.bubbleHeight);

                        //bring on the text
                        if (isPlayerTalking)
                            AudioManager.Instance.Play(AudioEnum.Text_Sent);
                        else
                            AudioManager.Instance.Play(AudioEnum.Text_Received);
                        startTime = Time.time;
                        float appearTime = (textVisualizer.indent * Mathf.Sqrt(2f)) / moveSpeed; //time taken to appear

                        while (Time.time < startTime + appearTime)
                        {
                            float prog = (Time.time - startTime) / appearTime;
                            Vector2 pos = (1 - prog) * startLoc + prog * endLoc;
                            bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = pos;
                            SetBubbleOpacity(bubble, bubbleColor, textColor, prog);

                            //if (zoomToText)
                            //{
                            //    float y = prog * endScroll + (1 - prog) * startScroll;
                            //    textVisualizer.rt.localPosition = new Vector3(textVisualizer.rt.localPosition.x, y, 0f);
                            //}

                            yield return null;
                        }

                        //if (zoomToText)
                            //textVisualizer.rt.localPosition = new Vector3(textVisualizer.rt.localPosition.x, endScroll, 0f);

                    }

                    firstTextDisplayed = true;

                    //make sure bubble ends in the right place and state
                    bubble.GetComponent<TextMessageController>().RectTransform.anchoredPosition = endLoc;
                    SetBubbleOpacity(bubble, bubbleColor, textColor, 1);
                    bubbleController.placed = true;


                    bubble = null;
                    //currentIndex++;

                    //prepare next text

                    nextTextLoc -= (bubbleController.bubbleHeight + textVisualizer.vertSpace);
                }
                if (hasNextMessage = TextMsgObjManager.TryGetNextMsg(CurrentSpeaker, out thisMessage))
                {
                    if (NotificationController.Instance && !isPlayerTalking && thisMessage != "")
                    {
                        MessageFrom_0_NotificationData data = (MessageFrom_0_NotificationData)NotificationController.Instance.CreateDataInstance(Utility.App.Text, "MessageFrom");
                        data.FromWho = CurrentSpeaker;
                        data.Message = thisMessage;
                        NotificationController.Instance.PushNotification(data, CurrentSpeaker);
                    }
                    yield return new WaitForSeconds(bubbleDisappearTime);
                }
                else {
                    string message;
                    TextMsgObjManager.TryGetLastMessage(CurrentSpeaker, out message);
                    if (NotificationController.Instance && !isPlayerTalking && message != "")
                    {
                        MessageFrom_0_NotificationData data = (MessageFrom_0_NotificationData)NotificationController.Instance.CreateDataInstance(Utility.App.Text, "MessageFrom");
                        data.FromWho = CurrentSpeaker;
                        data.Message = message;
                        NotificationController.Instance.PushNotification(data, CurrentSpeaker);
                    }
                }
            }
            wasOpened = true;
            AdvanceTexts();
            yield return null;
        }
    }

    void SetBubbleOpacity(GameObject bubble, Color bubbleColor, Color textColor, float opacity)
    {
        for (int j = 0; j < bubble.transform.childCount - 1; j++)
        {
            bubble.transform.GetChild(j).GetComponent<Image>().color =
                new Color(bubbleColor.r, bubbleColor.g, bubbleColor.b, opacity);
        }
        bubble.transform.GetChild(bubble.transform.childCount - 1).GetComponent<Text>().color =
            new Color(textColor.r, textColor.g, textColor.b, opacity);
    }

    void AdvanceTexts()
    {
        if (AppCallbackModule.Instance)
        {
            //TextMsgObj currTextObj = GetText(currText);
            //List<TextMsgObj> futureTexts = ListFutureTexts(currTextObj);
            string[] nextMessage;
            AppCallback[] nextCallback;
            if (TextMsgObjManager.TryGetCallback(CurrentSpeaker, out nextMessage, out nextCallback))
            {
                if (nextCallback == null || nextCallback.Length == 0) //conversation ended
                {
                    //currText = null;
                    justMadeChoice = false;
                    if (NotificationController.Instance)
                    {
                        NotificationController.Instance.HideNotification(Utility.App.Text, CurrentSpeaker);
                    }
                    TextMsgObjManager.OnFinish(CurrentSpeaker);
                    return;
                }

                else if (nextCallback.Length == 1) //linear conversation
                {
                    //currText = GetText(currText.nextText[0]);
                    //currTextObj = GetText(currText);
                    justMadeChoice = false;
                    AppCallback NextExecution = nextCallback[0];
                    //currText = null;
                    //textAppController.ExecuteScriptableObject(GetText(currText));
                    TextMsgObjManager.OnFinish(CurrentSpeaker);
                    AppCallbackModule.Instance.Execute(NextExecution);
                    if (NotificationController.Instance && NextExecution.AppExecution == null)
                    {
                        NotificationController.Instance.HideNotification(Utility.App.Text, CurrentSpeaker);
                    }
                    //StartCoroutine(ShowText(currTextObj, false));
                }

                else if (nextCallback.Length == 2) //player makes a dialogue choice
                {
                    StartCoroutine(textVisualizer.CollapsePanel(false));
                    Invoke("ShowButtons", textVisualizer.panelCollapseTime);
                    needDecisionMaking = true;
                }
            }
        }
        else
        {
            //if (currText.nextText == null || currText.nextText.Length == 0) //conversation ended
            //{
            //    currText = null;
            //    return;
            //}

            //else if (currText.nextText.Length == 1) //linear conversation
            //{
            //    //currTextObj = GetText(currText);
            //    justMadeChoice = false;
            //    //textAppController.ExecuteScriptableObject(GetText(currText));
            //    //AppCallbackModule.Instance.Execute(currText.nextText[0]);
            //    //StartCoroutine(ShowText(currTextObj, false));
            //    //SetText(GetText(currText.nextText[0]));
            //    StartToShowText();
            //}

            //else if (currText.nextText.Length == 2) //player makes a dialogue choice
            //{
            //    StartCoroutine(textVisualizer.CollapsePanel(false));
            //    Invoke("ShowButtons", textVisualizer.panelCollapseTime);
            //    needDecisionMaking = true;
            //}
        }
    }

    //this function isn't currently being used
    //List<TextMsgObj> ListFutureTexts (TextMsgObj currTextObj)
    //{
    //    List<TextMsgObj> futureTexts = new List<TextMsgObj>();
    //    foreach (AppCallback call in currTextObj.nextText)
    //    {
    //        if (call != null &&
    //            call.AppExecution != null &&
    //            call.AppExecution.GetType() == typeof(TextMsgObj) &&
    //            call.AppExecution.ExecutingApp == Utility.App.Text)
    //            futureTexts.Add(GetText(call));
    //    }

    //    return futureTexts;
    //}

    public void ProcessChoice(int i)
    {
        if (AppCallbackModule.Instance)
        {
            //TextMsgObj currTextObj = GetText(currText);
            string[] nextMessage;
            AppCallback[] nextCallback;
            if (TextMsgObjManager.TryGetCallback(CurrentSpeaker, out nextMessage, out nextCallback))
            {
                choiceText1.text = "";
                choiceText2.text = "";
                choiceButton1.interactable = false;
                choiceButton2.interactable = false;
                justMadeChoice = true;
                //currText = currTextObj.nextText[i];
                TextMsgObjManager.OnFinish(CurrentSpeaker);
                //currTextObj = GetText(currText);
                AppCallback NextExecution = nextCallback[i];
                //currText = null;
                //textAppController.ExecuteScriptableObject(GetText(currText));
                AppCallbackModule.Instance.Execute(NextExecution);
                //StartCoroutine(ShowText(currTextObj, true));
            }
        }
        else
        {
            //choiceText1.text = "";
            //choiceText2.text = "";
            //choiceButton1.interactable = false;
            //choiceButton2.interactable = false;
            ////SetText(GetText(currText.nextText[i]));
            ////currTextObj = GetText(currText);
            //justMadeChoice = true;
            ////textAppController.ExecuteScriptableObject(currTextObj);
            ////AppCallbackModule.Instance.Execute(currText.nextText[i]);
            //StartToShowText();
        }
        AudioManager.Instance.Play(AudioEnum.Button_Default);
        needDecisionMaking = false;
        StartCoroutine(textVisualizer.CollapsePanel(true));
    }

    //public bool NewTextVisible(float pos) //takes in a y-value to see if that's in the player's field of view
    //{
    //    //float curr = textVisualizer.rt.localPosition.y;
    //    float max = textVisualizer.screenHeight / 2f - curr;
    //    float min = -textVisualizer.screenHeight / 2f - curr + 
    //        bubble.GetComponent<TextMessageController>().bubbleHeight + textVisualizer.panel.sizeDelta.y;
    //    return max >= pos && pos >= min;
    //}

    //public TextMsgObj GetText(AppCallback appCallback)
    //{
    //    return (TextMsgObj)appCallback.AppExecution;
    //}

    float GetTypingSpeed(string sender)
    {
        float wpm = 0;
        foreach (TextingProfile textingProfile in textingProfiles)
        {
            if (textingProfile.character == sender)
                wpm = textingProfile.typingWPM;
        }

        if (wpm == 0)
        {
            Debug.Log("ERROR: No typing speed set for " + sender);
            if (sender != "Player")
                return GetTypingSpeed("Player");
            else
                wpm = 120f;
        }

        return 60 / (wpm * 5.5f);
    }

    //public void SetText(TextMsgObj text)
    //{
    //    currText = text;
    //    currentIndex = 0;
    //}
    public void SetCurrentSpeaker(string speaker)
    {
        CurrentSpeaker = speaker;
    }
    public void StartToShowText()
    {
        if (showText != null)
            StopCoroutine(showText);
        showText = StartCoroutine(ShowText());
    }
    public void StopShowTexting()
    {
        if (showText != null)
            StopCoroutine(showText);
        showText = null;
        if (bubble)
            textVisualizer.RemoveBubble(bubble);
    }

    public string GetCurrSpeaker()
    {
        //if (currText)
        //{
        //    if (currText.playerTalking)
        //        return "Player";
        //    else
        //        return currText.speaker;
        //}
        //return "";
        return CurrentSpeaker;
    }

    public void HideButtons()
    {
        choiceButton1.interactable = false;
        choiceButton2.interactable = false;
    }

    public void ShowButtons()
    {
        choiceButton1.interactable = true;
        choiceButton2.interactable = true;
        string[] nextMessage;
        AppCallback[] nextCallback;
        if (TextMsgObjManager.TryGetCallback(CurrentSpeaker, out nextMessage, out nextCallback))
        {
            if (nextMessage == null)
                return;
            if (0 < nextMessage.Length)
                choiceText1.text = nextMessage[0];
            if (1 < nextMessage.Length)
                choiceText2.text = nextMessage[1];
        }
    }

    public void OnShow()
    {
        IsShow = true;
        TextAppController.Instance.RequestMessagesBySenders(new string[] { CurrentSpeaker });
        headText.text = Utility.ParseName(CurrentSpeaker);
        resetView();
        textVisualizer.UpdateHistoryMessagesView(CurrentSpeaker);
        string temp;
        if (!TextMsgObjManager.TryGetCurrentMsg(CurrentSpeaker, out temp) && TextMsgObjManager.TryGetLastMessage(CurrentSpeaker, out temp))
        {
            StartCoroutine(textVisualizer.CollapsePanel(false));
            Invoke("ShowButtons", textVisualizer.panelCollapseTime);
        }
        StartToShowText();
        Utility.SetScroll(textVisualizer.scroll, 0f);
    }

    private void resetView()
    {
        textVisualizer.ClearAllBubbles();
        textVisualizer.Init();
    }

    public void OnHide()
    {
        IsShow = false;
        //textVisualizer.ClearAllBubbles();
        StopShowTexting();
    }
}
