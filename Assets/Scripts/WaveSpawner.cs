using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour {

	public static WaveSpawner Instance;

	public static int EnemiesAlive = 0;

	public Wave[] waves;

	public Transform spawnPoint;

	public float timeBetweenWaves = 3f;  // 波次间隔改为3秒
	private float countdown = 0f;

	public GameManager gameManager;

	private int waveIndex = 0;
	private int wavesPerStage = 5;  // 每阶段5波
	private bool isSpawning = false;
	private bool stageActive = false;  // 当前阶段是否激活
	private int lastDisplayedWave = -1;  // 上次显示的波次号
	private int lastCompletedStageWave = -1;  // 上次触发休息时的波次号（防止重复触发）

	void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
		else
		{
			Destroy(gameObject);
		}
	}

	void Start()
	{
		// 游戏开始时不自动播放，等待玩家点击开始
		stageActive = false;
		countdown = 0f;
		
		// 初始阶段信息由GamePhaseManager管理，不在这里设置
	}
	
	/// <summary>
	/// 重置WaveSpawner状态（场景重载时调用）
	/// </summary>
	public void ResetWaveSpawner()
	{
		waveIndex = 0;
		stageActive = false;
		isSpawning = false;
		countdown = 0f;
		lastDisplayedWave = -1;
		lastCompletedStageWave = -1;
		EnemiesAlive = 0;
	}

	void Update ()
	{
		if (!stageActive)
			return;

		if (EnemiesAlive > 0 || isSpawning)
		{
			return;
		}

		// 检查是否完成所有波次
		if (waveIndex == waves.Length)
		{
			gameManager.WinLevel();
			this.enabled = false;
			return;
		}

		// 检查是否完成当前阶段（每5波）
		// 添加lastCompletedStageWave检查，防止重复触发
		if (waveIndex % wavesPerStage == 0 && waveIndex > 0 && waveIndex != lastCompletedStageWave)
		{
			// 完成一个阶段，进入休息
			stageActive = false;
			lastDisplayedWave = -1;  // 重置显示状态
			lastCompletedStageWave = waveIndex;  // 记录已触发休息的波次
			UpdateRestInfo();  // 显示休息信息
			GamePhaseManager.Instance?.OnStageCompleted();
			return;
		}

		// 只在波次号改变时更新Phase信息
		if (lastDisplayedWave != waveIndex)
		{
			UpdatePhaseInfo();
			lastDisplayedWave = waveIndex;
		}

		// 倒计时到下一波
		if (countdown <= 0f)
		{
			StartCoroutine(SpawnWave());
			countdown = timeBetweenWaves;
			return;
		}

		countdown -= Time.deltaTime;
		countdown = Mathf.Clamp(countdown, 0f, Mathf.Infinity);
	}

	// 开始下一个阶段（5波）
	public void StartNextStage()
	{
		stageActive = true;
		countdown = 0f;
		lastDisplayedWave = -1;  // 重置显示状态，确保立即更新
		
		UpdatePhaseInfo();
		
		// 音乐系统控制
		if (MusicManager.Instance != null)
		{
			if (waveIndex == 0)
			{
				// 第一波：播放第一波音乐
				MusicManager.Instance.PlayFirstWaveMusic();
			}
			else if (waveIndex >= wavesPerStage)
			{
				// 第一次休息后（第6波及以后）：播放主战斗音乐
				MusicManager.Instance.PlayMainBattleMusic();
			}
		}
	}

	// 更新阶段信息（游戏进行中）
	public void UpdatePhaseInfo()
	{
		// 显示真实的波次号（1-15），而不是阶段号（1-3）
		int currentWave = waveIndex + 1;
		int totalWaves = waves.Length;
		gameManager?.UpdatePhaseInfo($"Phase: {currentWave} of {totalWaves}");
	}

	// 更新休息信息
	public void UpdateRestInfo()
	{
		// 休息时显示当前进度
		gameManager?.UpdatePhaseInfo($"Rest Time ({waveIndex}/{waves.Length})");
	}

	// 获取当前波次信息（给GameManager用）
	public int GetCurrentWave()
	{
		return waveIndex + 1;
	}

	public int GetTotalWaves()
	{
		return waves.Length;
	}

	IEnumerator SpawnWave ()
	{
		isSpawning = true;
		PlayerStats.Rounds++;

		Wave wave = waves[waveIndex];
		
		// 计算总敌人数
		int totalEnemies = 0;
		foreach (EnemySpawn enemySpawn in wave.enemies)
		{
			totalEnemies += enemySpawn.count;
		}
		EnemiesAlive = totalEnemies;

		// 生成所有敌人类型
		foreach (EnemySpawn enemySpawn in wave.enemies)
		{
			for (int i = 0; i < enemySpawn.count; i++)
			{
				SpawnEnemy(enemySpawn.enemy);
				yield return new WaitForSeconds(1f / wave.rate);
			}
		}

		waveIndex++;
		// 不在这里更新PhaseInfo，让Update中的逻辑处理
		isSpawning = false;
	}

	void SpawnEnemy (GameObject enemy)
	{
		Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
	}

}
