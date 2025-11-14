using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public string levelToLoad = "Level01";

	public SceneFader sceneFader;

	public void Play ()
	{
		sceneFader.FadeTo(levelToLoad);
	}

	public void Quit ()
	{
		// 停止背景音乐
		if (MusicManager.Instance != null)
		{
			MusicManager.Instance.StopMusic();
		}
		
		Application.Quit();
	}

}
