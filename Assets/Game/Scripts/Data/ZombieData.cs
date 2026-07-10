using UnityEngine;

[CreateAssetMenu(fileName = "ZombieData", menuName = "PVZF/Zombie Data")]
public class ZombieData : ScriptableObject
{
    [Header("Identity")]
    public string zombieName;
    public int zombieID;
    public ZombieType zombieType;

    [Header("Stats")]
    public int maxHP;
    public int armorHP;
    public int attackDamage;
    public float baseSpeedMultiplier;
    public bool isEatAnim;

    [Header("Resistances")]
    public bool immuneToFreeze;
    public bool weakToElectric;

    [Header("UI")]
    public Sprite cardSprite;
}


