using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextBook : Item
{
    public float textBookRadius = 15f;

    public override void applyItem(PlayerController playerController)
    {
        base.applyItem(playerController);
        playerController.textBookRadius += textBookRadius;
    }
}
