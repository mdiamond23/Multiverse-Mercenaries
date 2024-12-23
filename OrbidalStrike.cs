using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbidalStrike : Item
{
   public float strikeChance = .33f;

    public override void applyItem(PlayerController playerController)
    {
        base.applyItem(playerController);
        playerController.strikeChance += Mathf.Min(strikeChance,1);
    }
}
