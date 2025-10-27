using UnityEngine;
using UnityEngine.SceneManagement;
using System;

public class GameState : MonoBehaviour
{
    public static GameState Instance { get; private set; }

    public int Happy = 25;
    public int Hunger = 25;
    public int Health   = 25;

    public int minValue = 0;
    public int maxValue = 120;

    public string gameOverScene = "GameOver";
    public string gameWinScene = "GameWin";
    private bool autoReload = false;
    public event Action<int,int,int> OnChanged;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Add(int dhappy, int dhunger, int dhealth)
    {
        Happy = Mathf.Clamp(Happy + dhappy, minValue, maxValue);
        Hunger = Mathf.Clamp(Hunger + dhunger, minValue, maxValue);
        Health = Mathf.Clamp(Health + dhealth, minValue, maxValue);

        OnChanged?.Invoke(Happy, Hunger, Health);
        CheckFor0();
        CheckFor1000();
    }

    public void ForceSync()
    {
        OnChanged?.Invoke(Happy, Hunger, Health);
    }

    public void ResetAll(int happy = 25, int hunger = 25, int health = 25)
    {
        Happy = Mathf.Clamp(happy, minValue, maxValue);
        Hunger = Mathf.Clamp(hunger, minValue, maxValue);
        Health = Mathf.Clamp(health, minValue, maxValue);
        OnChanged?.Invoke(Happy, Hunger, Health);
    }

    void CheckFor0()
    {
        if (autoReload) return;

        if (Hunger <= 0 || Health <= 0 || Happy <= 0)
        {
            autoReload = true;
            SceneManager.LoadScene(gameOverScene);
        }
    }
        void CheckFor1000()
    {
        if (autoReload) return;

        if (Hunger >= 100 && Health >= 100 && Happy >= 100)
        {
            autoReload = true;
            SceneManager.LoadScene(gameWinScene);
        }
    }
}