using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    #region Singleton
    public static EndingManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    #endregion

    public void EndPlaythrough()
    {
        int result = PlayerPrefs.GetInt("Ended");
        if (result < 1)
        {
            int numOfHelp = Profile.Instance.getNumberOfMembers();
            BankOperations.Instance.updateAmount(numOfHelp * 0.55f, "You get help from your circle");
        }
        PlayerPrefs.SetInt("Ended", 1);
        SettingAppController.Instance.EndGame(Profile.Instance.getAmountInBank() >= 0f);
        if (NotificationController.Instance)
        {
            GameEnd_5_NotificationData data = NotificationController.Instance.CreateDataInstance(Utility.App.Setting, "GameEnd") as GameEnd_5_NotificationData;
            NotificationController.Instance.PushNotification(data, "GameEnd");
        }
    }
}
