using UnityEngine;

[System.Serializable]
public class BigFivePersonality
{
    [SerializeField, Range(0f, 1f)] private float Openness = .5f;
    [SerializeField, Range(0f, 1f)] private float Conscientiousness = .5f;
    [SerializeField, Range(0f, 1f)] private float Extraversion = .5f;
    [SerializeField, Range(0f, 1f)] private float Agreeableness = .5f;
    [SerializeField, Range(0f, 1f)] private float Neuroticism = .5f;

    public string Describe()
    {
        string description = "";

        description += "Openness: " + (Openness * 100).ToString();
        description += ", Conscientiousness: " + (Conscientiousness * 100).ToString();
        description += ", Extraversion: " + (Extraversion * 100).ToString();
        description += ", Agreeableness: " + (Agreeableness * 100).ToString();
        description += ", Neuroticism: " + (Neuroticism * 100).ToString();

        description += ".";

        return description;
    }
}
