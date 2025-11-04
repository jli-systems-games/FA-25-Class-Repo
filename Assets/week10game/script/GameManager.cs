using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Refs")]
    public RoomGenerator roomGenerator;
    public ItemSpawner itemSpawner;
    public TargetItemManager targetItemManager;
    public TimerController timerController;

    [Header("Round Settings")]
    public int targetsPerRound = 3;
    public float roundTimeSeconds = 60f;
    public float nextRoundDelay = 2f;

    [Header("Scene Transition")]
    public bool manageScenesHere = true;          // 성공/실패 시 씬 전환을 여기서 처리할지
    public string successSceneName = "Success";   // 성공 씬 이름 (Build Settings에 추가 필수)
    public string gameOverSceneName = "Fail";

    bool roundRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        StartRound();
    }

    public void StartRound()
    {
        roundRunning = true;

        // 1) 방 싹 새로 만들기
        roomGenerator.GenerateRoom();

        // 2) 아이템 뿌리기
        List<InteractableItem> spawnedItems = itemSpawner.SpawnItems(roomGenerator.GetItemSpawnPoints());

        // 3) 그중에서 목표 3개 뽑기
        List<InteractableItem> targets = itemSpawner.ChooseTargets(spawnedItems, targetsPerRound);

        // 4) UI/매니저에 전달
        targetItemManager.SetTargets(targets);

        // 5) 타이머 시작
        timerController.StartTimer(roundTimeSeconds);
    }

    public void OnTargetAllFound()
    {
        if (!roundRunning) return;
        roundRunning = false;

        timerController.StopTimer();

        if (manageScenesHere && !string.IsNullOrEmpty(successSceneName))
        {
            // 성공 씬으로 이동
            SceneManager.LoadScene(successSceneName);
            return;
        }

        // 씬 전환을 안 쓰는 경우: 다음 라운드로 루프
        Invoke(nameof(GoNextRound), nextRoundDelay);
    }
    public void OnTimeOver()
    {
        if (!roundRunning) return;
        roundRunning = false;

        if (manageScenesHere && !string.IsNullOrEmpty(gameOverSceneName))
        {
            // 실패 씬으로 이동
            SceneManager.LoadScene(gameOverSceneName);
            return;
        }

        // 씬 전환을 안 쓰는 경우: 다음 라운드로 루프
        Invoke(nameof(GoNextRound), nextRoundDelay);
    }

    void GoNextRound()
    {
        // 방/아이템 정리
        roomGenerator.ClearRoom();
        itemSpawner.ClearItems();
        targetItemManager.ClearTargets();

        StartRound();
    }



}
