using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TextDataManager: MonoBehaviour
{
    Dictionary<string, List<Conversation>> DataHashMap = new Dictionary<string, List<Conversation>>();

    public class Conversation
    {
        public enum Sender { Player, NPC }
        public Sender speaker { get; }
        public string message { get; }
        public int indexInDatabase { get; }
        public Conversation(Sender speaker, string message, int index)
        {
            this.speaker = speaker;
            this.message = message;
            this.indexInDatabase = index;
        }
        public class Comparation : IComparer<Conversation>
        {
            private Comparation() { }

            private static Comparation m_instance;
            public static Comparation Instance
            {
                get
                {
                    if (m_instance == null)
                        m_instance = new Comparation();
                    return m_instance;
                }
            }
            // Compares by Height, Length, and Width.
            public int Compare(Conversation x, Conversation y)
            {
                return x.indexInDatabase.CompareTo(y.indexInDatabase);
            }
        }
    }

    public class TextDataDesc: AppDataManager.DataDesc
    {
        public string sender { get; }
        public string message { get; }
        public bool sentByPlayer { get; }
        public int situationId { get; }
        public TextDataDesc(int index, string sender, string message, bool sentByPlayer, int situationId)
            : base(index)
        {
            this.sender = sender;
            this.message = message;
            this.sentByPlayer = sentByPlayer;
            this.situationId = situationId;
        }
    }

    public int GetContactCount()
    {
        return DataHashMap.Count;
    }

    public string[] GetContactList()
    {
        return DataHashMap.Keys.ToArray();
    }

    public int GetMessageCount(string contactName)
    {
        List<Conversation> messages;
        if (DataHashMap.TryGetValue(contactName, out messages))
        {
            return messages.Count;
        }
        return 0;
    }

    public Conversation GetMessage(string contactName, int index)
    {
        List<Conversation> messages;
        if (DataHashMap.TryGetValue(contactName, out messages))
        {
            return messages[index];
        }
        return null;
    }

    public void AcquiredLatestMessage(List<TextDataDesc> dataDesc)
    {
        DataHashMap.Clear();
        List<Conversation> temp;
        for (int i = 0; i < dataDesc.Count; i++)
        {
            if (DataHashMap.TryGetValue(dataDesc[i].sender, out temp))
            {
                temp.Add(new Conversation(dataDesc[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDesc[i].message, dataDesc[i].id));
            }
            else
            {
                temp = new List<Conversation>();
                temp.Add(new Conversation(dataDesc[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDesc[i].message, dataDesc[i].id));
                DataHashMap.Add(dataDesc[i].sender, temp);
            }
        }
        foreach (var conversations in DataHashMap)
        {
            conversations.Value.Sort(Conversation.Comparation.Instance);
        }
    }

    public void AppendPreviousMessages(List<TextDataDesc> dataDescs, string sender)
    {
        List<Conversation> temp;
        if (DataHashMap.TryGetValue(sender, out temp))
        {
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));
            }
            temp.Sort(Conversation.Comparation.Instance);
        }
        else
        {
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp = new List<Conversation>();
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));

            }
            temp.Sort(Conversation.Comparation.Instance);
            DataHashMap.Add(sender, temp);
        }

    }

    public void AcquiredMessageBySituation(List<TextDataDesc> dataDescs, string sender)
    {
        List<Conversation> temp;
        if (DataHashMap.TryGetValue(sender, out temp))
        {
            temp.Clear();
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));
            }
            temp.Sort(Conversation.Comparation.Instance);
        }
        else
        {
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp = new List<Conversation>();
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));
            }
            temp.Sort(Conversation.Comparation.Instance);
            DataHashMap.Add(sender, temp);
        }
    }

    //need protocol, used after TextMsgObjFinishedNeedUpdate
    public void AcquiredLatestMessageForConversation(List<TextDataDesc> dataDescs)
    {
        if (dataDescs.Count < 1)
            return;
        string sender = dataDescs[0].sender;
        List<Conversation> temp;
        if (DataHashMap.TryGetValue(sender, out temp))
        {
            temp.Clear();
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));
            }
            temp.Sort(Conversation.Comparation.Instance);
        }
        else
        {
            for (int i = 0; i < dataDescs.Count; i++)
            {
                temp = new List<Conversation>();
                temp.Add(new Conversation(dataDescs[i].sentByPlayer ? Conversation.Sender.Player : Conversation.Sender.NPC, dataDescs[i].message, dataDescs[i].id));
            }
            temp.Sort(Conversation.Comparation.Instance);
            DataHashMap.Add(sender, temp);
        }
    }
}
