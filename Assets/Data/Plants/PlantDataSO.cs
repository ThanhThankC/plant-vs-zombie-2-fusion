using UnityEngine;

[CreateAssetMenu(fileName = "NewPlant", menuName = "PVZ/Plant Data")]

public class PlantDataSO : ScriptableObject
{
    public string plantName;
    public int cost = 100;
    public float cooldown = 7f;
    public int maxHP = 300;
    public GameObject prefab;
    public Sprite icon;
}
