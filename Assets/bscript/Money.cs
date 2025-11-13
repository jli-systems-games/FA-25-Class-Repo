using UnityEngine;
using TMPro;

public class MoneyManager : MonoBehaviour
{
    public int money = 100;
    public TMP_Text moneyText;

    void Start() => UpdateUI();

    public bool SpendMoney(int amount)
    {
        if (money >= amount)
        {
            money -= amount;
            UpdateUI();
            return true;
        }
        return false;
    }

    public void AddMoney(int amount)
    {
        money += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        moneyText.text = " " + money;
    }
}
