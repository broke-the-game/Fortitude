using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResizeTextHeader : MonoBehaviour
{
    private RectTransform icon;
    private RectTransform header;
    [SerializeField]
    private Text text;
    [SerializeField] private TextProgression prog;

    [SerializeField] private float defaultScreenHeight;

    // Start is called before the first frame update
    void Start()
    {
        header = this.GetComponent<RectTransform>();
        icon = transform.GetChild(0).GetComponent<RectTransform>();
        //text = transform.GetChild(1).GetComponent<Text>();

        float screenHeight = transform.parent.parent.GetComponent<RectTransform>().rect.height;
        float headerSize = header.sizeDelta.y * screenHeight / defaultScreenHeight;
        header.sizeDelta = new Vector2(header.sizeDelta.x, headerSize);
        //icon.sizeDelta = new Vector2(headerSize, headerSize);
        text.text = "";
        StartCoroutine(SetTexter());
    }

    IEnumerator SetTexter() //displays the name of who is texting the player
    {
        //while (prog.firstText.AppExecution == null || !prog.showConversation)
        //{
        //    yield return null;
        //}

        //TextMsgObj firstTextObj = prog.GetText(prog.firstText);
        text.text = prog.GetCurrSpeaker();
        yield return null;
    }
}
