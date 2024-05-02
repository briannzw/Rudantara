using System.Collections.Generic;

public class ActionResponse
{
    public int State;
    public int ActionIndex;
    public int AlternativeActionIndex;
    public string TargetName;
    public int SkillIndex;
    public string SkillTarget;
    public string Line;
    public int EmotionIndex;

    public ActionResponse(Dictionary<string, string> data)
    {
        State = int.Parse(data["state"]);

        ActionIndex = int.Parse(data["action"]) - 1;

        if (!data.ContainsKey("alternative") || data["alternative"] == null) AlternativeActionIndex = -1;
        else AlternativeActionIndex = int.Parse(data["alternative"]) - 1;

        if (!data.ContainsKey("skill") || data["skill"] == null || !data.ContainsKey("skillTarget") || data["skillTarget"] == null)
        {
            SkillIndex = -1;
            SkillTarget = "";
        }
        else
        {
            SkillIndex = int.Parse(data["skill"]) - 1;
            SkillTarget = data["skillTarget"];
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
        SkillIndex = -1;
        SkillTarget = null;
        TargetName = null;
        Line = null;
        EmotionIndex = 0;
    }
}
