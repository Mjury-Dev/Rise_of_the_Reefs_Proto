using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using Unity.VisualScripting;
using UnityEngine;

public class ShieldBehavior : ShieldWeaponBehavior
{
    private Vector3 playerPosition;
    List<GameObject> markedEnemies;

    protected override void Start()
    {
        base.Start();
        markedEnemies = new List<GameObject>();

        Vector3 playerPos = Camera.main.transform.position;

        float angle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = 2.2f;
        Vector3 offset = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * radius;
        transform.position = playerPos + offset + new Vector3(0f, 0f, 5f);
    }

    protected void Update()
    {
        Vector3 playerPosition = Camera.main.transform.position;
        transform.RotateAround(
            playerPosition,
            Vector3.forward,
            weaponData.OrbitSpeed * Time.deltaTime);
        transform.Rotate(0f, 0f, weaponData.SpinSpeed * Time.deltaTime);
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        if(col.CompareTag("Enemy") && !markedEnemies.Contains(col.gameObject))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.Hurt(currentDamage);

            markedEnemies.Add(col.gameObject);
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable) && !markedEnemies.Contains(col.gameObject))
            {
                breakable.TakeDamage(currentDamage);
                markedEnemies.Add(col.gameObject);
            }
        }

        
    }
}
