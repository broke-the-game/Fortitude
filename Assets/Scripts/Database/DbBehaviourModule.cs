using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DataBank;
using System;


public class DbBehaviourModule : MonoBehaviour {
    private static DbBehaviourModule _instance;
    public static DbBehaviourModule Instance { get { return _instance; } }
    private const string database_name = "Fortitude_db_v1.db";
    //private const string database_name = "Fortitude_dbTest";
    private void Awake()
    {
        
        _instance = this;
        
    }

   

    private void Start()
    {
        //LocalDb testWriteDB = new LocalDb(Utility.App.Text.ToString());
        //testWriteDB.addData(new LocalEntity("02a","Text","Player","Test input","03a"));
        //testWriteDB.close();
        string db_connection_string;
        #region SETUP DATABASE
#if UNITY_IOS
            db_connection_string = "file://" + Application.dataPath + "/Raw/" +database_name;
#endif
#if UNITY_ANDROID
        db_connection_string = "jar:file://" + Application.dataPath + "!/assets/" + database_name;


#endif
#if UNITY_EDITOR
        db_connection_string = "file:///" + Application.dataPath + "/StreamingAssets/" + database_name;
        //StartCoroutine(loadStreamingAsset(db_connection_string));
#endif
        if (System.IO.File.Exists(Application.persistentDataPath + "/Fortitude.db") && StateLoadingModule.Instance.StartFromBegin)
        {
            System.IO.File.Delete(Application.persistentDataPath + "/Fortitude.db");
        }
        StartCoroutine(loadStreamingAsset(db_connection_string));

        #endregion

    }
    #region Profile Methods
    public int RequestInitialBankBalance(int ProfileID)
    {
        LocalDb _ProfileDB = new LocalDb("Profile");
        int result = 0;
        try
        {
            System.Data.IDataReader reader = _ProfileDB.getInitBankBalance(ProfileID.ToString());
            while (reader.Read())
            {
                result = int.Parse(reader[0].ToString());
            }
        }
        catch (Exception e)
        {
            Debug.Log("Database Exception");
        }
        
        
        _ProfileDB.close();
        return result;
    }

    public string[] RequestTeamMember(int ProfileID)
    {
        LocalDb _ProfileDB = new LocalDb("Profile");
        
        List<string> members = new List<string>();
        try
        {
            System.Data.IDataReader reader = _ProfileDB.getTeamMember(ProfileID.ToString());
            while (reader.Read())
            {
                members = new List<string>(reader[0].ToString().Split(','));
            }
        }
        catch(Exception e)
        {
            Debug.Log("Database Exception");
        }
        
        _ProfileDB.close();
        return members.ToArray();
    }

    public List<int> RequestClusterList(int ProfileID)
    {
        LocalDb _ProfileDB = new LocalDb("Profile");
        //TODO: Check IsDone
        System.Data.IDataReader reader = _ProfileDB.getClusterList(ProfileID.ToString());
        List<int> clusters = new List<int>();
        while (reader.Read())
        {
            foreach (string s in reader[0].ToString().Split(','))
            {
                clusters.Add(int.Parse(s));
            }
        }
        _ProfileDB.close();

        return clusters;
    }
    #endregion

    #region Cluster Methods
    public List<int> RequestSituationFromCluster(int clusterID)
    {
        List<int> result = new List<int>();
        LocalDb _ClusterDB = new LocalDb("Cluster");
        //New situation
        System.Data.IDataReader reader =  _ClusterDB.getSitByCluster(clusterID.ToString());
        while (reader.Read())
        {
            ClusterEntity entity = new ClusterEntity(reader[0].ToString(), reader[1].ToString(), reader[2].ToString());
            result = entity._sitID;
            result.Insert(0, int.Parse(entity._socialImpact));
        }
        result = _ClusterDB.updateSitStatus(result);
        _ClusterDB.close();
        return result;
    }

    public void MarkClusterFinish(int id)
    {
        LocalDb _ClusterDB = new LocalDb("Cluster");
        _ClusterDB.setClusterFinished(id.ToString());
        _ClusterDB.close();
    }
    #endregion

    #region Situation Methods
    public AppExecution FetchSituation(int clusterID, int situationID, string AppExeID = "")
    {    
        if(AppExeID == "")
        {
            LocalDb _SitDB = new LocalDb("Situation");
            string[] APPnIndex = _SitDB.getSituationIndex(situationID.ToString());
            var result = FetchSituation(clusterID, situationID,  APPnIndex[1], APPnIndex[0], _SitDB);
            _SitDB.close();
            return result;
        }
        else
        {
            LocalDb _SitDB = new LocalDb("Situation");
            string[] APPnIndex = _SitDB.getSituationIndex(situationID.ToString());
            var result = FetchSituation(clusterID, situationID, AppExeID, APPnIndex[0], _SitDB);
            _SitDB.close();
            return result;
        }
    }

    public AppExecution FetchSituation(int clusterID, int situationID, string id, string app, LocalDb SitDB)
    {
        
        System.Data.IDataReader reader;
        switch (app){
            case "Text":
                reader = SitDB.getDataByString(app.ToString(), id);
                TextMsgObj tempTextObj = ScriptableObject.CreateInstance<TextMsgObj>();
                while (reader.Read())
                {
                    //Fetch data into entity
                    TextEntity entity = new TextEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString(),
                                                               reader[5].ToString(),
                                                               reader[6].ToString(),
                                                               reader[7].ToString(),
                                                               reader[8].ToString(),
                                                               reader[9].ToString(),
                                                               reader[10].ToString());

                    //Split message by linebreaker
                    var splitted = entity._Content.Split('\n');
                    tempTextObj.message = new string[splitted.Length];
                    for (int i = 0; i < splitted.Length; i++)
                    {
                        tempTextObj.message[i] = splitted[i];
                    }

                    string[] NextList = entity._Next.Split(',');
                    tempTextObj.AppExe_Id = entity._id;
                    tempTextObj.speaker = entity._Speaker;
                    tempTextObj.nextText = new AppCallback[NextList.Length];
                    tempTextObj.nextmessage = new string[NextList.Length];
                    tempTextObj.playerTalking = entity._IsPlayer.Equals("True") ? true : false;
                    tempTextObj.bankImpact = float.Parse(entity._BankImpact);
                    
                    tempTextObj.relationshipImpacted = entity._TeamMember; 
                    tempTextObj.relationshipImpact = float.Parse(entity._TeamValue);
                    tempTextObj.ExecutingApp = Utility.App.Text;
                    tempTextObj.Cluster_Id = clusterID;
                    tempTextObj.Situation_Id = situationID;
                    
                    
                    
                    for (int i = 0; i < NextList.Length; i++)
                    {
                        AppCallback tempAppCallBack = new AppCallback();

                        try
                        {
                            tempAppCallBack.CallbackFuntion.AddListener(StateLoadingModule.Instance.FinishCurrentState, tempTextObj);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("ArgumentException");
                        }

                        if (Mathf.Abs(tempTextObj.bankImpact) >= 0.01f)
                        {
                            
                            try
                            {
                                tempAppCallBack.CallbackFuntion.AddListener(BankOperations.Instance.addBankActivity, new string[2] { tempTextObj.bankImpact.ToString(), entity._BankSummary });
                            }catch(Exception e)
                            {
                                Debug.Log("ArgumentException");
                            }
                        }
                        //if(Math.Abs(tempTextObj.relationshipImpact) >= 0.01f)
                        if(entity._TeamMessage != null && entity._TeamMessage != "")
                        {
                            tempTextObj.relationshipImpacted = SitDB.getTeamMemberNameFromIndex(int.Parse(tempTextObj.relationshipImpacted));
                            try
                            {
                                
                                tempAppCallBack.CallbackFuntion.AddListener(TeamOperations.Instance.addTeamActivity, new string[3] { tempTextObj.relationshipImpacted , entity._TeamMessage, entity._TeamValue });
                            }
                            catch (Exception e)
                            {
                                Debug.Log("ArgumentException");
                            }
                        }
                        
                        
                        Debug.Log("Next Debug Before: " + NextList[i]);
                        if (NextList[i] != null && NextList[i] != "" && !NextList[i].StartsWith("End"))
                        {                         
                            tempAppCallBack.AppExecution = FetchSituation(clusterID, situationID, NextList[i], app, SitDB);
							
							tempTextObj.nextText[i] = tempAppCallBack;
                            tempTextObj.nextmessage[i] = ((TextMsgObj)tempAppCallBack.AppExecution).message[0];
                            Debug.Log("Returned Value: " + tempTextObj.nextmessage[i]);
                        }
                        else if(NextList[0] == "End")
                        {
                            tempTextObj.nextText[0] = tempAppCallBack;

                            tempAppCallBack.CallbackFuntion.AddListener(SituationModule.Instance.SituationOnFinish,new string[2]{situationID.ToString(),clusterID.ToString()});
                            return tempTextObj;

                        }
                        else
                        {
                            tempTextObj.nextText[0] = tempAppCallBack;
                            return tempTextObj;
                        }
                    }
                    
                }
                /*if(tempTextObj.relationshipImpacted != "")
                {
                    tempTextObj.relationshipImpacted = SitDB.getTeamMemberNameFromIndex(int.Parse(tempTextObj.relationshipImpacted));
                }*/
                
                return tempTextObj;
            case "Bank":

                return new AppExecution();
            case "Mail":
                reader = SitDB.getDataByString(app.ToString(), id);
                EmailExecutionObj emailObj = ScriptableObject.CreateInstance<EmailExecutionObj>();
                string teamMemberTemp = "";
                while (reader.Read())
                {
                    //Fetch data into entity
                    MailEntity entity = new MailEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString(),
                                                               reader[5].ToString(),
                                                               reader[6].ToString(),
                                                               reader[7].ToString(),
                                                               reader[8].ToString(),
                                                               reader[9].ToString(),
                                                               reader[10].ToString(),
                                                               reader[11].ToString(),
                                                               reader[12].ToString());

                    string[] NextList = entity._next.Split(',');
                    emailObj.AppExe_Id = entity._id;
                    emailObj.WithWho = entity._speaker;
                    emailObj.IsPlayerTalking = entity._isPlayer.Equals("True") ? true : false;
                    emailObj.Subject = entity._subject;
                    emailObj.Content = entity._content;
                    emailObj.NextEmail = new string[NextList.Length];
                    emailObj.NextEmailCallback = new AppCallback[NextList.Length];
                    emailObj.OptionDescription = entity._option.Split(',');
                    emailObj.ExecutingApp = Utility.App.Mail;
                    emailObj.Cluster_Id = clusterID;
                    emailObj.Situation_Id = situationID;
                    teamMemberTemp = entity._teamMember;
            

                    for (int i = 0; i < NextList.Length; i++)
                    {
                        AppCallback tempAppCallBack = new AppCallback();
                        try
                        {
                            tempAppCallBack.CallbackFuntion.AddListener(StateLoadingModule.Instance.FinishCurrentState, emailObj);
                        }
                        catch (Exception e)
                        {
                            Debug.Log("ArgumentException");
                        }
                        if (Mathf.Abs(float.Parse(entity._bankImpact)) >= 0.01f)
                        {
                            try
                            {
                                tempAppCallBack.CallbackFuntion.AddListener(BankOperations.Instance.addBankActivity, new string[2] { entity._bankImpact, entity._bankSummary });
                            }
                            catch (Exception e)
                            {
                                Debug.Log("ArgumentException");
                            }   
                        }

                        //if (Math.Abs(float.Parse(entity._teamValue)) >= 0.01f)
                        if (entity._teamMessage != null && entity._teamMessage != "")
                        {
                            entity._teamMember = SitDB.getTeamMemberNameFromIndex(int.Parse( entity._teamMember));

                            try
                            {
                                tempAppCallBack.CallbackFuntion.AddListener(TeamOperations.Instance.addTeamActivity, new string[3] { entity._teamMember, entity._teamMessage, entity._teamValue});
                            }
                            catch (Exception e)
                            {
                                Debug.Log("ArgumentException");
                            }
                        }


                        if (NextList[i] != null && NextList[i] != "" && !NextList[i].StartsWith("End"))
                        {
                            tempAppCallBack.AppExecution = FetchSituation(clusterID, situationID, NextList[i], app, SitDB);

                            emailObj.NextEmailCallback[i] = tempAppCallBack;
                            emailObj.NextEmail[i] = ((EmailExecutionObj)tempAppCallBack.AppExecution).Content;
                        }
                        else if (NextList[0] == "End")
                        {
                            emailObj.NextEmailCallback[0] = tempAppCallBack;
                            tempAppCallBack.CallbackFuntion.AddListener(SituationModule.Instance.SituationOnFinish, new string[2] { situationID.ToString(), clusterID.ToString() });
                            
                            return emailObj;

                        }
                        else
                        {
                            emailObj.NextEmailCallback[0] = tempAppCallBack;
                            return emailObj;
                        }
                    }

                }
                
                return emailObj;
            case "News":
                reader = SitDB.getDataByString(app.ToString(), id);
                NewsExec newsObj = ScriptableObject.CreateInstance<NewsExec>();
                
                while (reader.Read())
                {
                    //Fetch data into entity
                    NewsEntity entity = new NewsEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString(),
                                                               reader[5].ToString());

                    newsObj.AppExe_Id = entity._Id;
                    newsObj.title = entity._title;
                    newsObj.description = entity._content;
                    newsObj.iconPath = entity._icon;
                    
                    newsObj.ExecutingApp = Utility.App.News;
                    newsObj.Cluster_Id = clusterID;
                    newsObj.Situation_Id = situationID;

                    AppCallback tempAppCallBack = new AppCallback();

                    try
                    {
                        tempAppCallBack.CallbackFuntion.AddListener(StateLoadingModule.Instance.FinishCurrentState, newsObj);
                        tempAppCallBack.CallbackFuntion.AddListener(SituationModule.Instance.SituationOnFinish, new string[2] { situationID.ToString(), clusterID.ToString() });

                    }
                    catch (Exception e) { }
                    
                    newsObj.appCallback = tempAppCallBack;

                    if (Mathf.Abs(float.Parse(entity._bankImpact)) >= 0.01f)
                    {

                        try
                        {
                            tempAppCallBack.CallbackFuntion.AddListener(BankOperations.Instance.addBankActivity, new string[2] { entity._bankImpact, entity._bankSummary });
                        }
                        catch (Exception e)
                        {
                            Debug.Log("ArgumentException");
                        }


                        
                    }



                }

                return newsObj;
                
                
        }
        return null;
    }

    public void MarkSitFinish(int id)
    {
        LocalDb _SitDB = new LocalDb("Situation");
        _SitDB.setSitFinished(id.ToString());
        _SitDB.close();
    }
   
    #endregion

    #region Text History Methods
    public void WriteData(List<AppDataManager.DataDesc> data)
    {
        LocalDb _HistoryDB = new LocalDb("TextHistory");
        foreach (TextDataManager.TextDataDesc d in data)
        {
            string temp = "False";
            if (d.sentByPlayer)
            {
                temp = "True";
            }

            System.Data.IDataReader reader = _HistoryDB.getLatestID("TextHistory");
            int TempID = 0;
            while (reader.Read())
            {
                TempID = int.Parse(reader[0].ToString());      
            }
            TempID += 1;

            HistoryEntity history = new HistoryEntity(TempID.ToString(),d.situationId.ToString(),temp,d.sender,d.message);
            _HistoryDB.appendTableFromBottom("TextHistory", history);
        }

        _HistoryDB.close();
        
    }

    public List<AppDataManager.DataDesc> GetHistoryFromIndex(string startIndex, string length, string sender)
    {
        
        //List<TextDataManager.TextDataDesc> FetchedData = new List<TextDataManager.TextDataDesc>();

        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();
        LocalDb _HistoryDB = new LocalDb("TextHistory");
        System.Data.IDataReader reader = _HistoryDB.getHistDataFromIndex(int.Parse(startIndex), int.Parse(length), sender);

        while (reader.Read())
        {
            HistoryEntity entity = new HistoryEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString());
            FetchedData.Add(new TextDataManager.TextDataDesc(int.Parse(entity._id), entity._Speaker, entity._Content, entity._isPlayer, int.Parse(entity._sitID)));


        }

        _HistoryDB.close();
        return FetchedData;
    }
    public List<AppDataManager.DataDesc> GetHistorySituation(string situationID, string sender)
    {
        
        LocalDb _HistoryDB = new LocalDb("TextHistory");
        System.Data.IDataReader reader = _HistoryDB.getHistDataSituation(int.Parse(situationID), sender);
        
        
        //List<TextDataManager.TextDataDesc> FetchedData = new List<TextDataManager.TextDataDesc>();

        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        

        while (reader.Read())
        {
            HistoryEntity entity = new HistoryEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString());
            FetchedData.Add(new TextDataManager.TextDataDesc(int.Parse(entity._id), entity._Speaker, entity._Content, entity._isPlayer, int.Parse(entity._sitID)));
            
            
        }
        

        _HistoryDB.close();
        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetHistoryUpdate(string length)
    {
        LocalDb _HistoryDB = new LocalDb("TextHistory");
        System.Data.IDataReader reader = _HistoryDB.getUniqueConversation();
        List<string> SpeakerList = new List<string>();
        //List<TextDataManager.TextDataDesc> FetchedData = new List<TextDataManager.TextDataDesc>();

        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        while (reader.Read())
        {
            SpeakerList.Add(reader[0].ToString());
        }
        foreach (string s in SpeakerList)
        {
            reader = _HistoryDB.getHistDataUpdate(int.Parse(length), s);
            while (reader.Read())
            {
                HistoryEntity entity = new HistoryEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString());
                FetchedData.Add(new TextDataManager.TextDataDesc(int.Parse(entity._id), entity._Speaker, entity._Content, entity._isPlayer, int.Parse(entity._sitID)));
            }
        }
        _HistoryDB.close();
        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetHistoryUpdateSpeaker(string length, string sender)
    {
        LocalDb _HistoryDB = new LocalDb("TextHistory");
        
        //List<TextDataManager.TextDataDesc> FetchedData = new List<TextDataManager.TextDataDesc>();

        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        
        System.Data.IDataReader reader = _HistoryDB.getHistDataUpdate(int.Parse(length), sender);
        while (reader.Read())
        {
            HistoryEntity entity = new HistoryEntity(reader[0].ToString(),
                                                            reader[1].ToString(),
                                                            reader[2].ToString(),
                                                            reader[3].ToString(),
                                                            reader[4].ToString());

            FetchedData.Add(new TextDataManager.TextDataDesc(int.Parse(entity._id), entity._Speaker, entity._Content, entity._isPlayer, int.Parse(entity._sitID)));
        }
        
        _HistoryDB.close();

        return FetchedData;
    }
#endregion

    #region Bank Methods
    public List<AppDataManager.DataDesc> GetBankUpdate(string length)
    {
        LocalDb _BankDB = new LocalDb("BankHistory");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();
        
        System.Data.IDataReader reader = _BankDB.getBankDataUpdate(int.Parse(length));

        while (reader.Read())
        {
            BankEntity entity = new BankEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString(),
                                                    reader[4].ToString());
            FetchedData.Add(new BankDataManager.BankDataDesc(int.Parse(entity._id),  entity._impact, entity._activity, entity._balance));
        }
        _BankDB.close();
        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetBankFromIndex(string startIndex, string length)
    {
        LocalDb _BankDB = new LocalDb("BankHistory");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _BankDB.getBankFromIndex(int.Parse(startIndex), int.Parse(length));

        while (reader.Read())
        {
            BankEntity entity = new BankEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString(),
                                                    reader[4].ToString());
            FetchedData.Add(new BankDataManager.BankDataDesc(int.Parse(entity._id), entity._impact, entity._activity,entity._balance));
        }
        _BankDB.close();
        return FetchedData;
    }

    public void WriteBankData(List<AppDataManager.DataDesc> data)
    {
        LocalDb _BankDB = new LocalDb("BankHistory");
        foreach (BankDataManager.BankDataDesc d in data)
        {
            
            BankEntity bank = new BankEntity("", "", d.activitySummary, d.amount, d.balance);
            _BankDB.appendTableFromBottom("BankHistory", bank);
        }
        _BankDB.close();

    }
    #endregion

    #region Team Methods
    public List<AppDataManager.DataDesc> GetTeamUpdate(string length)
    {
        LocalDb _TeamDB = new LocalDb("Team");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _TeamDB.getTeamDataUpdate(int.Parse(length));

        while (reader.Read())
        {
            TeamEntity entity = new TeamEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString());
            FetchedData.Add(new TeamDataManager.TeamDataDesc(int.Parse(entity._id), entity._member, entity._value, entity._message));
        }
        _TeamDB.close();

        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetTeamActUpdate(string length)
    {
        LocalDb _TeamDB = new LocalDb("TeamHistory");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _TeamDB.getTeamActDataUpdate(int.Parse(length));

        while (reader.Read())
        {
            TeamEntity entity = new TeamEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString());
            FetchedData.Add(new TeamDataManager.TeamDataDesc(int.Parse(entity._id), entity._member, entity._value, entity._message));
        }
        _TeamDB.close();

        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetTeamFromIndex(string startIndex, string length)
    {
        LocalDb _TeamDB = new LocalDb("TeamHistory");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _TeamDB.getTeamFromIndex(int.Parse(startIndex), int.Parse(length));

        while (reader.Read())
        {
            TeamEntity entity = new TeamEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString());
            FetchedData.Add(new TeamDataManager.TeamDataDesc(int.Parse(entity._id), entity._member, entity._value, entity._message));
        }
        _TeamDB.close();

        return FetchedData;
    }

    public void WriteTeamData(List<AppDataManager.DataDesc> data)
    {
        LocalDb _TeamDB = new LocalDb("TeamHistory");
        foreach (TeamDataManager.TeamDataDesc d in data)
        {

            TeamEntity team = new TeamEntity(d.id.ToString(),d.member, d.value, d.message);
            _TeamDB.appendTableFromBottom("TeamHistory", team);
        }
        _TeamDB.close();

    }

   
    #endregion

    #region Email Methods
    public List<AppDataManager.DataDesc> GetEmailUpdate()
    {
        LocalDb _MailDB = new LocalDb("MailHistory");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _MailDB.getMailDataUpdate();

        while (reader.Read())
        {
            MailEntity entity = new MailEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString(),
                                                    reader[4].ToString(),
                                                    reader[5].ToString(),
                                                    "",
                                                    "",
                                                    "",
                                                    "", 
                                                    "",
                                                    "",
                                                    "");
            bool sentByPlayer = entity._isPlayer.Equals("True") ? true : false;
            FetchedData.Add(new EmailDataManager.EmailDataDesc(int.Parse(entity._id),entity._speaker,  sentByPlayer, entity._subject, entity._content,int.Parse(entity._sitID)));
        }
        _MailDB.close();

        return FetchedData;
    }

    public void WriteMailData(List<AppDataManager.DataDesc> data)
    {
        LocalDb _MailDB = new LocalDb("MailHistory");
        System.Data.IDataReader reader = _MailDB.getLatestMailOrderID();
        int tempOrder = 0;
        while (reader.Read())
        {
            tempOrder = int.Parse(reader[0].ToString());
        }
        tempOrder += 1;
        foreach (EmailDataManager.EmailDataDesc d in data)
        {
            MailEntity mail = new MailEntity(tempOrder.ToString(),d.situationId.ToString(),d.sentByPlayer?"True":"False",d.sender,d.subject,d.content,"","","","","","","");
            _MailDB.appendTableFromBottom("MailHistory", mail);
        }
        _MailDB.close();

    }

    #endregion


    #region News Methods
    public List<AppDataManager.DataDesc> GetNewsUpdate(string length)
    {
        LocalDb _NewsDB = new LocalDb("News");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _NewsDB.getNewsDataUpdate(int.Parse(length));

        while (reader.Read())
        {
            NewsEntity entity = new NewsEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString(),
                                                    reader[4].ToString(),
                                                    reader[5].ToString(),
                                                    reader[6].ToString());
            FetchedData.Add(new NewsDataManager.NewsDataDesc(int.Parse(entity._showOrder), entity._title, entity._icon, entity._content));
        }
        _NewsDB.close();
        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetNewsFromIndex(string startIndex, string length)
    {
        LocalDb _NewsDB = new LocalDb("News");
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();

        System.Data.IDataReader reader = _NewsDB.getNewsFromIndex(int.Parse(startIndex), int.Parse(length));

        while (reader.Read())
        {
            NewsEntity entity = new NewsEntity(reader[0].ToString(),
                                                    reader[1].ToString(),
                                                    reader[2].ToString(),
                                                    reader[3].ToString(),
                                                    reader[4].ToString(),
                                                    reader[5].ToString(),
                                                    reader[6].ToString());
            FetchedData.Add(new NewsDataManager.NewsDataDesc(int.Parse(entity._showOrder), entity._title, entity._icon, entity._content));
        }
        _NewsDB.close();

        return FetchedData;
    }

    public List<AppDataManager.DataDesc> GetNewsSituation(string situationID)
    {

        LocalDb _NewsDB = new LocalDb("News");
        string[] idx = _NewsDB.getSituationIndex(situationID);
        System.Data.IDataReader reader = _NewsDB.getNewsDataSituation(int.Parse(idx[1]));     
        List<AppDataManager.DataDesc> FetchedData = new List<AppDataManager.DataDesc>();
 
        while (reader.Read())
        {
            NewsEntity entity = new NewsEntity(reader[0].ToString(),
                                                               reader[1].ToString(),
                                                               reader[2].ToString(),
                                                               reader[3].ToString(),
                                                               reader[4].ToString(),
                                                               reader[5].ToString(),
                                                               reader[6].ToString());
            FetchedData.Add(new NewsDataManager.NewsDataDesc(int.Parse(entity._showOrder), entity._title, entity._icon, entity._content));
        }
  
        _NewsDB.close();
        return FetchedData;
    }

    public void WriteNewsData(List<AppDataManager.DataDesc> data)
    {
        LocalDb _NewsDB = new LocalDb("News");
        System.Data.IDataReader reader = _NewsDB.getLatestNewsOrderID();
        int tempOrder = 0;
        while (reader.Read())
        {
            tempOrder = int.Parse(reader[0].ToString());       
        }
        tempOrder += 1;
        foreach (NewsDataManager.NewsDataDesc d in data)
        {            
            NewsEntity news = new NewsEntity("", d.title, "", "", "","",tempOrder.ToString());
            _NewsDB.appendTableFromBottom("News", news);
        }
        _NewsDB.close();

    }

    #endregion

    #region General Setup
    private IEnumerator<WWW> loadStreamingAsset(string filePath)
    {
        Debug.Log("Start Loading");
        WWW www = new WWW(filePath);
        yield return www;
        Byte[] result = www.bytes;
        Debug.Log(www.error);
        Debug.Log("Loaded DB: " + result.Length);
        if(System.IO.File.Exists(Application.persistentDataPath + "/Fortitude.db"))
        {
            Debug.Log("File Exists");
        }
        else
        {
            System.IO.File.WriteAllBytes(Application.persistentDataPath + "/Fortitude.db", result);
        }
        Initializer.Instance.Init();
    }
    #endregion


}
