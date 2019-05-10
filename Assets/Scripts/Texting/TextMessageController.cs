using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextMessageController : MonoBehaviour
{
    //parts of the text object
    private RectTransform top;
    private RectTransform middle;
    private RectTransform bottom;
    private RectTransform topLeft;
    private RectTransform topRight;
    private RectTransform bottomLeft;
    private RectTransform bottomRight;
    private RectTransform messageText;
    [SerializeField] private float defaultTopHeight; //height of "top" gameObject
    [SerializeField] private float defaultBottomHeight;
    [SerializeField] private float defaultLeftWidth; //width of left corners
    [SerializeField] private float defaultRightWidth;
    [SerializeField] private float defaultIndent; //space between edge of bubble and edge of text
    [SerializeField] private float defaultBubbleWidth;
    [SerializeField] private bool fromPlayer;
    private float topHeight;
    private float bottomHeight;
    private float leftWidth;
    private float rightWidth;
    private float indent;
    private float bubbleWidth;
    public float bubbleHeight;

    //values to be changed by aspect ratio
    [SerializeField] private float defaultScreenWidth;
    [SerializeField] private float defaultScreenHeight;
    [SerializeField] private float defaultEdgeSpace; //horizontal dist between edge of text bubble and edge of screen
    [SerializeField] private float defaultVertSpace; //vertical space between two sequential texts from different people
    private float screenHeight;
    private float screenWidth;
    [HideInInspector] public float edgeSpace;
    private float overlapSpace;
    [HideInInspector] public float vertSpace;

    //movement related variables
    [HideInInspector] public bool placed = false; //is this in the correct place?

    public RectTransform RectTransform => (RectTransform) transform;

    // Start is called before the first frame update
    void Start()
    {
        top = transform.GetChild(0).GetComponent<RectTransform>();
        middle = transform.GetChild(1).GetComponent<RectTransform>();
        bottom = transform.GetChild(2).GetComponent<RectTransform>();
        topLeft = transform.GetChild(3).GetComponent<RectTransform>();
        topRight = transform.GetChild(4).GetComponent<RectTransform>();
        bottomLeft = transform.GetChild(5).GetComponent<RectTransform>();
        bottomRight = transform.GetChild(6).GetComponent<RectTransform>();
        messageText = transform.GetChild(7).GetComponent<RectTransform>();
        bubbleHeight = messageText.GetComponent<Text>().preferredHeight + indent * 2f;
    }

    public void ResizeBubble () //changes the size of the text bubble
    {
        //in case start hasn't been called yet
        if (messageText == null) {
            top = transform.GetChild(0).GetComponent<RectTransform>();
            middle = transform.GetChild(1).GetComponent<RectTransform>();
            bottom = transform.GetChild(2).GetComponent<RectTransform>();
            topLeft = transform.GetChild(3).GetComponent<RectTransform>();
            topRight = transform.GetChild(4).GetComponent<RectTransform>();
            bottomLeft = transform.GetChild(5).GetComponent<RectTransform>();
            bottomRight = transform.GetChild(6).GetComponent<RectTransform>();
            messageText = transform.GetChild(7).GetComponent<RectTransform>();
            bubbleHeight = messageText.GetComponent<Text>().preferredHeight + indent * 2f;
        }

        //change size to match aspect ratio
        screenWidth = transform.parent.parent.GetComponent<RectTransform>().rect.width;
        screenHeight = transform.parent.parent.GetComponent<RectTransform>().rect.height;
        float k = screenWidth / defaultScreenWidth;
        float kh = screenHeight / defaultScreenHeight;
        edgeSpace = k * defaultEdgeSpace;
        vertSpace = kh * defaultVertSpace;
        topHeight = k * defaultTopHeight;
        bottomHeight = k * defaultBottomHeight;
        leftWidth = k * defaultLeftWidth;
        rightWidth = k * defaultRightWidth;
        bubbleWidth = k * defaultBubbleWidth;
        indent = k * defaultIndent;
        bubbleWidth = k * defaultBubbleWidth;
        messageText.GetComponent<Text>().fontSize = (int)(k * messageText.GetComponent<Text>().fontSize);
        overlapSpace = 2f * (bubbleWidth + edgeSpace) - screenWidth;

        //change size to adjust to amount of text
        bubbleWidth = Mathf.Clamp(messageText.GetComponent<Text>().preferredWidth + indent * 2f,
            leftWidth + rightWidth, bubbleWidth);

        top.sizeDelta = new Vector2(bubbleWidth - leftWidth - rightWidth, topHeight);
        bottom.sizeDelta = new Vector2(bubbleWidth - leftWidth - rightWidth, bottomHeight);
        topLeft.sizeDelta = new Vector2(leftWidth, topHeight);
        topRight.sizeDelta = new Vector2(rightWidth, topHeight);
        bottomLeft.sizeDelta = new Vector2(leftWidth, bottomHeight);
        bottomRight.sizeDelta = new Vector2(rightWidth, bottomHeight);

        bubbleHeight = messageText.GetComponent<Text>().preferredHeight + indent * 2f;
        middle.sizeDelta = new Vector2(bubbleWidth, bubbleHeight - topHeight - bottomHeight);
        messageText.sizeDelta = new Vector2(bubbleWidth - 2f * indent, bubbleHeight - indent * 2f);

        if (fromPlayer)
        {
            top.localPosition = Vector3.left * (edgeSpace + rightWidth);
            middle.localPosition = new Vector3(-edgeSpace, -topHeight, 0f);
            bottom.localPosition = new Vector3(-edgeSpace - rightWidth, bottomHeight - bubbleHeight, 0f);
            topLeft.localPosition = new Vector3(-edgeSpace - bubbleWidth + leftWidth, 0f, 0f);
            topRight.localPosition = new Vector3(-edgeSpace, 0f, 0f);
            bottomLeft.localPosition = new Vector3(-edgeSpace - bubbleWidth + leftWidth, bottomHeight - bubbleHeight, 0f);
            bottomRight.localPosition = new Vector3(-edgeSpace, bottomHeight - bubbleHeight, 0f);
            messageText.localPosition = new Vector3(-edgeSpace - indent, -indent, 0f);
        }
        else
        {
            top.localPosition = Vector3.right * (edgeSpace + leftWidth);
            middle.localPosition = new Vector3(edgeSpace, -topHeight, 0f);
            bottom.localPosition = new Vector3(edgeSpace + leftWidth, bottomHeight - bubbleHeight, 0f);
            topLeft.localPosition = new Vector3(edgeSpace, 0f, 0f);
            topRight.localPosition = new Vector3(edgeSpace + bubbleWidth - rightWidth, 0f, 0f);
            bottomLeft.localPosition = new Vector3(edgeSpace, bottomHeight - bubbleHeight, 0f);
            bottomRight.localPosition = new Vector3(edgeSpace + bubbleWidth - rightWidth, bottomHeight - bubbleHeight, 0f);
            messageText.localPosition = new Vector3(edgeSpace + indent, -indent, 0f);
        }

        //sometimes the preferred text height changes in the bubble resizing. This makes it right again
        if (bubbleHeight != messageText.GetComponent<Text>().preferredHeight + indent * 2f)
        {
            bubbleHeight = messageText.GetComponent<Text>().preferredHeight + indent * 2f;
            middle.sizeDelta = new Vector2(bubbleWidth, bubbleHeight - topHeight - bottomHeight);
            messageText.sizeDelta = new Vector2(bubbleWidth - 2f * indent, bubbleHeight - indent * 2f);

            if (fromPlayer)
            {
                bottom.localPosition = new Vector3(-edgeSpace - rightWidth, bottomHeight - bubbleHeight, 0f);
                bottomLeft.localPosition = new Vector3(-edgeSpace - bubbleWidth + leftWidth, bottomHeight - bubbleHeight, 0f);
                bottomRight.localPosition = new Vector3(-edgeSpace, bottomHeight - bubbleHeight, 0f);
            }
            else
            {
                bottom.localPosition = new Vector3(edgeSpace + leftWidth, bottomHeight - bubbleHeight, 0f);
                bottomLeft.localPosition = new Vector3(edgeSpace, bottomHeight - bubbleHeight, 0f);
                bottomRight.localPosition = new Vector3(edgeSpace + bubbleWidth - rightWidth, bottomHeight - bubbleHeight, 0f);
            }
        }

    }
}
