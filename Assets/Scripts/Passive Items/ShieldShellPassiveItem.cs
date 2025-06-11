using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldShellPassiveItem : PassiveItem
{
    protected override void ApplyModifier()
    {
        player.MaxHealth += passiveItemData.Multiplier;
    }
}
