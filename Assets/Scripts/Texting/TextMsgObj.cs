using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Text Message", menuName = "Fortitude/Text Message", order = 1)]
public class TextMsgObj : AppExecution
{
    public string speaker; //which npc is involved in this conversation
    public bool playerTalking; //did the player send this text?
    public string[] message; //if the speaker sends multiple texts at once,
    public string[] nextmessage; // next message content
    //each can be listed as one string in this list
    public AppCallback[] nextText; //empty if end of conversation, 
    //multiple if the player will have to make a choice.
    //it's recommended to map dialogue trees in Twine for visual reference

    //the following show if certain levels go up or down when this text is sent, and by how much
    public float bankImpact;
    public string relationshipImpacted; //which relationship is impacted here?
    public float relationshipImpact;
}
