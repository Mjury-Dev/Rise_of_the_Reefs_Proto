using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldWeaponBehavior : MonoBehaviour
{
    public WeaponScriptableObject weaponData;

    protected float currentDamage;
    protected float currentCooldownDuration;
    protected float spinSpeed;
    protected float orbitSpeed;

    public float GetCurrentDamage()
    {
        return currentDamage *= FindObjectOfType<PlayerStats>().CurrentStrength;
    }

    private void Awake()
    {
        currentDamage = weaponData.Damage;
        currentCooldownDuration = weaponData.CooldownDuration;
        spinSpeed = weaponData.SpinSpeed;
        orbitSpeed = weaponData.OrbitSpeed;
    }

    public float destroyAfterSeconds;
    protected virtual void Start()
    {
        Destroy(gameObject, destroyAfterSeconds);
    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            EnemyStats enemy = col.GetComponent<EnemyStats>();
            enemy.Hurt(GetCurrentDamage(), transform.position);
        }
        else if (col.CompareTag("Prop"))
        {
            if (col.gameObject.TryGetComponent(out BreakableProps breakable))
            {
                breakable.TakeDamage(GetCurrentDamage());
            }
        }
    }
}
