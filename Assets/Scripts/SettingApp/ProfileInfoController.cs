using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataBank;

public class ProfileInfoController : MonoBehaviour
{
    [SerializeField]
    TMPro.TextMeshProUGUI InfoTitle, BasicInfo, StruggleNGoal;
    public void ParseProfile(ProfileEntity profileEntity)
    {
        string basicInfo = "";
        string value;
        string subHead;
        InfoTitle.text = profileEntity.GetField(ProfileEntity.DataField.Name);
        for (int i = ProfileEntity.BASIC_START; i <= ProfileEntity.BASIC_END; i++)
        {
            if (profileEntity.TryGetField((ProfileEntity.DataField)i, out value))
            {
                subHead = typeof(ProfileEntity.DataField).GetEnumName(i);
                basicInfo = basicInfo + "<b>" + subHead + ":</b> " + value + "\n";
            }
        }
        BasicInfo.text = basicInfo;

        string goal = profileEntity.GetField(ProfileEntity.DataField.Goal);
        string challenges = profileEntity.GetField(ProfileEntity.DataField.Challenges);
        StruggleNGoal.text = "<b><size=+2>Goal:</size></b>\n" + Utility.FormatData(goal) + "<line-height=10%>\n </line-height>\n<b><size=+2>Challenges:</size></b>\n" + Utility.FormatData(challenges);
    }
}
