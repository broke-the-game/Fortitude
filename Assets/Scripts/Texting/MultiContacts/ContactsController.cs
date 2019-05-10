using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ContactsController : MonoBehaviour
{
    public RectTransform rectTransform { get { return (RectTransform)transform; } }

    [SerializeField]
    GameObject ContactPrefab;

    [SerializeField]
    Transform ContactContainer;

    List<ContactControl> m_contactsList = new List<ContactControl>();

    TextDataManager TextDataManager { get { return TextAppController.Instance.TextDataManager; } }

    TextMsgObjManager TextMsgObjManager { get { return TextAppController.Instance.TextMsgObjManager; } }

    public bool IsShow;

    public void UpdateView()
    {
        updateContactList();
        updateContactPreview();
    }

    private void updateContactList()
    {
        if (m_contactsList.Count < TextDataManager.GetContactCount())
        {
            for (int i = m_contactsList.Count; i < TextDataManager.GetContactCount(); i++)
            {
                GameObject go = Instantiate(ContactPrefab, ContactContainer);
                m_contactsList.Add(go.GetComponent<ContactControl>());
            }
        }
        else if (m_contactsList.Count > TextDataManager.GetContactCount())
        {
            for (int i = m_contactsList.Count - 1; i >= TextDataManager.GetContactCount(); i--)
            {
                DestroyImmediate(m_contactsList[m_contactsList.Count - 1].gameObject);
                m_contactsList.RemoveAt(m_contactsList.Count - 1);
            }
        }
    }

    private void updateContactPreview()
    {
        string[] contacts = TextDataManager.GetContactList();
        if (contacts != null)
        {
            for (int i = 0; i < contacts.Length; i++)
            {
                TextDataManager.Conversation coversation = TextDataManager.GetMessage(contacts[i], TextDataManager.GetMessageCount(contacts[i]) - 1);
                m_contactsList[i].UnfinishedMessage = false;
                m_contactsList[i].SetName(contacts[i]);
                m_contactsList[i].SetMessage(coversation.message);
            }
        }

        string[] newMessageContacts = TextMsgObjManager.GetSpeakerList();
        if (newMessageContacts != null)
        {
            bool nameFound;
            for (int i = 0; i < newMessageContacts.Length; i++)
            {
                string message;
                GameObject go;
                ContactControl newContact;
                if (TextMsgObjManager.TryGetCurrentMsg(newMessageContacts[i], out message) || TextMsgObjManager.TryGetLastMessage(newMessageContacts[i], out message))
                {
                    nameFound = false;
                    for (int j = 0; j < m_contactsList.Count; j++)
                    {
                        if (m_contactsList[j].CurrentName == newMessageContacts[i])
                        {
                            m_contactsList[j].UnfinishedMessage = true;
                            m_contactsList[j].SetMessage(message);
                            m_contactsList[j].transform.SetAsFirstSibling();
                            nameFound = true;
                            return;
                        }
                    }
                    if (!nameFound)
                    {
                        go = Instantiate(ContactPrefab, ContactContainer);
                        newContact = go.GetComponent<ContactControl>();
                        m_contactsList.Add(newContact);
                        newContact.UnfinishedMessage = true;
                        newContact.SetName(newMessageContacts[i]);
                        newContact.SetMessage(message);
                        newContact.transform.SetAsFirstSibling();
                    }

                }
            }
        }
    }
    public void OnShow()
    {
        IsShow = true;
        string[] conversationNeedsUpdate = TextAppController.Instance.GetFinishedConversationAndClear();
        TextAppController.Instance.RequestMessagesBySenders(conversationNeedsUpdate);
        UpdateView();
    }
    public void OnHide()
    {
        IsShow = false;
    }
}
