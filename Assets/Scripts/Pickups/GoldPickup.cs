using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldPickup : Pickup, ICollectible
{
    public int goldAmount;
    public override void Collect()
    {
        hasBeenCollected = true;
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.addGold(goldAmount);
    }
}
