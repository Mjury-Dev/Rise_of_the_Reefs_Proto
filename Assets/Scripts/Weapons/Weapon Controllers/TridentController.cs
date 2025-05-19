using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TridentController : WeaponController
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Attack()
    {
        base.Attack();
        GameObject spawnedTrident = Instantiate(weaponData.Prefab);
        spawnedTrident.transform.position = transform.position;
        spawnedTrident.transform.parent = transform;
        spawnedTrident.GetComponent<TridentBehavior>().DirectionChecker(pm.lastMovedVector);
    }
}
