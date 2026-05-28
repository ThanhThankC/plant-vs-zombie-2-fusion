using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChillEffect : IEffect
{
    public float Duration => 3f;

    public void OnApply(ZombieContext ctx)
    {
        //ctx.Zombie.Body.color = ctx.Zombie.IceColor;
    }

    public void OnExpire(ZombieContext ctx)
    {
        //ctx.Zombie.Body.color = ctx.Zombie.DefaultColor;
    }
}
