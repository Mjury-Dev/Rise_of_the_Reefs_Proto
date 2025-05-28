using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager instance;

    [SerializeField]
    private int currency;
    public int Currency { get => currency; private set => currency = value; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCurrency();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCurrency(int amount)
    {
        Currency += amount;
    }

    public bool SpendCurrency(int amount)
    {
        if (Currency >= amount)
        {
            Currency -= amount;
            return true;
        }
        return false;
    }

    public void SaveCurrency()
    {
        PlayerPrefs.SetInt("Currency", Currency);
        PlayerPrefs.Save();
    }

    public void LoadCurrency()
    {
        Currency = PlayerPrefs.GetInt("Currency", 0);
    }

    private void OnApplicationQuit()
    {
        SaveCurrency();
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
            SaveCurrency();
    }
}
