using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using AudioManaging;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLinkClick : MonoBehaviour, IPointerClickHandler
{
    TextMeshProUGUI textMesh => GetComponent<TextMeshProUGUI>();

    public void OnPointerClick(PointerEventData eventData)
    {
        Vector3 position2 = eventData.position;

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(textMesh, position2, Camera.main);
        Debug.Log(linkIndex);
        if (linkIndex != -1)
        { // was a link clicked?
            TMP_LinkInfo linkInfo = textMesh.textInfo.linkInfo[linkIndex];
            
            // open the link id as a url, which is the metadata we added in the text field
            Application.OpenURL(linkInfo.GetLinkID());
            Debug.Log("Link Clicked: "+ linkInfo.GetLinkID());
            AudioManager.Instance.Play(AudioEnum.Button_Default);
        }
    }
}