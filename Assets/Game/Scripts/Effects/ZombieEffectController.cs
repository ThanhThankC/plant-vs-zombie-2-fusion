using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieEffectController : MonoBehaviour
{
    private Dictionary<System.Type, Coroutine> activeCoroutines = new();
    private Dictionary<System.Type, Coroutine> activeTickCoroutines = new();
    private Dictionary<System.Type, IEffect> activeEffects = new();

    public ZombieStat Stat { get; private set; } = new ZombieStat();
    private ZombieContext context;
    private ILaneSwitchEffect activeLaneSwitchEffect;

    private void Awake()
    {
        ZombieBase zombie = GetComponent<ZombieBase>();
        context = new ZombieContext(zombie, Stat);
    }

    public void ApplyEffect(IEffect newEffect)
    {
        if (!ResolveInteraction(newEffect)) return;
        if (TryHandleInstant(newEffect)) return;

        RegisterEffect(newEffect);
        RefreshStats();
    }

    public void RemoveEffect(System.Type type)
    {
        if (activeTickCoroutines.TryGetValue(type, out Coroutine tick))
        {
            activeTickCoroutines.Remove(type);
            StopCoroutine(tick);
        }

        if (activeCoroutines.TryGetValue(type, out Coroutine routine))
        {
            activeCoroutines.Remove(type);
            StopCoroutine(routine);
        }

        if (activeEffects.TryGetValue(type, out IEffect effect))
        {
            activeEffects.Remove(type);
            effect.OnExpire(context);
            RefreshStats();
        }
    }

    private bool ResolveInteraction(IEffect newEffect)
    {
        var applyResult = EffectInteractionTable.Resolve(activeEffects, newEffect);
        if (!applyResult.ShouldApply) return false;

        foreach (var type in applyResult.ToRemove)
            RemoveEffect(type);

        return true;
    }

    private bool TryHandleInstant(IEffect newEffect)
    {
        if (newEffect.Duration == 0f)
        {
            newEffect.OnApply(context);
            newEffect.OnExpire(context);
            RefreshStats();
            return true;
        }
        return false;
    }

    private void RegisterEffect(IEffect newEffect)
    {
        var newType = newEffect.GetType();

        if (activeCoroutines.TryGetValue(newType, out Coroutine existing))
            StopCoroutine(existing);

        activeEffects[newType] = newEffect;
        newEffect.OnApply(context);

        if (newEffect.Duration > 0f)
            activeCoroutines[newType] = StartCoroutine(RunEffect(newEffect));

        if (newEffect is ILaneSwitchEffect laneSwitchEffect)
        {
            activeLaneSwitchEffect = laneSwitchEffect;
            //context.Zombie.Movement.OnLaneReached += OnLaneReached;
            //context.Zombie.Movement.SwitchLane();
        }

        if (newEffect is ITickableEffect && !activeTickCoroutines.ContainsKey(newType))
            activeTickCoroutines[newType] = StartCoroutine(TickRoutine((ITickableEffect)newEffect));
    }

    private IEnumerator RunEffect(IEffect effect)
    {
        yield return new WaitForSeconds(effect.Duration);

        var type = effect.GetType();

        if (!activeEffects.TryGetValue(type, out IEffect current) || current != effect)
            yield break;

        if (activeTickCoroutines.TryGetValue(type, out Coroutine tickRoutine))
        {
            StopCoroutine(tickRoutine);
            activeTickCoroutines.Remove(type);
        }

        activeCoroutines.Remove(type);
        activeEffects.Remove(type);

        var expireResult = EffectInteractionTable.OnExpired(activeEffects, effect);
        effect.OnExpire(context);

        if (expireResult.TriggerNext != null)
            ApplyEffect(expireResult.TriggerNext);
        else
            RefreshStats();
    }

    private IEnumerator TickRoutine(ITickableEffect effect)
    {
        yield return new WaitForSeconds(effect.FirstTickDelay);

        while (activeTickCoroutines.ContainsKey(effect.GetType()))
        {
            effect.OnTick(context);
            yield return new WaitForSeconds(effect.TickInterval);
        }
    }

    private void OnLaneReached()
    {
        //context.Zombie.Movement.OnLaneReached -= OnLaneReached;
        activeLaneSwitchEffect?.OnLaneReached(context);
        activeLaneSwitchEffect = null;
    }

    private void RefreshStats()
    {
        if (activeEffects.ContainsKey(typeof(FreezeEffect)) ||
            activeEffects.ContainsKey(typeof(ButterEffect)) ||
            activeEffects.ContainsKey(typeof(StinkyEffect)))
            context.Zombie.Movement.OnStun();
        else if (activeEffects.ContainsKey(typeof(ChillEffect)))
            context.Zombie.Movement.OnSlow(0.5f);
        else
            context.Zombie.Movement.ResetSpeed();
    }
}
