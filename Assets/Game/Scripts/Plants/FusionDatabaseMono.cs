public class FusionDatabaseMono : Singleton<FusionDatabaseMono>
{
    public static FusionDatabase DB { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        DB = new FusionDatabase();
    }
}
