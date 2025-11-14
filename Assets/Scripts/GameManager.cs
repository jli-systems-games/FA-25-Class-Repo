using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameManager : MonoBehaviour {

	public static bool GameIsOver;

	public GameObject gameOverUI;
	public GameObject completeLevelUI;

	[Header("UI Elements")]
	public Text phaseInfoText;  // 显示 Phase X of Y 或 Rest Time (X/Y)

	void Start ()
	{
		GameIsOver = false;
		
		// 确保Phase Info Text始终显示
		if (phaseInfoText != null)
		{
			phaseInfoText.gameObject.SetActive(true);
		}
	}

	// Update is called once per frame
	void Update () {
		if (GameIsOver)
			return;

		if (PlayerStats.Lives <= 0)
		{
			EndGame();
		}
	}

	void EndGame ()
	{
		GameIsOver = true;
		gameOverUI.SetActive(true);
		
		// 播放失败音乐
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.PlayDefeatMusic();
		}
	}

	public void WinLevel ()
	{
		GameIsOver = true;
		completeLevelUI.SetActive(true);
		
		// 播放胜利音乐
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.PlayVictoryMusic();
		}
	}

	// 更新阶段信息UI
	public void UpdatePhaseInfo(string info)
	{
		if (phaseInfoText != null)
		{
			phaseInfoText.text = info;
			
			// 确保Text和它的父对象都是激活状态
			if (!phaseInfoText.gameObject.activeInHierarchy)
			{
				phaseInfoText.gameObject.SetActive(true);
			}
		}
	}

}
