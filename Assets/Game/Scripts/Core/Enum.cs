using System;
using System.Collections.Generic;
using System.Reflection;

public enum PlantType
{
    [PlantField(FieldType.Normal)] Sunflower,
    [PlantField(FieldType.Normal)] Twinflower,
    [PlantField(FieldType.Normal)] Peashooter,
    [PlantField(FieldType.Normal)] Repeater,
    [PlantField(FieldType.Normal)] Splitpea,
    [PlantField(FieldType.Normal)] GatlingPea,
    [PlantField(FieldType.Support)] PeaVine,
    [PlantField(FieldType.Support)] Pumpkin,
    [PlantField(FieldType.Support)] GreenGourd,
    [PlantField(FieldType.Support)] XVine,
}

[AttributeUsage(AttributeTargets.Field)]
public class PlantFieldAttribute : Attribute
{
    public FieldType FieldType { get; }
    public PlantFieldAttribute(FieldType fieldType) => FieldType = fieldType;
}

public static class PlantTypeExtensions
{
    private static readonly Dictionary<PlantType, FieldType> cache;

    static PlantTypeExtensions()
    {
        cache = new Dictionary<PlantType, FieldType>();
        foreach (PlantType p in Enum.GetValues(typeof(PlantType)))
        {
            var field = typeof(PlantType).GetField(p.ToString());
            var attr = field?.GetCustomAttribute<PlantFieldAttribute>();
            cache[p] = attr?.FieldType ?? FieldType.Normal;
        }
    }
    public static FieldType GetFieldType(this PlantType plantType) => cache[plantType];
}

public enum CellType
{
    Plantable,
    MowerZone,
    BorderZone,
    ZombieSpawn,
}

public enum ToolType { None, Glove, Shovel }

public enum GloveState { Idle, PlantSelected }