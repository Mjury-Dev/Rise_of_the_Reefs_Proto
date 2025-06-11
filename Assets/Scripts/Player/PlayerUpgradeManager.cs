using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUpgradeManager : MonoBehaviour
{
    public static PlayerUpgradeManager Instance;

    public int StrengthPoints { get; private set; }
    public int RecoveryPoints { get; private set; }
    public int SpeedPoints { get; private set; }
    public int MagnetPoints { get; private set; }
    public int HealthPoints { get; private set; }

    private readonly float[] strengthBonuses = { 0f, 0.1f, 0.2f, 0.3f, 0.5f };
    private readonly float[] recoveryBonuses = { 0f, 0.25f, 0.35f, 0.40f, 0.50f };
    private readonly float[] speedBonuses = { 0f, 0.5f, 1f, 1.5f, 2f };
    private readonly float[] magnetBonuses = { 0f, 1f, 1.4f, 2.3f, 3.6f };
    private readonly float[] healthBonuses = { 0f, 5f, 10f, 15f, 25f };

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadUpgrades();
    }

    public float GetUpgradeBonus(int type)
    {
        int level = GetPoints(type);
        return type switch
        {
            0 => strengthBonuses[Mathf.Clamp(level, 0, strengthBonuses.Length - 1)],
            1 => recoveryBonuses[Mathf.Clamp(level, 0, recoveryBonuses.Length - 1)],
            2 => speedBonuses[Mathf.Clamp(level, 0, speedBonuses.Length - 1)],
            3 => magnetBonuses[Mathf.Clamp(level, 0, magnetBonuses.Length - 1)],
            4 => healthBonuses[Mathf.Clamp(level, 0, healthBonuses.Length - 1)],
            _ => 0f
        };
    }

    public void AddPoint(int type)
    {
        switch (type)
        {
            case 0: StrengthPoints = Mathf.Min(StrengthPoints + 1, 4); break;
            case 1: RecoveryPoints = Mathf.Min(RecoveryPoints + 1, 4); break;
            case 2: SpeedPoints = Mathf.Min(SpeedPoints + 1, 4); break;
            case 3: MagnetPoints = Mathf.Min(MagnetPoints + 1, 4); break;
            case 4: HealthPoints = Mathf.Min(HealthPoints + 1, 4); break;
        }

        SaveUpgrades();
    }

    public int GetPoints(int type)
    {
        return type switch
        {
            0 => StrengthPoints,
            1 => RecoveryPoints,
            2 => SpeedPoints,
            3 => MagnetPoints,
            4 => HealthPoints,
            _ => 0
        };
    }

    void SaveUpgrades()
    {
        PlayerPrefs.SetInt("Strength", StrengthPoints);
        PlayerPrefs.SetInt("Recovery", RecoveryPoints);
        PlayerPrefs.SetInt("Speed", SpeedPoints);
        PlayerPrefs.SetInt("Magnet", MagnetPoints);
        PlayerPrefs.SetInt("Health", HealthPoints);
        PlayerPrefs.Save();
    }

    void LoadUpgrades()
    {
        StrengthPoints = PlayerPrefs.GetInt("Strength", 0);
        RecoveryPoints = PlayerPrefs.GetInt("Recovery", 0);
        SpeedPoints = PlayerPrefs.GetInt("Speed", 0);
        MagnetPoints = PlayerPrefs.GetInt("Magnet", 0);
        HealthPoints = PlayerPrefs.GetInt("Health", 0);
    }

    public void ResetUpgrades()
    {
        StrengthPoints = 0;
        RecoveryPoints = 0;
        SpeedPoints = 0;
        MagnetPoints = 0;
        HealthPoints = 0;
        SaveUpgrades();
    }
}