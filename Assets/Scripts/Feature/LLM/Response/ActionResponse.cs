using System.Collections.Generic;

public class ActionResponse
{
    public int ActionIndex;
    public int AlternativeActionIndex;
    public int SkillIndex;
    public string TargetName;
    public string Line;

    public ActionResponse(Dictionary<string, string> data)
    {
        ActionIndex = int.Parse(data["action"]) - 1;

        if (!data.ContainsKey("alternative") || data["alternative"] == null) AlternativeActionIndex = -1;
        else AlternativeActionIndex = int.Parse(data["alternative"]) - 1;

        if(!data.ContainsKey("skill") || data["skill"] == null) SkillIndex = -1;
        else SkillIndex = int.Parse(data["skill"]) - 1;

        TargetName = data["target"];
        Line = data["line"];
    }

    public ActionResponse(int actionIndex)
    {
        ActionIndex = actionIndex;
        AlternativeActionIndex = -1;
        SkillIndex = -1;
        TargetName = null;
        Line = null;
    }
}
