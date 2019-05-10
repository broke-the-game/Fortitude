using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class EmailExecutionManager : MonoBehaviour
{
    Dictionary<int, EmailExecutionObj> EmailExeList = new Dictionary<int, EmailExecutionObj>();

    public int[] GetKeyList()
    {
        return EmailExeList.Keys.ToArray();
    }

    public void AddExeObj(EmailExecutionObj exeObj)
    {
        EmailExeList.Add(exeObj.Situation_Id, exeObj);
    }

    public bool TryGetEmailExe(int situationId, out EmailExecutionObj emailExe)
    {
        return EmailExeList.TryGetValue(situationId, out emailExe);
    }

    public EmailExecutionObj GetEmailExe(int situationId)
    {
        EmailExecutionObj value = null;
        EmailExeList.TryGetValue(situationId, out value);
        return value;
    }

    public void OnFinish(int situationId)
    {
        EmailExecutionObj exeObj = GetEmailExe(situationId);
        writeToDatabase(exeObj);
        EmailExeList.Remove(situationId);
        EmailAppController.Instance.RequestLatestEmails();
    }

    private void writeToDatabase(EmailExecutionObj exeObj)
    {
        EmailDataManager.EmailDataDesc dataDesc = new EmailDataManager.EmailDataDesc(-1, exeObj.WithWho, exeObj.IsPlayerTalking, exeObj.Subject, exeObj.Content, exeObj.Situation_Id);
        List<AppDataManager.DataDesc> dataList = new List<AppDataManager.DataDesc>();
        dataList.Add(dataDesc);
        AppDataManager.SetData(AppDataManager.Protocol.EMAIL_WRITE_TO_HISTORY, dataList);
    }
}
