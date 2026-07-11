using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlantEffectType
{
    None,
    CherryExplosion,
    JalapenoFire,
    PotatoMine,
    StormFreeze,
    BooExplosion,
}

[CreateAssetMenu(menuName = "Events/PlantEffectActivateEvent")]
public class PlantEffectActivateEvent : GameEventChannel<PlantEffectType> { }
