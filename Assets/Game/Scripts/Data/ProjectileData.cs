using UnityEngine;

[CreateAssetMenu(fileName = "ProjectileData", menuName = "PVZF/Projectile Data")]
public class ProjectileData : ScriptableObject
{
    [Header("Stats")]
    public DamageSource damageSource;
    public int damage;
    public bool hasEffect;

    public float linearSpeed; 
    public float arcHeight;
    public float arcDuration;

    [Header("AOE Stats")]
    public int rangeRow;
    public int aoeDamage;
}
