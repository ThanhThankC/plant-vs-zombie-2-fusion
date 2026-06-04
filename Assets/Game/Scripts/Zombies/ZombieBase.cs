using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;

[RequireComponent(typeof(ZombieMovement))]
[RequireComponent(typeof(ZombieEffectController))]
[RequireComponent(typeof(ZombieSpineController))]
[RequireComponent(typeof(CellTracker))]
public abstract class ZombieBase : MonoBehaviour
{
    public ZombieData Data { get; private set; }
    public ZombieMovement Movement { get; private set; }
    public ZombieEffectController EffectController { get; private set; }
    public CellTracker CellTracker { get; private set; }

    public int CurrentHP { get; private set; }
    public int ArmorHP { get; private set; }
    public bool IsDead { get; private set; }
    public int Row { get; private set; }

    protected virtual void Awake()
    {
        Movement = GetComponent<ZombieMovement>();
        EffectController = GetComponent<ZombieEffectController>();
        CellTracker = GetComponent<CellTracker>();
    }

    public void Init(ZombieData data)
    {
        Data = data;
        CurrentHP = data.maxHP;
        ArmorHP = data.armorHP;
        OnInit();
    }

    protected virtual void OnInit() { }

    public virtual void TakeDamage(int amount, DamageSource source = DamageSource.Normal)
    {
        if (IsDead) return;

        if (ArmorHP > 0)
        {
            ArmorHP -= amount;
            if (ArmorHP <= 0)
            {
                ArmorHP = 0;
                OnArmorBroken();
            }
            return;
        }

        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die(source);
        }
    }

    protected virtual void OnArmorBroken() { }

    protected virtual void Die(DamageSource source)
    {
        if (IsDead) return;
        IsDead = true;
        //TODO: Notify to ZombieManager
        OnDie(source);
    }

    protected virtual void OnDie(DamageSource source)
    {
        Destroy(gameObject,0.5f);
    }

    public void SetBodyColor()
    {

    }

    //public void ApplyFreeze() => EffectController.ApplyEffect(new FreezeEffect());
}
