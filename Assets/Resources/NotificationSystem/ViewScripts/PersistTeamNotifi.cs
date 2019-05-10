using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioManaging;

public class PersistTeamNotifi : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI memberCount;

    private void Start()
    {
        StartCoroutine(wait4SceneLoad());
    }

    IEnumerator wait4SceneLoad()
    {
        while (!TeamOperations.Instance)
            yield return null;
        TeamOperations.Instance.OnTeamMemberUpdated += updateMemberCount;
        updateMemberCount();
    }

    void updateMemberCount()
    {
        memberCount.text = Profile.Instance.getNumberOfMembers().ToString();
    }
    public void OpenApp()
    {
        AudioManager.Instance.Play(AudioEnum.Button_Default);

        if (ViewController.Instance)
        {
            ViewController.Instance.OpenApp(Utility.App.Team);
        }
    }
}