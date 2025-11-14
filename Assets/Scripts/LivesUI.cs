using UnityEngine;
using UnityEngine.UI;

public class LivesUI : MonoBehaviour {

	public Text livesText;

	// Update is called once per frame
	void Update () {
		if (livesText != null)
		{
			livesText.text = "Lives: " + PlayerStats.Lives.ToString();
		}
	}
}
