using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(menuName = "Level Pattern/Stats Distribution", fileName = "New Stats Distribution")]
public class CharacterStatLevelPattern : ScriptableObject
{
    public SerializedDictionary<StatEnum, AnimationCurve> statDistributions;
    public SerializedDictionary<DynamicStatEnum, AnimationCurve> dynamicStatDistributions;
}
