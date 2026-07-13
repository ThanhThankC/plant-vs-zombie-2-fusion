using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExoNut : PlantNutBase
{
    protected override void Die(ZombieBase zombie = null)
    {
        PlantActivator.Instance.Activate(PlantType.CherryBomb, OccupiedCell);
        base.Die(zombie);
    }
}
