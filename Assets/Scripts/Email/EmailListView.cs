using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmailListView : MonoBehaviour
{
    public RectTransform RectTransform => (RectTransform)transform;

    [SerializeField]
    Transform emailSlotsContainer;

    [SerializeField]
    GameObject EmailSlotPrefab;

    List<EmailSlotControl> emailSlots = new List<EmailSlotControl>();

    public void UpdateView()
    {
        int[] exeKeys = EmailAppController.Instance.EmailExeManager.GetKeyList();
        int slotCount = 0;
        EmailExecutionObj exeObj;
        for (int i = 0; i < exeKeys.Length; i++)
        {
            if (slotCount > emailSlots.Count - 1)
            {
                emailSlots.Add(Instantiate(EmailSlotPrefab, emailSlotsContainer).GetComponent<EmailSlotControl>());
            }
            EmailAppController.Instance.EmailExeManager.TryGetEmailExe(exeKeys[i], out exeObj);
            emailSlots[slotCount].SetInfo(exeObj.Subject, exeObj.WithWho, exeObj.Content, false, exeObj.Situation_Id);
            slotCount++;
        }

        int[] dataKeys = EmailAppController.Instance.EmailDataManager.GetKeyList();
        bool found;
        EmailDataManager.EmailGroupInfo emailGroupInfo;
        for (int i = 0; i < dataKeys.Length; i++)
        {
            found = false;
            for (int j = 0; j < exeKeys.Length; j++)
            {
                if (emailSlots[j].SituationId == dataKeys[i])
                {
                    found = true;
                    break;
                }
            }
            if (found)
                continue;
            emailGroupInfo = EmailAppController.Instance.EmailDataManager.GetEmailGroup(dataKeys[i]);
            if (slotCount > emailSlots.Count - 1)
            {
                emailSlots.Add(Instantiate(EmailSlotPrefab, emailSlotsContainer).GetComponent<EmailSlotControl>());
            }
            emailSlots[slotCount].SetInfo(emailGroupInfo.Subject, emailGroupInfo.Sender, emailGroupInfo.GetEmailInfo(emailGroupInfo.GetEmailCount() - 1).content, true, emailGroupInfo.SituationId);
            slotCount++;
        }

        EmailSlotControl slotToDelete;
        for (; emailSlots.Count > slotCount;)
        {
            slotToDelete = emailSlots[emailSlots.Count - 1];
            emailSlots.RemoveAt(emailSlots.Count - 1);
            DestroyImmediate(slotToDelete.gameObject);
        }
    }

    public void OnShowBeforeTransition()
    {
        EmailAppController.Instance.RequestLatestEmails();
        UpdateView();
    }
    public void OnHide()
    {

    }
}
