using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealPotion : Pickup, ICollectible
{
    public int healthToRestore;
    public override void Collect()
    {
        AudioManager.Instance.PlaySFX("PickUpHeal", transform.position);
        hasBeenCollected = true;
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.RestoreHealth(healthToRestore);
    }
}
