using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteAlways]
[RequireComponent(typeof(TMPro.TextMeshProUGUI))]
public class ResizeFontWithLine : MonoBehaviour, Resizable
{
    [SerializeField]
    uint Lines;
    TMPro.TextMeshProUGUI text => GetComponent<TMPro.TextMeshProUGUI>();

    public void Resize()
    {
        text.fontSize = ((RectTransform)transform).rect.height / Lines * 0.893f;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Resize();
    }
}
