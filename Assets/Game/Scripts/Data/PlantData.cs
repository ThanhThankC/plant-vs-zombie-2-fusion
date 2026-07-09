using UnityEngine;

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
    public bool isInvincible;

    [Header("UI")]
    public Sprite cardSprite;

    [Header("Effect Stats")]
    public EffectHitType effectHitType;
    public float effectDuration;
    public float expireEffectDuration;
    public float effectDieDuration;
    public int damagePerTick;

    [Header("Other Stats")]
    public DamageSource damageSource;
    public int rangeRow;
    public int aoeDamage;
    public bool notBeEaten;

    public IEffect CreateOnHitEffect(bool isOnDie = false)
    {
        return effectHitType switch
        {
            EffectHitType.Freeze => new FreezeEffect(effectDuration, expireEffectDuration),
            EffectHitType.Burn => new BurnInstantEffect(),
            EffectHitType.Chill => new ChillEffect(effectDuration),
            EffectHitType.Butter => new ButterEffect(effectDuration),
            EffectHitType.Poison => new PoisonEffect(damagePerTick, !isOnDie ? effectDuration : effectDieDuration),
            _ => null
        };
    }
}


