using UnityEngine.Profiling;

public enum LayerType
{
    NormalPlant,
    SupportPlant,
    Zombie,
    Projectile,
    Splat,
    RearPlantEffect,
    TopPlantEffect,
    ZombieEffect,
    Shadow,
    Sun,
    GhostPlant,
    Tool
}

public static class SortingOrderUtility
{
    private const int maxRow = 4;
    private const int startOrder = 5;
    private const int shadowOrder = 1;
    private const int sunOrderSpace = 45;
    private const int ghostPlantOrderSpace = 48;
    private const int toolOrderSpace = 50;

    private const int rowOrderSpace = 20;
    private const int rearEffectPlantOrderSpace = 1;
    private const int normalPlantOrderSpace = 3;
    private const int supportPlantOrderSpace = 5;
    private const int zombieOrderSpace = 8;
    private const int zombieEffectOrderSpace = 10;
    private const int projectileOrderSpace = 12;
    private const int splatOrderSpace = 15;
    private const int topEffectPlantOrderSpace = 18;

    public static int GetSortingOrder(LayerType layer, int row)
    {
        int rowOrder = startOrder + (maxRow - row) * rowOrderSpace;
        return layer switch
        {
            LayerType.RearPlantEffect => rowOrder + rearEffectPlantOrderSpace,
            LayerType.NormalPlant => rowOrder  + normalPlantOrderSpace,
            LayerType.SupportPlant => rowOrder + supportPlantOrderSpace,
            LayerType.Zombie => rowOrder + zombieOrderSpace,
            LayerType.ZombieEffect => rowOrder + zombieEffectOrderSpace,
            LayerType.Projectile => rowOrder  + projectileOrderSpace,
            LayerType.Splat => rowOrder  + splatOrderSpace,
            LayerType.TopPlantEffect => rowOrder + topEffectPlantOrderSpace,
            _ => startOrder
        };
    }

    public static int GetSortingOrder(LayerType layer)
    {
        int maxOrder = maxRow * rowOrderSpace;
        return layer switch
        {
            LayerType.Shadow => shadowOrder,
            LayerType.Sun => maxOrder + sunOrderSpace,
            LayerType.GhostPlant => maxOrder + ghostPlantOrderSpace,
            LayerType.Tool => maxOrder + toolOrderSpace,
            _ => startOrder
        };
    }
}
