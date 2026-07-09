using System;
using System.Collections.Generic;
using System.Reflection;

public enum PlantType
{
    [PlantField(FieldType.Normal)] Peashooter, //a1
    [PlantField(FieldType.Normal)] Sunflower, //a2
    [PlantField(FieldType.Normal)] Wallnut, //a3
    [PlantField(FieldType.Normal)] Tallnut, //a4
    [PlantField(FieldType.Normal)] Frostflower,
    [PlantField(FieldType.Normal)] Repeater, //a6
    [PlantField(FieldType.Normal)] Splitpea, //a7
    [PlantField(FieldType.Normal)] Iceberg, //a8
    [PlantField(FieldType.Normal)] CherryBomb, //a9
    [PlantField(FieldType.Normal)] IceStorm, //a10
    [PlantField(FieldType.Normal)] PotatoMine, //a11
    [PlantField(FieldType.Support)] Pumpkin, //a12
    [PlantField(FieldType.Normal)] Jalapeno, //a13
    [PlantField(FieldType.Normal)] BooShroom, //a14
    [PlantField(FieldType.Normal)] Fireshooter, //a15
    [PlantField(FieldType.Normal)] Snowpea, //a16
    [PlantField(FieldType.Normal)] GooPeashooter, //a17
    [PlantField(FieldType.Normal)] Twinflower, //a18
    [PlantField(FieldType.Normal)] CabbagePult, //a19
    [PlantField(FieldType.Normal)] KernelPult, //a20
    [PlantField(FieldType.Normal)] MelonPult, //a21
    [PlantField(FieldType.Normal)] WinterMelon, //a22
    [PlantField(FieldType.Normal)] PepperPult, //a23
    [PlantField(FieldType.Normal)] Spikeweed, //a24
    [PlantField(FieldType.Normal)] Glaric, //a25
    [PlantField(FieldType.Normal)] Exonut, //a26
    [PlantField(FieldType.Normal)] Erudrion, 
    [PlantField(FieldType.Support)] PeaVine, //a28

    [PlantField(FieldType.Support)] GreenGourd, //test
    [PlantField(FieldType.Support)] XVine, //test
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

public enum DamageSource { Normal, Burn, Shock }

public enum EffectHitType { None, Freeze, Chill, Poison, Butter, Burn,}

public enum ZombieType 
{
    Basic,
    Conehead,
    Bucket,
    Newspaper,
    Gargantuar,
    Imp
}