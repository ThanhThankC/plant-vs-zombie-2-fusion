using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct FusionKey : IEquatable<FusionKey>
{
    public readonly PlantType A;
    public readonly PlantType B;

    public FusionKey(PlantType a, PlantType b)
    {
        A = (int)a <= (int)b ? a : b;
        B = (int)a <= (int)b ? b : a;
    }

    public bool Equals(FusionKey other) => A == other.A && B == other.B;
    public override bool Equals(object obj) => obj is FusionKey other && Equals(other);
    public override int GetHashCode() => HashCode.Combine(A, B);
}

public class FusionDatabase
{
    private Dictionary<FusionKey, PlantType> recipes;

    public FusionDatabase()
    {
        recipes = new Dictionary<FusionKey, PlantType>
        {
            { new FusionKey(PlantType.Peashooter, PlantType.Peashooter), PlantType.Repeater },
            { new FusionKey(PlantType.Peashooter, PlantType.Repeater), PlantType.Splitpea },
            { new FusionKey(PlantType.Peashooter, PlantType.IceStorm), PlantType.Snowpea },
            { new FusionKey(PlantType.Peashooter, PlantType.Jalapeno), PlantType.Fireshooter },
            { new FusionKey(PlantType.Peashooter, PlantType.BooShroom), PlantType.GooPeashooter },
            { new FusionKey(PlantType.Sunflower, PlantType.Sunflower), PlantType.Twinflower },
            { new FusionKey(PlantType.Iceberg, PlantType.Iceberg), PlantType.IceStorm },
            { new FusionKey(PlantType.Wallnut, PlantType.Wallnut), PlantType.Tallnut },
            { new FusionKey(PlantType.CherryBomb, PlantType.Wallnut), PlantType.Exonut },
            { new FusionKey(PlantType.CabbagePult, PlantType.Jalapeno), PlantType.PepperPult },
            { new FusionKey(PlantType.MelonPult, PlantType.IceStorm), PlantType.WinterMelon },
            { new FusionKey(PlantType.Pumpkin, PlantType.Peashooter), PlantType.PeaVine },

            //test
            { new FusionKey(PlantType.GreenGourd, PlantType.GreenGourd), PlantType.XVine },
        };
    }

    public PlantType? GetPlantResult(PlantType incoming, PlantType? primaryExisting, PlantType? otherExisting)
    {
        if (!primaryExisting.HasValue && !otherExisting.HasValue) return incoming;

        if (!primaryExisting.HasValue || !otherExisting.HasValue)
        {
            var existingPlant = primaryExisting ?? otherExisting;

            if (incoming.GetFieldType() != existingPlant.Value.GetFieldType())
                return incoming;

            if (recipes.TryGetValue(new FusionKey(incoming, existingPlant.Value), out var fused))
                return fused;

            return null;
        }

        if (CanAttemptFuse(incoming, primaryExisting.Value))
            if (recipes.TryGetValue(new FusionKey(incoming, primaryExisting.Value), out var fusedPrimary))
                return fusedPrimary;

        if (CanAttemptFuse(incoming, otherExisting.Value))
            if (recipes.TryGetValue(new FusionKey(incoming, otherExisting.Value), out var fusedOther))
                return fusedOther;

        return null;
    }

    private bool CanAttemptFuse(PlantType incoming, PlantType existing)
    {
        return incoming.GetFieldType() == FieldType.Normal
            || incoming.GetFieldType() == existing.GetFieldType();
    }
}
