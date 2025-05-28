using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public TextMeshProUGUI currencyText;

    private void Start()
    {
        // Load saved currency from PlayerPrefs (default to 0 if not found)
        int savedCurrency = PlayerPrefs.GetInt("Currency", 0);

        // Update the TMP text with the saved currency value
        currencyText.text = "Gold:" + savedCurrency.ToString();
    }
}