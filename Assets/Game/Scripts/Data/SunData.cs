using UnityEngine;

[CreateAssetMenu(fileName = "SunData", menuName ="PVZF/SunData")]
public class SunData : ScriptableObject
{
    public int startingSun = 100;
    public bool autoSpawn = true;
    public float firstDuration = 7f;
    public float interval = 7f;
}
