using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextMsgObjManager : MonoBehaviour
{
    Dictionary<string, TextMsgObjEncap> TextMsgObjList = new Dictionary<string, TextMsgObjEncap>();

    public class TextMsgObjEncap
    {
        public TextMsgObj textMsgObj;
        public int currentMsgIndex;
        public TextMsgObjEncap(TextMsgObj tmo)
        {
            textMsgObj = tmo;
            currentMsgIndex = 0;
        }
    }

    public void AddMsgObj(TextMsgObj textMsgObj)
    {
        TextMsgObjEncap temp = new TextMsgObjEncap(textMsgObj);
        if (TextMsgObjList.ContainsKey(textMsgObj.speaker))
        {
            TextMsgObjList[textMsgObj.speaker] = temp;
        }
        else
        {
            TextMsgObjList.Add(textMsgObj.speaker, temp);
        }
    }

    public void RemoveMsgObj(string speaker)
    {
        if (TextMsgObjList.ContainsKey(speaker))
        {
            TextMsgObjList.Remove(speaker);
        }
        else
        {
            Debug.LogError("Speaker: " + speaker + " not found");
        }
    }

    public bool TryGetNextMsg(string speaker, out string message)
    {
        message = null;
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            temp.currentMsgIndex++;
            if (temp.currentMsgIndex < temp.textMsgObj.message.Length)
            {
                message = temp.textMsgObj.message[temp.currentMsgIndex];
                return true;
            }
        }
        return false;
    }

    public bool TryGetCurrentMsg(string speaker, out string message)
    {
        message = null;
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            if (temp.currentMsgIndex < temp.textMsgObj.message.Length)
            {
                message = temp.textMsgObj.message[temp.currentMsgIndex];
                return true;
            }
        }
        return false;
    }

    public bool TryGetCallback(string speaker, out string[] nextMessages, out AppCallback[] appCallback)
    {
        nextMessages = null;
        appCallback = null;
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            if (temp.textMsgObj.nextText != null)
            {
                appCallback = temp.textMsgObj.nextText;
                nextMessages = temp.textMsgObj.nextmessage;
                return true;
            }
        }
        return false;
    }

    public bool TryGetIsPlayerSpeaking(string speaker, out bool isPlayerSpeaking)
    {
        isPlayerSpeaking = false;
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            isPlayerSpeaking = temp.textMsgObj.playerTalking;
            return true;
        }
        return false;
    }

    public bool TryGetOldMessages(string speaker, out string[] messages)
    {
        messages = new string[0];
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            messages = new string[temp.currentMsgIndex];
            for (int i = 0; i < messages.Length; i++)
            {
                messages[i] = temp.textMsgObj.message[i];
            }
            return true;
        }
        return false;
    }

    public bool TryGetLastMessage(string speaker, out string message)
    {
        message = null;
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            if (temp.currentMsgIndex > 0)
            {
                message = temp.textMsgObj.message[temp.currentMsgIndex - 1];
                return true;
            }
        }
        return false;
    }

    public void WriteToDatabase(string speaker)
    {
        TextMsgObjEncap temp;
        if (TextMsgObjList.TryGetValue(speaker, out temp))
        {
            string[] messages = temp.textMsgObj.message;
            List<TextDataManager.TextDataDesc> dataToWrite = new List<TextDataManager.TextDataDesc>();
            for (int i = 0; i < messages.Length; i++)
            {
                if (messages[i].Length > 0)
                {
                    dataToWrite.Add(new TextDataManager.TextDataDesc(-1, speaker, messages[i], temp.textMsgObj.playerTalking, temp.textMsgObj.Situation_Id));
                }
            }
            AppDataManager.SetData(AppDataManager.Protocol.TEXT_WRITE_TO_HISTORY, dataToWrite.Cast<AppDataManager.DataDesc>().ToList());
        }
        else
        {
            Debug.LogError("Speaker: " + speaker +" not found");
        }
    }

    public string[] GetSpeakerList()
    {
        return TextMsgObjList.Keys.ToArray();
    }

    public void OnFinish(string speaker)
    {
        WriteToDatabase(speaker);
        RemoveMsgObj(speaker);
        TextAppController.Instance.FinishedConversationWith(speaker);
    }
}
