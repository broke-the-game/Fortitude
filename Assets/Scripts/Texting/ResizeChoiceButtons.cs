using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeChoiceButtons : MonoBehaviour
{
    [SerializeField] private float marginSize; //space between buttons/screen edge. 0 = none, 1 = screen width
    [SerializeField] private float maxHeight; //1 = screen height
    [SerializeField] private float minHeight;
    [SerializeField] private int index;

    [SerializeField] private float defaultWidth;
    [SerializeField] private bool button;

    // Start is called before the first frame update
    void Start()
    {
        int buttonCount = transform.parent.childCount - 1;

        float screenWidth = transform.parent.parent.parent.GetComponent<RectTransform>().rect.width;
        float screenHeight = transform.parent.parent.parent.GetComponent<RectTransform>().rect.height;

        marginSize = marginSize * screenWidth;
        maxHeight = maxHeight * screenHeight;
        minHeight = minHeight * screenHeight;

        float width = (screenWidth - ((buttonCount + 1) * marginSize)) / buttonCount;
        float height = Mathf.Clamp(width, minHeight, maxHeight);

        RectTransform rt = GetComponent<RectTransform>();

        if (button)
        {
            float x = (-screenWidth / 2f) + index * width + (index + 1) * marginSize;
            float y = (-screenHeight / 2f) + marginSize;

            rt.localPosition = new Vector3(x, y, 0);
            rt.sizeDelta = new Vector2(width, height);

            Text text = transform.GetChild(0).GetComponent<Text>();
            text.resizeTextMaxSize = (int)(text.fontSize * screenWidth / defaultWidth);
        }
        else //the panel behind the buttons
        {
            float panelHeight = height;
            rt.sizeDelta = new Vector2(rt.sizeDelta.x, panelHeight);
            PanelResizer.Instance.StartCoroutine(SetPanelHeight());
            //StartCoroutine(SetPanelHeight());
        }
    }

    private void OnEnable()
    {
        if (!button)
            StartCoroutine(SetPanelHeight());
    }

    public IEnumerator SetPanelHeight()
    {
        yield return new WaitForEndOfFrame();
        TextVisualizer textVisualizer = TextAppController.Instance.TextViewController.TextProg.textVisualizer;

        float height = transform.parent.GetChild(1).GetComponent<RectTransform>().sizeDelta.y;
        float panelHeight = height + marginSize;
        RectTransform rt = GetComponent<RectTransform>();
        if (textVisualizer != null)
            textVisualizer.defaultPanelSize = panelHeight;
        rt.sizeDelta = new Vector2(rt.sizeDelta.x, 0);
        //if (textVisualizer != null)
        //    textVisualizer.AdjustScrolling(textVisualizer.GetComponent <TextProgression>().endLoc,
        //        textVisualizer.transform.GetChild(textVisualizer.transform.childCount - 1).
        //        GetComponent<TextMessageController>().bubbleHeight);

        //textVisualizer.GetComponent<RectTransform>().localPosition = new Vector3(0, -textVisualizer.maxScroll, 0);

        yield return null;
    }

}
