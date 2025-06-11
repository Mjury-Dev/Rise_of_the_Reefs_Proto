using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceGem : Pickup, ICollectible
{
    public int experienceGranted;

    public override void Collect()
    {
        AudioManager.Instance.PlaySFX("PickUpExp", transform.position);
        hasBeenCollected = true;
        PlayerStats player = FindObjectOfType<PlayerStats>();
        player.IncreaseExperience(experienceGranted);
    }
}
