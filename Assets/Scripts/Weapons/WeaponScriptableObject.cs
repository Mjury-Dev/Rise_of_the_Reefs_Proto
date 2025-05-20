using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponScriptableObject", menuName = "ScriptableObjects/Weapon")]
public class WeaponScriptableObject : ScriptableObject
{
    [SerializeField]
    GameObject prefab;
    public GameObject Prefab { get => prefab; private set => prefab = value; }

    [SerializeField]
    float damage;
    public float Damage { get =>  damage; set => damage = value; }

    [SerializeField] 
    float speed;
    public float Speed { get => speed; set => speed = value; }

    [SerializeField]
    float cooldownDuration;
    public float CooldownDuration { get => cooldownDuration; set => cooldownDuration = value; }

    [SerializeField]
    float spinSpeed;
    public float SpinSpeed { get => spinSpeed; set => spinSpeed = value; }

    [SerializeField]
    float orbitSpeed;
    public float OrbitSpeed { get => orbitSpeed; set => orbitSpeed = value; }

    [SerializeField]
    int pierce;
    public int Pierce { get => pierce; set => pierce = value; }

    [SerializeField]
    int bounce;
    public int Bounce { get => bounce; set => bounce = value; }

    [SerializeField]
    int level;
    public int Level { get => level; set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

}
