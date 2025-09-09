using UnityEngine;
using System;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    public static GameManager liveManager;
    public static GameManager speedManager;

    public static event Action OnGameChange;
    public static event Action<Game> OnStartGame;

    public GameObject cupGroup;
    public GameObject spottingGroup;
    public GameObject countingGroup;

    private int maxLives = 3;
    private int currentLives;
    private float speed=1;
    public GameObject[] lifeIcons;

    private void Awake()
    {
        if (liveManager != null && liveManager != this)
        {
            Destroy(gameObject);
            return;
        }
        liveManager = this;
        speedManager = this;
        currentLives = maxLives;
        UpdateLifeIcons();
    }

    private void Start()
    {
        OnGameChange?.Invoke();
    }

    public enum Game
    {
        CupGame,
        Spotting,
        Counting
    }

    public void ResetGroups()
    {
        cupGroup.SetActive(false);
        spottingGroup.SetActive(false);
        countingGroup.SetActive(false);
    }

    public void StartSpecificGame(Game game)
    {
        ResetGroups();
        switch (game)
        {
            case Game.CupGame: cupGroup.SetActive(true); break;
            case Game.Spotting: spottingGroup.SetActive(true); break;
            case Game.Counting: countingGroup.SetActive(true); break;
        }
        OnStartGame?.Invoke(game);
    }
    public void LoseLife()
    {
        if (currentLives <= 0) return;
        currentLives--;
        UpdateLifeIcons();
    }

    public void SetLives(int value)
    {
        currentLives = value;
        UpdateLifeIcons();
    }

    public int GetLives()
    {
        return currentLives;
    }

    private void UpdateLifeIcons()
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            lifeIcons[i].SetActive(i == currentLives - 1);
        }
    }

    public void IncreaseSpeed()
    {
        speed *= 1.1f;
    }

    public float GetSpeed()
    {
        return speed;
    }

    public void GameOver()
    {
        if (currentLives == 0)
            Debug.Log("game over!!!!!");
    }
}