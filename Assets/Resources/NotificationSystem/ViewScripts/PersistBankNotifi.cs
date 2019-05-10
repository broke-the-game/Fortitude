using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using AudioManaging;

[RequireComponent(typeof(Button))]
public class PersistBankNotifi : MonoBehaviour
{
    float amountInBank;

    [SerializeField]
    RectTransform UpArrow, DownArrow;

    [SerializeField]
    RectTransform Mask, MaskParent;

    [SerializeField]
    float range;

    [SerializeField]
    Color Ok, Reach, Below;

    private void Start()
    {
        StartCoroutine(wait4SceneLoad());
        UpArrow.gameObject.SetActive(false);
        DownArrow.gameObject.SetActive(false);
    }

    IEnumerator wait4SceneLoad()
    {
        while (!BankOperations.Instance)
            yield return null;
        BankOperations.Instance.OnBankAmountUpdated += updateAmount;
        updateAmount(0f);
    }

    public void OpenApp()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);

        if (ViewController.Instance)
        {
            ViewController.Instance.OpenApp(Utility.App.Bank);
        }
    }

    void updateAmount(float amt)
    {
        UpArrow.gameObject.SetActive(false);
        DownArrow.gameObject.SetActive(false);
        if (amt > 0f)
        {
            UpArrow.gameObject.SetActive(true);
        }
        if (amt < 0f)
        {
            DownArrow.gameObject.SetActive(true);
        }
        amountInBank = Profile.Instance.getAmountInBank();
        if (amountInBank < 0f)
        {
            Mask.GetComponent<Image>().color = Below;
        }
        if (amountInBank > 0f)
        {
            Mask.GetComponent<Image>().color = Reach;
        }
        if (amountInBank == 0f)
        {
            Mask.GetComponent<Image>().color = Ok;
        }
        Mask.sizeDelta = new Vector2( Mask.sizeDelta.x, MaskParent.rect.height / (range * 2f)* (amountInBank + range));
    }
}
