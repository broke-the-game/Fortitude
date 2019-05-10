using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class MessagePreviewShortener : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textObj;
    private string defaultText;

    private RectTransform rt;

    private float prevWidth = 0;
    private float prevHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        textObj = GetComponent<TMPro.TextMeshProUGUI>();
        textObj.enableWordWrapping = false;
        textObj.overflowMode = TMPro.TextOverflowModes.Overflow;
        textObj.autoSizeTextContainer = false;
        defaultText = textObj.text;

        rt = GetComponent<RectTransform>();

        Shorten();
    }

    // Update is called once per frame
    void Update()
    {
        //may want to remove this from Update later, and instead call Shorten() whenever the text box changes size
        if (rt.sizeDelta.y != prevHeight || rt.sizeDelta.x != prevWidth)
            Shorten();
    }

    public void Shorten()
    {
        //find best font size
        if (rt.sizeDelta.y != prevHeight)
        {
            while (textObj.preferredHeight < rt.sizeDelta.y)
                textObj.fontSize++;

            while (textObj.preferredHeight > rt.sizeDelta.y && textObj.fontSize > 1)
                textObj.fontSize = textObj.fontSize - 1;
        }

        if (rt.sizeDelta.x != prevWidth || rt.sizeDelta.y != prevHeight)
        {
            float currWidth = textObj.preferredWidth;
            float maxWidth = rt.sizeDelta.x;

            //lengthen if necessary
            if (currWidth < maxWidth)
            {
                textObj.text = defaultText;
                currWidth = textObj.preferredHeight;
            }

            currWidth = textObj.preferredWidth;

            //shorten if necessary
            if (currWidth > maxWidth)
            {
                textObj.text += "...";
                while (currWidth > maxWidth && textObj.text.Length > 3)
                {
                    textObj.text = textObj.text.Substring(0, textObj.text.Length - 4);
                    textObj.text += "...";
                    currWidth = textObj.preferredWidth;
                }
            }

            prevHeight = rt.sizeDelta.y;
            prevWidth = rt.sizeDelta.x;
        }
    }
}
