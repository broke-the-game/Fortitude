using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
[ExecuteAlways]
public class TextSizeControl : MonoBehaviour
{
    private TMPro.TextMeshProUGUI textObj { get => GetComponent<TMPro.TextMeshProUGUI>(); }
    private string defaultText;

    private RectTransform rt;

    private float prevWidth = 0;
    private float prevHeight = 0;

    // Start is called before the first frame update
    void Start()
    {
        textObj.enableWordWrapping = false;
        textObj.overflowMode = TMPro.TextOverflowModes.Overflow;
        textObj.autoSizeTextContainer = false;
        defaultText = textObj.text;

        rt = GetComponent<RectTransform>();

        Shorten();
    }
    public void UpdateDefaultText()
    {
        defaultText = textObj.text;
    }
    // Update is called once per frame
    void Update()
    {
        Shorten();
    }

    public void Shorten()
    {
        //find best font size
        if (rt.sizeDelta.y != prevHeight)
        {
            while (textObj.preferredHeight < rt.rect.height && textObj.fontSize < 200)
                textObj.fontSize++;

            while (textObj.preferredHeight > rt.rect.height)
                textObj.fontSize = textObj.fontSize - 1;
        }

        if (rt.sizeDelta.x != prevWidth || rt.rect.height != prevHeight)
        {
            float currWidth = textObj.preferredWidth;
            float maxWidth = rt.rect.width;

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

            prevHeight = rt.rect.height;
            prevWidth = rt.rect.width;
        }
    }
}
