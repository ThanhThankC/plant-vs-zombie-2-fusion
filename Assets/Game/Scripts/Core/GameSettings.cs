using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSettings : PersistentSingleton<GameSettings>
{
    public static float MasterSfxVolume = 1f;

    public static int SelectedLevel = 0;

    public static string TransitionTargetScene;
}
