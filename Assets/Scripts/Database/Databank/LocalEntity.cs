using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataBank
{
	public class TextEntity {

		public string _id;
        public string _rowId;
        public string _IsPlayer;
        public string _Speaker;
        public string _Content;
        public string _Next;
        public string _BankImpact;
        public string _BankSummary;
        public string _TeamMember;
        public string _TeamValue;
        public string _TeamMessage;
        
        

        public String _dateCreated; // Auto generated timestamp


        public TextEntity(string rowid, string id, string isPlayer, string speaker, string content, string next, string bankI, string bankS, string teamMember, string teamValue, string teamMessage)
        {
            _rowId = rowid;
            _id = id;
            _IsPlayer = isPlayer;
            _Speaker = speaker;
            _Content = content;
            _Next = next;
            _BankImpact = bankI;
            _BankSummary = bankS;
            _TeamMember = teamMember;
            _TeamValue = teamValue;
            _TeamMessage = teamMessage;
        }
        

        //public static LocationEntity getFakeLocation()
        //{
        //    return new LocationEntity("0", "Test_Type", "0.0", "0.0");
        //}
	}

    public class ProfileEntity
    {
        public const int BASIC_START = 2;
        public const int BASIC_END = 7;
        public enum DataField{
            ID          = 0,
            Color       = 1,
            Age         = 2,
            Education   = 3,
            Family      = 4,
            Residence   = 5,
            Occupation  = 6,
            Struggles   = 7,
            Goal        = 8,
            Challenges  = 9,
            Name        = 10,
        }
        Dictionary<DataField, string> ProfileData = new Dictionary<DataField, string>();



        public ProfileEntity()
        {

        }
        
        public void SetField(int i, string s)
        {
            if (s != null && s != "")
                ProfileData.Add((DataField)i, s);
        }
        public string GetField(DataField dataField)
        {
            string value = null;
            ProfileData.TryGetValue(dataField, out value);
            return value;
        }

        public bool TryGetField(DataField dataField, out string value)
        {
            return ProfileData.TryGetValue(dataField, out value);
        }

        public static class ColorPool
        {
            public static Color Red = Color.red;
            public static Color Purple = new Color(0.4392157f, 0.3411765f, 0.5568628f);
            public static Color Green = new Color(0.2078432f, 0.6627451f, 0.5686275f);
            public static Color Blue = new Color(0.40836f, 0.5967264f, 0.664f);
            public static Color Yellow = new Color(0.853f, 0.6596739f, 0.047768f);
            public static Color Orange = new Color(0.937f, 0.3856905f, 0f);

        }

        public static Color GetColor(string colorName)
        {
            switch (colorName)
            {
                case "Red":
                    return ColorPool.Red;
                case "Purple":
                    return ColorPool.Purple;
                case "Green":
                    return ColorPool.Green;
                case "Blue":
                    return ColorPool.Blue;
                case "Yellow":
                    return ColorPool.Yellow;
                case "Orange":
                    return ColorPool.Orange;
                default:
                    return Color.white;
            }
        }
    }



    public class ClusterEntity
    {
        public string _id;
        public string _socialImpact;
        public List<int> _sitID;
        public ClusterEntity(string id, string socialImpact, string sitID)
        {
            _id = id;
            _socialImpact = socialImpact;
            
            _sitID = new List<int>();
            foreach(string s in sitID.Split(','))
            {
                _sitID.Add(int.Parse(s));
            }
        }
    }
    public class TeamEntity
    {
        public string _id;
        public string _member;
        public string _value;     
        public string _message;

        public TeamEntity(string id = "", string member = "",  string value = "", string message = "")
        {
            _id = id;
            _member = member;
            _value = value;
            _message = message;
        }

    }

    public class MailEntity
    {
        public string _id;
        public string _sitID;
        public string _isPlayer;
        public string _speaker;
        public string _subject;
        public string _content;
        public string _next;
        public string _bankImpact;
        public string _bankSummary;
        public string _teamMember;
        public string _teamValue;
        public string _teamMessage;
        
        public string _option;
        public MailEntity(string id, string sitID, string isPlayer, string speaker, string subject, string content, string option, string next, string bankImpact, string bankSummary, string teamMember, string teamValue, string teamMessage)
        {
            _id = id;
            _sitID = sitID;
            _isPlayer = isPlayer;
            _speaker = speaker;
            _subject = subject;
            _content = content;
            _option = option;
            _next = next;
            _bankImpact = bankImpact;
            _bankSummary = bankSummary;
            _teamMember = teamMember;
            _teamValue = teamValue;
            _teamMessage = teamMessage;
        }
    }

    public class BankEntity
    {
        public string _id;
        public string _sitID;
        public string _activity;
        public string _impact;
        public string _balance;

        public BankEntity(string id = "", string sitID = "", string activity= "", string impact = "", string balance = "")
        {
            _id = id;
            _sitID = sitID;
            _activity = activity;
            _impact = impact;
            _balance = balance;
        }

    }

    public class NewsEntity
    {
        public string _Id;
        public string _title;
        public string _content;
        public string _icon;
        public string _bankImpact;
        public string _bankSummary;
        public string _showOrder;

        public NewsEntity(string id = "", string title = "", string content = "", string icon = "", string bankImpact = "", string bankSummary ="", string showOrder = "")
        {
            _Id = id;
            _title = title;
            _content = content;
            _icon = icon;
            _bankImpact = bankImpact;
            _bankSummary = bankSummary;
            _showOrder = showOrder;
        }
    }
    public class HistoryEntity
    {
        public string _id;
        public string _sitID;
        public bool _isPlayer;
        public string _Speaker;
        public string _Content;
        
        public HistoryEntity(string id = "", string sitID = "", string isPlayer = "", string speaker = "", string content = "")
        {
            _id = id;
            _sitID = sitID;
            if (isPlayer == "False")
            {
                _isPlayer = false;
            }
            else
            {
                _isPlayer = true;
            }
          
            _Speaker = speaker;
            _Content = content; 

        }
    }
}
