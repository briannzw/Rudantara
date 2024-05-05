using UnityEngine;

public class ActionResult
{
    public bool WasInCombat = false;
    public int OwnEnemiesKilled = 0;
    public int PlayerEnemiesKilled = 0;
    public float PrevOwnHealth = 0;
    public float PrevPlayerHealth = 0;

    public Character OwnerChara;
    public Character PlayerChara;

    public string Describe()
    {
        string output = "";

        output += "Was in combat : " + (WasInCombat ? "Yes" : "No") + "\n";
        output += "Result : \n";
        output += "Player killed " + PlayerEnemiesKilled + " Enemies\n";
        output += "You killed " + OwnEnemiesKilled + " Enemies\n";
        output += "Player's health " + HealthDelta(PrevPlayerHealth, PlayerChara.CheckStat(DynamicStatEnum.Health), PlayerChara.CheckStatMax(DynamicStatEnum.Health)) + "\n";
        output += "Your health " + HealthDelta(PrevOwnHealth, OwnerChara.CheckStat(DynamicStatEnum.Health), OwnerChara.CheckStatMax(DynamicStatEnum.Health)) + "\n";

        return output;
    }

    private string HealthDelta(float previous, float current, float max)
    {
        string output = "";
        int delta = Mathf.RoundToInt(previous - current);

        if (delta < 0) output += "hurt by " + Mathf.Abs(delta) + "(" + Mathf.RoundToInt(delta/max * 100f) + "% health)";
        else if (delta > 0) output += "healed by " + delta + "(" + Mathf.RoundToInt(delta / max * 100f) + "% health)";
        else output += "unchanged";

        return output;
    }

    public void New()
    {
        OwnEnemiesKilled = 0;
        PlayerEnemiesKilled = 0;
        PrevOwnHealth = OwnerChara.CheckStat(DynamicStatEnum.Health);
        PrevPlayerHealth = PlayerChara.CheckStat(DynamicStatEnum.Health);
    }
}
