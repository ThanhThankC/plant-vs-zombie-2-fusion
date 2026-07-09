using System.Collections.Generic;

public class ApplyResult
{
    public bool ShouldApply { get; set; } = true;
    public List<System.Type> ToRemove { get; set; } = new();
}

public class ExpireResult
{
    public IEffect TriggerNext { get; set; } = null;
}

public static class EffectInteractionTable
{
    public static ApplyResult Resolve(Dictionary<System.Type, IEffect> current, IEffect incoming)
    {
        var result = new ApplyResult();

        if (incoming is BurnInstantEffect)
        {
            if (current.ContainsKey(typeof(FreezeEffect))) result.ToRemove.Add(typeof(FreezeEffect));
            if (current.ContainsKey(typeof(ChillEffect))) result.ToRemove.Add(typeof(ChillEffect));
            if (current.ContainsKey(typeof(PoisonEffect))) result.ToRemove.Add(typeof(PoisonEffect));
            return result;
        }

        if (incoming is BurnInfiniteEffect)
        {
            if (current.ContainsKey(typeof(FreezeEffect))) result.ToRemove.Add(typeof(FreezeEffect));
            if (current.ContainsKey(typeof(ChillEffect))) result.ToRemove.Add(typeof(ChillEffect));
            if (current.ContainsKey(typeof(PoisonEffect))) result.ToRemove.Add(typeof(PoisonEffect));
            return result;
        }

        if (incoming is FreezeEffect)
        {
            if (current.ContainsKey(typeof(BurnInstantEffect))) result.ToRemove.Add(typeof(BurnInstantEffect));
            if (current.ContainsKey(typeof(BurnInfiniteEffect))) result.ToRemove.Add(typeof(BurnInfiniteEffect));
            if (current.ContainsKey(typeof(ChillEffect))) result.ToRemove.Add(typeof(ChillEffect));
            return result;
        }

        if (incoming is PoisonEffect)
        {
            if (current.ContainsKey(typeof(BurnInstantEffect))) result.ToRemove.Add(typeof(BurnInstantEffect));
            if (current.ContainsKey(typeof(BurnInfiniteEffect))) result.ToRemove.Add(typeof(BurnInfiniteEffect));
            return result;
        }

        if (incoming is ChillEffect)
        {
            if (current.ContainsKey(typeof(FreezeEffect)))  return new ApplyResult { ShouldApply = false };
            if (current.ContainsKey(typeof(BurnInstantEffect))) result.ToRemove.Add(typeof(BurnInstantEffect));
            if (current.ContainsKey(typeof(BurnInfiniteEffect))) result.ToRemove.Add(typeof(BurnInfiniteEffect));
            return result;
        }

        return result;
    }

    public static ExpireResult OnExpired(Dictionary<System.Type, IEffect> current, IEffect expired)
    {
        if (expired is FreezeEffect freeze)
        {
            if (!current.ContainsKey(typeof(BurnInstantEffect)))
                return new ExpireResult { TriggerNext = new ChillEffect(freeze.ChillDuration) };
        }

        return new ExpireResult();
    }
}
