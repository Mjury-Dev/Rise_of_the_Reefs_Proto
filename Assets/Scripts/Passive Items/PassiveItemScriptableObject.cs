using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PassiveItemScriptableObject", menuName = "ScriptableObjects/Passive Item")]
public class PassiveItemScriptableObject : ScriptableObject
{
    [SerializeField]
    float multiplier;
    public float Multiplier { get => multiplier; set => multiplier = value; }

    [SerializeField]
    int level;
    public int Level { get => level; set => level = value; }

    [SerializeField]
    GameObject nextLevelPrefab;
    public GameObject NextLevelPrefab { get => nextLevelPrefab; set => nextLevelPrefab = value; }

    [SerializeField]
    Sprite icon;
    public Sprite Icon { get => icon; set => icon = value; }

    [SerializeField]
    string passiveItemName;
    public string PassiveItemName { get => name; set => name = value; }

    [SerializeField]
    string description;
    public string Description { get => description; set => description = value; }
}
