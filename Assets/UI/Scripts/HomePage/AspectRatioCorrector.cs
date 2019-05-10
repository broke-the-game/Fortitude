using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteAlways]
[RequireComponent(typeof(AspectRatioFitter))]
[RequireComponent(typeof(Image))]
public class AspectRatioCorrector : MonoBehaviour
{
    AspectRatioFitter AspectRatioFitter { get { return GetComponent<AspectRatioFitter>(); } }
    Image Image { get { return GetComponent<Image>(); } }
    // Start is called before the first frame update
    void Start()
    {
    }
    private void OnGUI()
    {

    }
    // Update is called once per frame
    void Update()
    {
        if(Image.sprite)
            AspectRatioFitter.aspectRatio = Image.sprite.rect.width / Image.sprite.rect.height;
    }
    IEnumerator afterStart()
    {
        yield return new WaitForEndOfFrame();
        AspectRatioFitter.aspectRatio = Image.sprite.rect.width / Image.sprite.rect.height;
    }
}
