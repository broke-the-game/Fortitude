using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;
using UnityEngine.UI;

public class StartingPageControl : MonoBehaviour
{
    [SerializeField]
    GameObject ContinueButton;

    [SerializeField]
    GameObject ChoosingPanel;

    [SerializeField]
    TMPro.TextMeshProUGUI ChoosingPanelInstruction, BasicInfo, GoalnChallenge, InfoTitle;

    [SerializeField]
    ButtonControl[] Buttons;

    [SerializeField]
    RectTransform ContentPanel;

    [SerializeField]
    string FirstPlayInstruction;
    [SerializeField]
    string PlayAgainInstruction;

    ProfileEntity CurrentProfile;

    Coroutine infoPageCoroutine;

    [SerializeField]
    float m_lerpSpeed = 10f, m_lerpThreshold = 0.997f;

    Vector3 InfoCenterPos;
    Vector3 InfoDownPos => InfoCenterPos - ContentPanel.rect.height * ContentPanel.up;

    public static StartingPageControl Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        ContinueButton.SetActive(System.IO.File.Exists(Application.persistentDataPath + "/Fortitude.db"));
        InfoCenterPos = ContentPanel.localPosition;
        ContentPanel.gameObject.SetActive(false);
    }

    public void GoToProfileChoosingPage()
    {
        ChoosingPanel.SetActive(true);
        if (System.IO.File.Exists(Application.persistentDataPath + "/Fortitude.db"))
        {
            ChoosingPanelInstruction.text = PlayAgainInstruction;
        }
        else
        {
            ChoosingPanelInstruction.text = FirstPlayInstruction;
        }
    }

    public void GoToContinuePage()
    {
        ChoosingPanel.SetActive(false);

    }

    public void InitializeButtons()
    {
        ProfileEntity[] profileList = StartingModule.Instance.ProfileList;
        int length = Mathf.Min(profileList.Length, Buttons.Length);
        for (int i = 0; i < length; i++)
        {
            Buttons[i].Button.interactable = true;
            Buttons[i].SetContent(int.Parse(profileList[i].GetField(ProfileEntity.DataField.ID)), ProfileEntity.GetColor(profileList[i].GetField(ProfileEntity.DataField.Color)), profileList[i].GetField(ProfileEntity.DataField.Name));
        }
    }

    public void ShowProfileInfo(ProfileEntity profile)
    {
        CurrentProfile = profile;
        UpdateView();
        if (infoPageCoroutine != null)
            StopCoroutine(infoPageCoroutine);
        infoPageCoroutine = StartCoroutine(showInfo());
    }

    public void HideProfileInfo()
    {
        if (infoPageCoroutine != null)
            StopCoroutine(infoPageCoroutine);
        infoPageCoroutine = StartCoroutine(hideInfo());
    }

    IEnumerator showInfo()
    {
        float progress = 0f;
        ContentPanel.gameObject.SetActive(true);

        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, Time.deltaTime * m_lerpSpeed);
            ContentPanel.localPosition = Vector3.Lerp(InfoDownPos, InfoCenterPos, progress);
            if (progress > m_lerpThreshold)
            {
                ContentPanel.localPosition = InfoCenterPos;
                yield break;
            }
            yield return null;
        }
    }

    IEnumerator hideInfo()
    {
        float progress = 0f;
        ContentPanel.gameObject.SetActive(true);

        while (true)
        {
            progress = Mathf.Lerp(progress, 1f, Time.deltaTime * m_lerpSpeed);
            ContentPanel.localPosition = Vector3.Lerp(InfoCenterPos, InfoDownPos, progress);
            if (progress > m_lerpThreshold)
            {
                ContentPanel.localPosition = InfoDownPos;
                ContentPanel.gameObject.SetActive(false);
                yield break;
            }
            yield return null;
        }
    }

    public void UpdateView()
    {
        string basicInfo = "";
        string value;
        string subHead;
        InfoTitle.text = CurrentProfile.GetField(ProfileEntity.DataField.Name);
        for (int i = ProfileEntity.BASIC_START; i <= ProfileEntity.BASIC_END; i++)
        {
            if (CurrentProfile.TryGetField((ProfileEntity.DataField)i, out value))
            {
                subHead = typeof(ProfileEntity.DataField).GetEnumName(i);
                basicInfo = basicInfo +  "<b>" + subHead + ":</b> " + value + "\n";
            }
        }
        BasicInfo.text = basicInfo;

        string goal = CurrentProfile.GetField(ProfileEntity.DataField.Goal);
        string challenges = CurrentProfile.GetField(ProfileEntity.DataField.Challenges);
        GoalnChallenge.text = "<b><size=+2>Goal:</size></b>\n" + Utility.FormatData(goal) + "<line-height=10%>\n </line-height>\n<b><size=+2>Challenges:</size></b>\n" + Utility.FormatData(challenges);
    }

    public void OnSelect()
    {
        StartingModule.Instance.StartANewProfile(int.Parse(CurrentProfile.GetField(ProfileEntity.DataField.ID)));
    }
}
