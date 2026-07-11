using UnityEngine;

public enum PlantAttackType
{
    None,
    Normal,
    Goopea,
    Melon,
    Spikeweed,
    Iceberg,
}

[CreateAssetMenu(menuName = "Events/PlantAttackEvent")]

public class PlantAttackEvent : GameEventChannel<PlantAttackType> { }