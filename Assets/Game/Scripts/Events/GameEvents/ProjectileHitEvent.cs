using UnityEngine;

public enum ProjectileHitType
{
    Pea,
    Snowpea,
    Goopea,
    Firepea,
    Cabbage,
    Corn,
    Butter,
    Melon,
    WinterMelon,
    Pepper,
    Plastic,
    Metal
}

[CreateAssetMenu(menuName = "Events/ProjectileHitEvent")]
public class ProjectileHitEvent : GameEventChannel<ProjectileHitType> { }