using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTest : MonoBehaviour
{
    public Material TestMat;
    
    // Start is called before the first frame update
    void Start()
    {
        Texture2D tex = new Texture2D(512, 512);
        var temp = new Situation("1",Utility.App.News);
        Debug.Log(temp.getID());
       //Debug.Log(test.title);
        
        //Debug.Log(tempObj.nextText[0]);
        StartCoroutine(TestProtocol());
        //var a = DbBehaviourModule.Instance.RequestInitialBankBalance(1);
        //var b = DbBehaviourModule.Instance.RequestClusterList(1);
        //var c = DbBehaviourModule.Instance.RequestTeamMember(1);
        //tex.LoadImage(System.IO.File.ReadAllBytes(Application.dataPath + "/StreamingAssets/news_pic/flood.png"));
        //Debug.Log("TEMPP:" + temp);
        GameObject.Find("Plane").GetComponent<Renderer>().material.SetTexture("_MainTex", tex);


    }
    private void Update()
    {
        
    }
    IEnumerator TestProtocol()
    {
        yield return new WaitForSeconds(5);
        //Test Protocol.TEXT_GET_FROM_INDEX
        //var test = (NewsExec)SituationModule.Instance.getSituationContent(temp);
        //TextMsgObj tempObj = (TextMsgObj)
        var test = DbBehaviourModule.Instance.FetchSituation(1, 2);

        //AppDataManager.SetData(AppDataManager.Protocol.TEAM_WRITE_TO_HISTORY, new List<AppDataManager.DataDesc>() { new TeamDataManager.TeamDataDesc(1, "Doctor","2", "This is amazing") });

        var temp = AppDataManager.RequestData(AppDataManager.Protocol.TEAM_ACT_GET_LATEST, new string[1] {"5"});
        //Debug.Log(((TeamDataManager.TeamDataDesc)temp[0]).message);
        //Debug.Log(((TeamDataManager.TeamDataDesc)temp[1]).message);
        
        //GameObject.Find("Canvas").gameObject.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = ((TeamDataManager.TeamDataDesc)temp[0]).message;
        //Debug.Log(temp.Count);

        //Test Protocol.TEXT_GET_LATEST_BY_SITUATION

        //TextDataManager.TextDataDesc TestData = new TextDataManager.TextDataDesc(10, "TempSpeaker", "TempContent", false, 99);
        //TextDataManager.TextDataDesc TestData1 = new TextDataManager.TextDataDesc(11, "TempSpeaker1", "TempContent2", false, 99);

        //AppDataManager.SetData(AppDataManager.Protocol.TEXT_WRITE_TO_HISTORY, new TextDataManager.TextDataDesc[]{TestData,TestData1} );


    }


}
