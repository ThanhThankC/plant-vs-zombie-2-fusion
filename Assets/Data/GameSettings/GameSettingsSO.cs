using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "PVZ/Game Settings")]
public class GameSettingsSO : ScriptableObject
{
    [Header("Sun")]
    public int startingSun = 100;
    public float sunDropInterval = 7f;
    [Header("Tools")]
    public bool shovelEnabled = true;
    public float shovelCooldown = 0f;
    public bool gloveEnabled = true;
    public float gloveCooldown = 2f;
}
