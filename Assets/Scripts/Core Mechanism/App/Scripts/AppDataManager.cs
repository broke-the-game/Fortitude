using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppDataManager : MonoBehaviour
{
    public abstract class DataDesc
    {
        public int id { get; }
        public DataDesc(int index)
        {
            id = index;
        }
    }
    public enum Protocol
    {
        TEXT_BEGIN,

        /// <summary> 
        ///     Get latest text history
        ///     <para>
        ///     Args: int length/conversation
        ///     </para>
        /// </summary>
        TEXT_GET_LATEST,

        /// <summary> 
        ///     Get text history from start from index
        ///     <para>
        ///     Args: int startIndex, int length, string sender
        ///     </para>
        /// </summary>
        TEXT_GET_FROM_INDEX,

        /// <summary> 
        ///     Get text history of the situation
        ///     <para>
        ///     Args: int situationId, string sender
        ///     </para>
        /// </summary>
        TEXT_GET_LATEST_BY_SITUATION,

        /// <summary> 
        ///     Get latest text history by speaker
        ///     <para>
        ///     Args: int length/conversation, string sender
        ///     </para>
        /// </summary>
        TEXT_GET_LATEST_BY_SPEAKER,


        /// <summary> 
        ///     Write new messages to history
        ///     <para>
        ///     Args: TextDataManager.TextDataDesc[] dataToWrite
        ///     </para>
        /// </summary>
        TEXT_WRITE_TO_HISTORY,

        TEXT_END,

        BANK_BEGIN,

        /// <summary>
        ///     Get latest bank history
        ///     <para>
        ///     Args: int length
        ///     </para>
        /// </summary>
        BANK_GET_LATEST,

        /// <summary>
        ///     Get bank hisotry from start index
        ///     <para>
        ///     Args: int startIndex, int length
        ///     </para>
        /// </summary>
        BANK_GET_FROM_INDEX,

        /// <summary>
        ///     Write new activity to history
        ///     <para>
        ///     Args: BankDataManager.BankDataDesc[] dataToWrite
        ///     </para>
        /// </summary>
        BANK_WRITE_TO_HISTORY,

        BANK_END,

        NEWS_BEGIN,

        /// <summary>
        ///     Get latest news contents
        ///     <para>
        ///     Args: int length
        ///     </para>
        /// </summary>
        NEWS_GET_LATEST,

        /// <summary>
        ///     Get news history from start index
        ///     <para>
        ///     Args: int startIndex, int length
        ///     </para>
        /// </summary>
        NEWS_GET_FROM_INDEX,

        /// <summary>
        ///     Get news content of the situation
        ///     <para>
        ///     Args: int situationId
        ///     </para>
        /// </summary>
        NEWS_GET_LATEST_BY_SITUATION,

        /// <summary>
        ///     Write new news content to history
        ///     <para>
        ///     Args: NewsDataManager.NewsDataDesc[] dataToWrite
        ///     </para>
        /// </summary>
        NEWS_WRITE_TO_HISTORY,

        NEWS_END,

        TEAM_BEGIN,
        /// <summary>
        ///     Get latest teamhistory
        ///     <para>
        ///     Args: int length
        ///     </para>
        /// </summary>
        TEAM_GET_LATEST,

        TEAM_ACT_GET_LATEST,
        /// <summary>
        ///     Get team history from start index
        ///     <para>
        ///     Args: int startIndex, int length
        ///     </para>
        /// </summary>
        TEAM_GET_FROM_INDEX,

        /// <summary>
        ///     Write new activity to history
        ///     <para>
        ///     Args: TeamDataManager.TeamDataDesc[] dataToWrite
        ///     </para>
        /// </summary>
        TEAM_WRITE_TO_HISTORY,

        TEAM_ACT_WRITE_TO_HISTORY,

        TEAM_END,

        EMAIL_BEGIN,

        /// <summary>
        ///     Get latest mailhistory
        ///     <para>
        ///  
        ///     </para>
        /// </summary>
        EMAIL_GET_LATEST,

        /// <summary>
        ///     Write new activity to history
        ///     <para>
        ///     Args: EmailDataManager.EmailDataDesc[] dataToWrite
        ///     </para>
        /// </summary
        EMAIL_WRITE_TO_HISTORY,

        EMAIL_END

    }

    public static List<DataDesc> RequestData(Protocol protocol, string[] args)
    {
        switch (protocol)
        {
            //Text APP Protocol
            case Protocol.TEXT_GET_LATEST:
                return DbBehaviourModule.Instance.GetHistoryUpdate(args[0]);

            case Protocol.TEXT_GET_FROM_INDEX:
                return DbBehaviourModule.Instance.GetHistoryFromIndex(args[0], args[1], args[2]);

            case Protocol.TEXT_GET_LATEST_BY_SITUATION:
                return DbBehaviourModule.Instance.GetHistorySituation(args[0], args[1]);

            case Protocol.TEXT_GET_LATEST_BY_SPEAKER:
                return DbBehaviourModule.Instance.GetHistoryUpdateSpeaker(args[0], args[1]);

            //Bank APP Protocol
            case Protocol.BANK_GET_LATEST:
                return DbBehaviourModule.Instance.GetBankUpdate(args[0]);
                
            case Protocol.BANK_GET_FROM_INDEX:
                return DbBehaviourModule.Instance.GetBankFromIndex(args[0],args[1]);

            //News APP Protocol
            case Protocol.NEWS_GET_LATEST:
                return DbBehaviourModule.Instance.GetNewsUpdate(args[0]);
                
            case Protocol.NEWS_GET_FROM_INDEX:
                return DbBehaviourModule.Instance.GetNewsFromIndex(args[0], args[1]);
                
            case Protocol.NEWS_GET_LATEST_BY_SITUATION:
                return DbBehaviourModule.Instance.GetNewsSituation(args[0]);

            //Team APP Protocol
            case Protocol.TEAM_GET_LATEST:
                return DbBehaviourModule.Instance.GetTeamUpdate(args[0]);

            case Protocol.TEAM_ACT_GET_LATEST:
                return DbBehaviourModule.Instance.GetTeamActUpdate(args[0]);

            case Protocol.TEAM_GET_FROM_INDEX:
                return DbBehaviourModule.Instance.GetTeamFromIndex(args[0],args[1]);

            //EMAIL APP Protocol
            case Protocol.EMAIL_GET_LATEST:
                return DbBehaviourModule.Instance.GetEmailUpdate();

        }
        return null;
    }

    public static void SetData(Protocol protocol, List<DataDesc> data)
    {
        switch (protocol)
        {
            case Protocol.TEXT_WRITE_TO_HISTORY:
                DbBehaviourModule.Instance.WriteData(data);
                break;
            case Protocol.BANK_WRITE_TO_HISTORY:
                DbBehaviourModule.Instance.WriteBankData(data);
                break;
            case Protocol.NEWS_WRITE_TO_HISTORY:
                DbBehaviourModule.Instance.WriteNewsData(data);
                break;
            case Protocol.TEAM_WRITE_TO_HISTORY:
                DbBehaviourModule.Instance.WriteTeamData(data);
                break;
            case Protocol.EMAIL_WRITE_TO_HISTORY:
                DbBehaviourModule.Instance.WriteMailData(data);
                break;
        }
    }
}
