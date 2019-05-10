using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Data.Sqlite;
using UnityEngine;
using System.Data;

namespace DataBank
{
    public class SqliteHelper : MonoBehaviour
    {
        private const string CodistanTag = "Codistan: SqliteHelper:\t";

        private const string database_name = "Fortitude.db";
        private const string profile_db_name = "Fortitude_db_v1.db";
        public string db_connection_string;
        public IDbConnection db_connection;
        public bool ReadProfile;
        public SqliteHelper(bool readProfile)
        {
            ReadProfile = readProfile;
            if (!ReadProfile)
            {
                //db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;
                //db_connection_string = "URI=file:" + Application.dataPath + "/Database/" + database_name;

#if UNITY_IOS
                        //db_connection_string = "URI=file:" + Application.dataPath + "/Raw/" +database_name;
                        db_connection_string = "URI=file:" + Application.persistentDataPath + "/" +database_name;
                        
#endif
#if UNITY_ANDROID
                //db_connection_string = "jar:file://" + Application.dataPath + "!/assets/" + database_name;
                db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;


#endif

#if UNITY_EDITOR
                //db_connection_string = "URI=file:" + Application.dataPath + "/StreamingAssets/" + database_name;
                db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;

#endif

                //Debug.Log("Windows");
                //db_connection_string = "URI=file:" + Application.dataPath + "/StreamingAssets/" + database_name;


            }
            else
            {
#if UNITY_IOS
                //db_connection_string = "file://" + Application.dataPath + "/Raw/" +profile_db_name;
                db_connection_string = "URI=file:" + Application.dataPath + "/Raw/" +profile_db_name;
                        
#endif
#if UNITY_ANDROID
                //db_connection_string = "jar:file://" + Application.dataPath + "!/assets/" + profile_db_name;
                db_connection_string = "URI=file:" + Application.persistentDataPath + "/Fortitude_db_androidProfile.db";


#endif

#if UNITY_EDITOR
                db_connection_string = "URI=file:" + Application.dataPath + "/StreamingAssets/" + profile_db_name;
                //db_connection_string = "file:///" + Application.dataPath + "/StreamingAssets/" + database_name;

                //db_connection_string = "URI=file:" + Application.persistentDataPath + "/" + database_name;

#endif

                //Debug.Log("Windows");
                //db_connection_string = "URI=file:" + Application.dataPath + "/StreamingAssets/" + database_name;

            }

            Debug.Log("db_connection_string " + db_connection_string);
            db_connection = new SqliteConnection(db_connection_string);
            db_connection.Open();
        }

        ~SqliteHelper()
        {
            db_connection.Close();
        }

        //vitual functions
        public virtual IDataReader getDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader getDataByString(string table, string str)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void deleteDataById(int id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

		public virtual void deleteDataByString(string id)
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual IDataReader getAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemented");
            throw null;
        }

        public virtual void deleteAllData()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual IDataReader getNumOfRows()
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        //helper functions
        public IDbCommand getDbCommand()
        {
            return db_connection.CreateCommand();
        }

        public IDataReader getAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT * FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

        public void deleteAllData(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText = "DROP TABLE IF EXISTS " + table_name;
            dbcmd.ExecuteNonQuery();
        }

        public IDataReader getNumOfRows(string table_name)
        {
            IDbCommand dbcmd = db_connection.CreateCommand();
            dbcmd.CommandText =
                "SELECT COALESCE(MAX(id)+1, 0) FROM " + table_name;
            IDataReader reader = dbcmd.ExecuteReader();
            return reader;
        }

		public void close (){
			db_connection.Close ();
            Debug.Log("DB Close: " + db_connection_string);

		}

        public virtual void appendTableFromBottom(string table_name, HistoryEntity history)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void appendTableFromBottom(string table_name, BankEntity bank)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void appendTableFromBottom(string table_name, NewsEntity News)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }

        public virtual void appendTableFromBottom(string table_name, TeamEntity Team)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }
        public virtual void appendTableFromBottom(string table_name, MailEntity Team)
        {
            Debug.Log(CodistanTag + "This function is not implemnted");
            throw null;
        }
        IEnumerator<WWW> loadStreamingAsset(string filePath)
        {
            Debug.Log("Start Loading");
            WWW www = new WWW(db_connection_string);
            yield return www;
            Byte[] result = www.bytes;
            Debug.Log("Loaded DB: " + result.ToString());
        }
    }
}