using UnityEngine;

[System.Serializable]
public class BigFivePersonality
{
    [Range(0f, 1f)] public float Openness = .5f;
    [Range(0f, 1f)] public float Conscientiousness = .5f;
    [Range(0f, 1f)] public float Extraversion = .5f;
    [Range(0f, 1f)] public float Agreeableness = .5f;
    [Range(0f, 1f)] public float Neuroticism = .5f;

    public BigFivePersonality()
    {
        Openness = .9f;
        Conscientiousness = .85f;
        Extraversion = .45f;
        Agreeableness = .95f;
        Neuroticism = .2f;
    }

    public string Describe()
    {
        string description = "";

        description += "Openness: " + Mathf.RoundToInt(Openness * 100).ToString();
        description += ", Conscientiousness: " + Mathf.RoundToInt(Conscientiousness * 100).ToString();
        description += ", Extraversion: " + Mathf.RoundToInt(Extraversion * 100).ToString();
        description += ", Agreeableness: " + Mathf.RoundToInt(Agreeableness * 100).ToString();
        description += ", Neuroticism: " + Mathf.RoundToInt(Neuroticism * 100).ToString();

        description += ".";

        return description;
    }
}
