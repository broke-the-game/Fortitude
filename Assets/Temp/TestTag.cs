using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestTag : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI Text;

    public string inputText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetText()
    {
        if(inputText != null)
        Text.text = Utility.FormatData(inputText);
        Text.gameObject.SetActive(false);
        Text.gameObject.SetActive(true);

    }
}
