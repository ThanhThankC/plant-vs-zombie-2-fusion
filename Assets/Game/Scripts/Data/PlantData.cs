using UnityEngine;

public enum PlantBehaviorType { Normal, Instant, SemiInstant }

[CreateAssetMenu(fileName = "PlantData", menuName = "PVZF/Plant Data")]
public class PlantData : ScriptableObject
{
    [Header("Identity")]
    public string plantName;
    public int plantID;
    public PlantType plantType;
    public int tier = 1;

    [Header("Stats")]
    public int maxHP = 300;
    public int sunCost;
    public float firstcooldown;
    public float cooldown;

    [Header("Behavior")]
    public PlantBehaviorType behaviorType;

    [Header("UI")]
    public Sprite cardSprite;
}


