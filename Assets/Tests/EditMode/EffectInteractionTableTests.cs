#if UNITY_EDITOR
using System;
using NUnit.Framework;
using System.Collections.Generic;

[TestFixture]
public class EffectInteractionTableTests
{
    private Dictionary<Type, IEffect> current;

    [SetUp]
    public void SetUp()
    {
        current = new Dictionary<Type, IEffect>();
    }

    // BLOCK
    [Test]
    public void Freeze_WhenChillApplied_ShouldBlock()
    {
        current.Add(typeof(FreezeEffect), new FreezeEffect());
        var result = EffectInteractionTable.Resolve(current, new ChillEffect());
        Assert.IsFalse(result.ShouldApply);
    }

    // SINGLE REMOVE
    [TestCase(typeof(FreezeEffect), typeof(BurnInstantEffect))]
    [TestCase(typeof(FreezeEffect), typeof(BurnInfiniteEffect))]
    [TestCase(typeof(ChillEffect), typeof(BurnInstantEffect))]
    [TestCase(typeof(ChillEffect), typeof(BurnInfiniteEffect))]
    [TestCase(typeof(PoisonEffect), typeof(BurnInstantEffect))]
    [TestCase(typeof(PoisonEffect), typeof(BurnInfiniteEffect))]
    [TestCase(typeof(BurnInfiniteEffect), typeof(FreezeEffect))]
    [TestCase(typeof(BurnInfiniteEffect), typeof(ChillEffect))]
    [TestCase(typeof(ChillEffect), typeof(FreezeEffect))]
    public void SingleExisting_WhenIncomingApplied_ShouldRemoveExisting(Type existingType, Type incomingType)
    {
        current.Add(existingType, (IEffect)Activator.CreateInstance(existingType));
        var incoming = (IEffect)Activator.CreateInstance(incomingType);
        var result = EffectInteractionTable.Resolve(current, incoming);

        Assert.IsTrue(result.ShouldApply);
        Assert.AreEqual(1, result.ToRemove.Count);
        CollectionAssert.Contains(result.ToRemove, existingType);
    }

    // MULTIPLE REMOVE
    [Test]
    public void BurnInstant_WhenFreezeChillPoison_ShouldRemoveAll()
    {
        current.Add(typeof(FreezeEffect), new FreezeEffect());
        current.Add(typeof(ChillEffect), new ChillEffect());
        current.Add(typeof(PoisonEffect), new PoisonEffect());

        var result = EffectInteractionTable.Resolve(current, new BurnInstantEffect());

        Assert.IsTrue(result.ShouldApply);
        CollectionAssert.AreEquivalent(
            new[] { typeof(FreezeEffect), typeof(ChillEffect), typeof(PoisonEffect) },
            result.ToRemove
        );
    }

    [Test]
    public void BurnInfinite_WhenFreezeAndChill_ShouldRemoveBoth()
    {
        current.Add(typeof(FreezeEffect), new FreezeEffect());
        current.Add(typeof(ChillEffect), new ChillEffect());

        var result = EffectInteractionTable.Resolve(current, new BurnInfiniteEffect());

        Assert.IsTrue(result.ShouldApply);
        CollectionAssert.AreEquivalent(
            new[] { typeof(FreezeEffect), typeof(ChillEffect) },
            result.ToRemove
        );
    }

    [Test]
    public void Freeze_WhenChillAndBurnInfinite_ShouldRemoveBoth()
    {
        current.Add(typeof(ChillEffect), new ChillEffect());
        current.Add(typeof(BurnInfiniteEffect), new BurnInfiniteEffect());

        var result = EffectInteractionTable.Resolve(current, new FreezeEffect());

        Assert.IsTrue(result.ShouldApply);
        CollectionAssert.AreEquivalent(
            new[] { typeof(ChillEffect), typeof(BurnInfiniteEffect) },
            result.ToRemove
        );
    }

    // COEXIST
    [TestCase(typeof(FreezeEffect), typeof(PoisonEffect))]
    [TestCase(typeof(FreezeEffect), typeof(ButterEffect))]
    [TestCase(typeof(FreezeEffect), typeof(StinkyEffect))]
    [TestCase(typeof(ChillEffect), typeof(PoisonEffect))]
    [TestCase(typeof(ChillEffect), typeof(ButterEffect))]
    [TestCase(typeof(ChillEffect), typeof(StinkyEffect))]
    [TestCase(typeof(PoisonEffect), typeof(ButterEffect))]
    [TestCase(typeof(PoisonEffect), typeof(StinkyEffect))]
    [TestCase(typeof(ButterEffect), typeof(StinkyEffect))]
    public void ExistingEffect_WhenCoexistIncoming_ShouldApplyWithNoRemoval(Type existingType, Type incomingType)
    {
        current.Add(existingType, (IEffect)Activator.CreateInstance(existingType));
        var incoming = (IEffect)Activator.CreateInstance(incomingType);
        var result = EffectInteractionTable.Resolve(current, incoming);

        Assert.IsTrue(result.ShouldApply);
        Assert.IsEmpty(result.ToRemove);
    }

    // NO EXISTING 
    [TestCase(typeof(FreezeEffect))]
    [TestCase(typeof(ChillEffect))]
    [TestCase(typeof(PoisonEffect))]
    [TestCase(typeof(BurnInstantEffect))]
    [TestCase(typeof(BurnInfiniteEffect))]
    [TestCase(typeof(ButterEffect))]
    [TestCase(typeof(StinkyEffect))]
    public void NoExistingEffect_AnyIncoming_ShouldApply(Type incomingType)
    {
        var incoming = (IEffect)Activator.CreateInstance(incomingType);
        var result = EffectInteractionTable.Resolve(current, incoming);

        Assert.IsTrue(result.ShouldApply);
        Assert.IsEmpty(result.ToRemove);
    }

    // ON EXPIRED
    [Test]
    public void Freeze_OnExpired_ShouldTriggerChill()
    {
        var result = EffectInteractionTable.OnExpired(current, new FreezeEffect());
        Assert.IsNotNull(result.TriggerNext);
        Assert.IsInstanceOf<ChillEffect>(result.TriggerNext);
    }

    [Test]
    public void Freeze_OnExpired_WhenBurnActive_ShouldNotTriggerChill()
    {
        current.Add(typeof(BurnInstantEffect), new BurnInstantEffect());
        var result = EffectInteractionTable.OnExpired(current, new FreezeEffect());
        Assert.IsNull(result.TriggerNext);
    }

    [TestCase(typeof(ChillEffect))]
    [TestCase(typeof(PoisonEffect))]
    [TestCase(typeof(ButterEffect))]
    [TestCase(typeof(StinkyEffect))]
    [TestCase(typeof(BurnInstantEffect))]
    [TestCase(typeof(BurnInfiniteEffect))]
    public void OtherEffects_OnExpired_ShouldNotTrigger(Type expiredType)
    {
        var expired = (IEffect)Activator.CreateInstance(expiredType);
        var result = EffectInteractionTable.OnExpired(current, expired);
        Assert.IsNull(result.TriggerNext);
    }
}
#endif