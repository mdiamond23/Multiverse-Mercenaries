using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindItem : Item
{
    public float forceModifier = 1500f;
    public float speedModifier = 1.75f;

    public override void applyItem(PlayerController playerController)
    {
        base.applyItem(playerController);
        playerController.jumpModifier += forceModifier;
        playerController.moveSpeed *= speedModifier;

    }
}