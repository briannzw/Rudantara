using System.Collections.Generic;
using UnityEngine;

public class ActionResponse
{
    public int State;
    public int ActionIndex;
    public int AlternativeActionIndex;
    public string TargetName;
    public string HealTarget;
    public string Line;
    public int EmotionIndex;

    public ActionResponse(Dictionary<string, string> data)
    {
        State = int.Parse(data["state"]);

        ActionIndex = int.Parse(data["action"]) - 1;

        if (!data.ContainsKey("alternative") || data["alternative"] == null) AlternativeActionIndex = -1;
        else AlternativeActionIndex = int.Parse(data["alternative"]) - 1;

        if (!data.ContainsKey("healTarget") || data["healTarget"] == null)
        {
            HealTarget = "";
        }
        else
        {
            HealTarget = data["healTarget"];
        }


        TargetName = data["target"];
        Line = data["line"];

        if (!data.ContainsKey("emotion") || data["emotion"] == null) EmotionIndex = 0;
        else EmotionIndex = int.Parse(data["emotion"]) - 1;
    }

    // Defaults
    public ActionResponse(int actionIndex)
    {
        State = 1;
        ActionIndex = actionIndex;
        AlternativeActionIndex = -1;
        HealTarget = null;
        TargetName = null;
        Line = null;
        EmotionIndex = 0;
    }
}
