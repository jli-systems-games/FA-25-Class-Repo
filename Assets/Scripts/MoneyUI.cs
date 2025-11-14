using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MoneyUI : MonoBehaviour {

	public Text skillPointsText;

	// Update is called once per frame
	void Update () {
		if (skillPointsText != null)
		{
			skillPointsText.text = "Points: " + PlayerStats.SkillPoints.ToString();
		}
	}
}
