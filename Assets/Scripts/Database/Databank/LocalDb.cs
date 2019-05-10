using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DataBank{
    public class LocalDb : SqliteHelper {
        private const String CodistanTag = "Codistan: LocalDb:\t";

        private string TABLE_NAME = "";
        private const String KEY_ID = "ID";
        private const String KEY_SIT = "SitID";
        private const String KEY_TITLE = "Title";
        private const String KEY_ICON = "Icon";
        private const String KEY_ACTIVITY = "Activity";
        private const String KEY_PLAYER = "IsPlayer";
        private const String KEY_APP = "APP";
        private const String KEY_SPEAKER = "Speaker";
        private const String KEY_CONTENT = "Content";
        private const String KEY_NEXT = "Next";
        private const String KEY_BALANCEIMPACT = "BankImpact";
        private const String KEY_BALANCE = "Balance";
        private const String KEY_ORDER = "ShowOrder";
        private const String KEY_DATE = "date";
        private const String KEY_SUBJECT = "Subject";


        private const String KEY_DEFAULTBANK = "InitialBank";
        private const String KEY_CLUSTER = "Cluster";
        private const String KEY_TEAMMEMBER = "TeamMember";
        private const String KEY_MEMBER = "Member";
        private const String KEY_MESSAGE = "Message";
        private const String KEY_VALUE = "Value";
        private const String KEY_SOCIALIMPACT = "SituationSocialImpact";
        private const String KEY_SITUATION = "Situation";
        private const String KEY_ISDONE = "IsDone";
        private const String KEY_SHOWORDER = "ShowOrder";
        private const String KEY_COLOR = "Color";
        private const String KEY_AGE = "Age";
        private const String KEY_EDUCATION = "Education";
        private const String KEY_RESIDENCE = "Residence";
        private const String KEY_OCCUPATION = "Occupation";
        private const String KEY_STRUGGLES = "Struggles";
        private const String KEY_GOAL = "Goal";
        private const String KEY_CHALLENGES = "Challenges";
        private const String KEY_FAMILY = "Family";
        private const String KEY_NAME = "Name";


        private const String TABLE_PROFILE = "Profile";
        private const String TABLE_CLUSTER = "Cluster";
        private const String TABLE_SITUATION = "Situation";
        private const String TABLE_TEAM = "Team";
        private const String TABLE_BANKHISTORY = "BankHistory";
        private const String TABLE_TEAMHISTORY = "TeamHistory";
        private const String TABLE_MAILHISTORY = "MailHistory";
        private const String TABLE_TEXTHISTORY = "TextHistory";
        private const String TABLE_NEWS = "News";


        private String[] COLUMNS = new String[] { KEY_ID, KEY_APP, KEY_SPEAKER, KEY_CONTENT, KEY_NEXT, KEY_DATE };

        public LocalDb(string app_name ="",  bool readProfile = false) : base(readProfile)
        {
            base.ReadProfile = readProfile;
            
        }


        public override IDataReader getDataById(int id)
        {
            return base.getDataById(id);
        }

        public string[] getSituationIndex(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT APP, SitIndex FROM  Situation WHERE ID = " + id;
            System.Data.IDataReader reader = dbcmd.ExecuteReader();
            string APP = "";
            string index = "";
            while (reader.Read())
            {
                APP = reader[0].ToString();
                index = reader[1].ToString();
            }
            return new string[2] { APP, index };
        }

        public override IDataReader getDataByString(string table, string str)
        {
            Debug.Log(CodistanTag + "Getting Location: " + str);

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + table + " WHERE " + KEY_ID + " = '" + str + "'";
            return dbcmd.ExecuteReader();
        }

        

        public override void deleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "Deleting Location: " + id);

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "DELETE FROM " + TABLE_NAME + " WHERE " + KEY_ID + " = '" + id + "'";
            dbcmd.ExecuteNonQuery();
        }

		public override void deleteDataById(int id)
        {
            base.deleteDataById(id);
        }

        public override void deleteAllData()
        {
            Debug.Log(CodistanTag + "Deleting Table");

            base.deleteAllData(TABLE_NAME);
        }

        public override IDataReader getAllData()
        {
            return base.getAllData(TABLE_NAME);
        }

        public IDataReader getNearestLocation(LocationInfo loc)
        {
            Debug.Log(CodistanTag + "Getting nearest centoid from: "
                + loc.latitude + ", " + loc.longitude);
            IDbCommand dbcmd = getDbCommand();

            string query =
                "SELECT * FROM "
                + TABLE_NAME
                + " ORDER BY ABS(" + KEY_SPEAKER + " - " + loc.latitude 
                + ") + ABS(" + KEY_CONTENT + " - " + loc.longitude + ") ASC LIMIT 1";

            dbcmd.CommandText = query;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getLatestTimeStamp()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_NAME + " ORDER BY " + KEY_DATE + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        #region Profile Methods

        public IDataReader getAllProfile()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_ID + " , " + KEY_COLOR + " , " + KEY_AGE + " , " + KEY_EDUCATION + " , " + KEY_FAMILY + " , " + KEY_RESIDENCE + " , " + KEY_OCCUPATION + " , " + KEY_STRUGGLES + " , " + KEY_GOAL + " , " + KEY_CHALLENGES + " , " + KEY_NAME + " from " + TABLE_PROFILE;
            return dbcmd.ExecuteReader();
        }
        public IDataReader getInitBankBalance(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_DEFAULTBANK + " from " + TABLE_PROFILE + " where ID = " + id;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getTeamMember(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_TEAMMEMBER + " from " + TABLE_PROFILE + " where ID = " + id;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getClusterList(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_CLUSTER + " from " + TABLE_PROFILE + " where ID = " + id;
            return dbcmd.ExecuteReader();
        }

        #endregion

        #region Cluster Methods
        public IDataReader getSitByCluster(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "SELECT " + KEY_ID + " , " + KEY_SOCIALIMPACT + " , " + KEY_SITUATION + " from " + TABLE_CLUSTER + " where " + KEY_ID + " = " + id;

            return dbcmd.ExecuteReader();
        }

        public List<int> updateSitStatus(List<int> before)
        {
            IDbCommand dbcmd = getDbCommand();
            string queryList = "(";
            foreach (int i in before)
            {
                queryList = queryList + "\"" + i.ToString() + "\",";     
            }
            queryList = queryList.TrimEnd(',') + ")";

            dbcmd.CommandText =
                "SELECT " +KEY_ID + " from " + TABLE_SITUATION + " where " + KEY_ID +" in " + queryList + " and " + KEY_ISDONE + " = \"True\"";
            System.Data.IDataReader reader = dbcmd.ExecuteReader();
           
            while (reader.Read())
            {
                if (before.Contains(int.Parse(reader[0].ToString())))
                {
                   before.Remove(int.Parse(reader[0].ToString()));
                }
            }
                return before;
        }
        public void setClusterFinished(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE  " + TABLE_CLUSTER + " SET " + KEY_ISDONE + " = \"True\"  where " + KEY_ID + " = " + id;
            dbcmd.ExecuteNonQuery();
        }

        #endregion
        #region Situation Methods

        public void setSitFinished(string id)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "UPDATE " + TABLE_SITUATION + " SET " + KEY_ISDONE + " = \"True\"  where " + KEY_ID + " = " + id;
            dbcmd.ExecuteNonQuery();
        }
        #endregion

 
        #region Text History Methods
        public override void appendTableFromBottom(string table_name, HistoryEntity history)
        {
            
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + table_name
                + " ( "
                + KEY_ID + ", "
                + KEY_SIT + ", "
                + KEY_PLAYER + ", "
                + KEY_SPEAKER + ", "
                + KEY_CONTENT + " ) "

                + "VALUES ( '"
                + history._id + "', '"
                + history._sitID + "', '"
                + history._isPlayer + "', '"
                + history._Speaker + "', '"
                + history._Content + "' )";
            dbcmd.ExecuteNonQuery();
            
        }

        public IDataReader getHistDataFromIndex(int startIndex, int length, string sender)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * FROM " + TABLE_TEXTHISTORY + " WHERE rowid < " + startIndex + " and " + KEY_SPEAKER + " = \"" + sender + "\" LIMIT " + length;


            return dbcmd.ExecuteReader();
        }
        public IDataReader getHistDataSituation(int situationID, string sender)
        {
            IDbCommand dbcmd = getDbCommand();
            
           
            dbcmd.CommandText =
               " SELECT * FROM " + TABLE_TEXTHISTORY + " WHERE " + KEY_SIT + " = " + situationID + " and " + KEY_SPEAKER + " = \"" + sender + "\"";
           

            return dbcmd.ExecuteReader();
        }
        public IDataReader getHistDataUpdate( int length, string s)
        {
            IDbCommand dbcmd = getDbCommand();
            
            dbcmd.CommandText =
                "SELECT * FROM " + TABLE_TEXTHISTORY + " where " + KEY_SPEAKER + " =\"" + s +"\" order by " + KEY_ID + " DESC LIMIT " + length;

            return dbcmd.ExecuteReader();
        }
        public IDataReader getUniqueConversation()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT " + KEY_SPEAKER + " FROM " + TABLE_TEXTHISTORY + " group by " +KEY_SPEAKER;
            return dbcmd.ExecuteReader();
        }
        #endregion

        #region Bank Methods
        public IDataReader getBankDataUpdate(int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT rowid, * from " + TABLE_BANKHISTORY + " order by rowid DESC LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getBankFromIndex(int startIndex, int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT rowid, * from " + TABLE_BANKHISTORY + " where rowid < " + startIndex + " order by rowid DESC LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public override void appendTableFromBottom(string table_name, BankEntity bank)
        {

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + table_name
                + " ( "
                + KEY_SIT + ", "
                + KEY_ACTIVITY + ", "
                + KEY_BALANCEIMPACT + ", "
                + KEY_BALANCE + " ) "

                + "VALUES ( '"
                + bank._sitID + "', '"
                + bank._activity + "', '"
                + bank._impact + "', '"
                + bank._balance + "' )";
            dbcmd.ExecuteNonQuery();

        }
        #endregion

        #region Mail Methods
        public IDataReader getMailDataUpdate()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * from " + TABLE_MAILHISTORY + " order by rowid DESC";
            return dbcmd.ExecuteReader();
        }

        public IDataReader getLatestMailOrderID()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT " + KEY_ID + " from " + TABLE_MAILHISTORY + " order by " + KEY_ID + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        public override void appendTableFromBottom(string table_name, MailEntity mail)
        {

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + table_name
                + " ( "
                + KEY_ID + ", "
                + KEY_SIT + ", "
                + KEY_PLAYER + ", "
                + KEY_SPEAKER + ", "
                + KEY_SUBJECT + ", "
                + KEY_CONTENT + " ) "

                + "VALUES ( '"
                + mail._id + "', '"
                + mail._sitID + "', '"
                + mail._isPlayer + "', '"
                + mail._speaker + "', '"
                + mail._subject + "', '"
                + mail._content + "' )";
            dbcmd.ExecuteNonQuery();

        }
        #endregion

        #region News Methods
        public IDataReader getNewsDataUpdate(int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * from " + TABLE_NEWS + " where " + KEY_SHOWORDER + " > 0 order by " + KEY_SHOWORDER + " DESC LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getNewsFromIndex(int startIndex, int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * from " + TABLE_NEWS + " where " + KEY_SHOWORDER + " > 0 and " + KEY_SHOWORDER + " < " + startIndex + " LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getNewsDataSituation(int ID)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * FROM " + TABLE_NEWS + " where " + KEY_ID + " = " + ID;
            return dbcmd.ExecuteReader();
        }

        public override void appendTableFromBottom(string table_name, NewsEntity News)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               "UPDATE " + TABLE_NEWS + " SET " + KEY_SHOWORDER + " = " + News._showOrder + " where " + KEY_TITLE + " = \"" + News._title +"\"";
            dbcmd.ExecuteNonQuery();

        }

        public IDataReader getLatestNewsOrderID()
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT " + KEY_SHOWORDER + " from " + TABLE_NEWS + " order by " + KEY_SHOWORDER + " DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }
        #endregion

        #region Team Methods
        public IDataReader getTeamDataUpdate(int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * from " + TABLE_TEAM + " where " + KEY_VALUE + " > 0 order by rowid DESC LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getTeamActDataUpdate(int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                " SELECT rowid, " + KEY_MEMBER + ", " + KEY_VALUE + ", " + KEY_MESSAGE + " from " + TABLE_TEAMHISTORY + " order by rowid DESC LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public IDataReader getTeamFromIndex(int startIndex, int length)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT * from " + TABLE_TEAM + " where " + KEY_VALUE + " > 0 and " + KEY_ID + " < " + startIndex + " LIMIT " + length;
            return dbcmd.ExecuteReader();
        }

        public string getTeamMemberNameFromIndex(int idx)
        {
            string temp = "";
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT " + KEY_MEMBER + " from " + TABLE_TEAM + " where " + KEY_ID + " = " + idx;
            IDataReader reader = dbcmd.ExecuteReader();
            while (reader.Read())
            {
                temp = reader[0].ToString();
            }

            return temp;
        }

        /*public override void appendTableFromBottom(string table_name, TeamEntity team)
        {

            IDbCommand dbcmd = getDbCommand();

            //TODO Discuss Team Behavior
            dbcmd.CommandText =
               "UPDATE Team SET Value = " + team._value + " , Message = \"" + team._message + "\"  where Member = \"" + team._member + "\"";

            
            dbcmd.ExecuteNonQuery();

        }*/

        public override void appendTableFromBottom(string table_name, TeamEntity team)
        {

            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
                "INSERT INTO " + table_name
                + " ( "
                + KEY_MEMBER + ", "
                + KEY_VALUE + ", "
                + KEY_MESSAGE + " ) "

                + "VALUES ( '"
                + team._member + "', '"
                + team._value + "', '"
                + team._message + "' )";
            dbcmd.ExecuteNonQuery();
        }
        #endregion

        #region General Methods
        public IDataReader getLatestID(string table)
        {
            IDbCommand dbcmd = getDbCommand();
            dbcmd.CommandText =
               " SELECT " + KEY_ID + " from " + table+ " order by rowid DESC LIMIT 1";
            return dbcmd.ExecuteReader();
        }

        #endregion

    }
}
