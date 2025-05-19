using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedMelee = Instantiate(weaponData.Prefab);
        spawnedMelee.transform.position = transform.position;
        spawnedMelee.transform.parent = transform;
    }
}
