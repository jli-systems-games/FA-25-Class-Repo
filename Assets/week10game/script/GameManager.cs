using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public RoomGenerator roomGenerator;
    public ItemSpawner itemSpawner;
    public TargetItemManager targetItemManager;
    public TimerController timerController;


    public int targetsPerRound = 3;
    public float roundTimeSeconds = 60f;
    public float nextRoundDelay = 2f;

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
        // TODO: 성공 UI 띄우기

        // 다음 라운드로
        Invoke(nameof(GoNextRound), nextRoundDelay);
    }

    public void OnTimeOver()
    {
        if (!roundRunning) return;
        roundRunning = false;

        // TODO: 실패 UI 띄우기
        // 그래도 새 라운드
        Invoke(nameof(GoNextRound), nextRoundDelay);
    }

    void GoNextRound()
    {
        // 방/아이템 싹 정리
        roomGenerator.ClearRoom();
        itemSpawner.ClearItems();
        targetItemManager.ClearTargets();

        StartRound();
    }
}
