using UnityEngine;

public class ArmoredHeadZombie : BasicZombie
{
    [Header("Armor Skin")]
    [SerializeField] private string[] armorSkins;

    [Header("Armor Settings")]
    [SerializeField] private ZombiePoolKey armorPartKey;

    private int totalSkins;
    private int currentStageIndex;

    protected override void OnInit()
    {
        base.OnInit();
        totalSkins = armorSkins.Length;
        currentStageIndex = totalSkins - 1;
        SpineController.SetSkinActive(armorSkins[currentStageIndex], true);
    }

    public override void TakeDamage(int amount, DamageSource source = DamageSource.Normal)
    {
        base.TakeDamage(amount, source);
        if (ArmorHP <= 0) return;

        int newStageIndex = GetStageIndex();
        if(currentStageIndex == newStageIndex) return;

        SpineController.SetSkinActive(armorSkins[newStageIndex], true);
        SpineController.SetSkinActive(armorSkins[currentStageIndex], false);
        currentStageIndex = newStageIndex;
    }

    protected override void OnArmorBroken()
    {
        SpineController.SetSkinActive(armorSkins[currentStageIndex], false);

        onPartDropped?.Raise();

        var armor = PoolManager.Instance.GetZombie<BodyPart>(armorPartKey, headSpawnPoint.position, Quaternion.identity);
        armor.Init(GetGroundY(), 12);
    }

    private int GetStageIndex()
    {
        float ratio = (float)ArmorHP / Data.armorHP;
        int stage = Mathf.FloorToInt(ratio * totalSkins);
        return Mathf.Clamp(stage, 0, totalSkins - 1);
    }
}
