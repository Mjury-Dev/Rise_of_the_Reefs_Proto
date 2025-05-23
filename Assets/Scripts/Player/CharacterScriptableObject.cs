using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterScriptableObject", menuName = "ScriptableObjects/Character")]
public class CharacterScriptableObject : ScriptableObject
{
    [SerializeField]
    GameObject startingWeapon;
    public GameObject StartingWeapon { get => startingWeapon; set => startingWeapon = value; }

    [SerializeField]
    float maxHealth;
    public float MaxHealth{ get => maxHealth; set => maxHealth = value; }

    [SerializeField]
    float recovery;
    public float Recovery { get => recovery; set => recovery = value; }

    [SerializeField]
    float moveSpeed;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }

    [SerializeField]
    float strength;
    public float Strength { get => strength; set => strength = value; }
        
    [SerializeField]
    float projectileSpeed;
    public float ProjectileSpeed { get => projectileSpeed; set => projectileSpeed = value; }

    [SerializeField]
    float magnet;
    public float Magnet { get => magnet; set => magnet = value; }        
}
