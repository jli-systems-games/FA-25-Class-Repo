using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager liveManager;
    public static GameManager speedManager;

    public static event Action OnGameChange;
    public static event Action<Game> OnStartGame;

    public GameObject phil;

    public GameObject angryPhil;
    public GameObject cupGroup;
    public GameObject spottingGroup;
    public GameObject countingGroup;

    public CupGame cupGameScript;
    public SpottingGame spottingScript;
    public CountingGame countingScript;

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
        StartCoroutine(GameFlow());
    }

    private void Update()
    {
        if (currentLives <= 0)
        {
            StartCoroutine(GameOverRoutine());
        }
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
            case Game.CupGame:
                cupGroup.SetActive(true);
                cupGameScript.StartCupGame(speed);
                break;

            case Game.Spotting:
                spottingGroup.SetActive(true);
                spottingScript.StartSpottingGame(speed);
                break;

            case Game.Counting:
                countingGroup.SetActive(true);
                countingScript.StartCountingGame(speed);
                break;
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
        speed += 0.1f;
    }

    public float GetSpeed()
    {
        return speed;
    }

    IEnumerator GameFlow()
    {
        while (currentLives > 0)
        {
            Game game = (Game)UnityEngine.Random.Range(0, 3);
            StartSpecificGame(game);

            bool finished = false;
            bool success = false;
            Action<bool> onEnd = (result) => { finished = true; success = result; };
            GameBridge.OnGameEnd += onEnd;

            yield return new WaitUntil(() => finished);
            GameBridge.OnGameEnd -= onEnd;

            if (success) IncreaseSpeed();
            else LoseLife();

            yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator GameOverRoutine()
    {
        angryPhil.SetActive(true);

        yield return new WaitForSeconds(1.2f);

        SceneManager.LoadScene("Opening");
    }
}