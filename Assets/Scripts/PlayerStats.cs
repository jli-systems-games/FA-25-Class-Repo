using UnityEngine;
using System.Collections;

public class PlayerStats : MonoBehaviour {

	// 技能点系统（替代货币系统）
	public static int SkillPoints;
	public int startSkillPoints = 3;

	public static int Lives;
	public int startLives = 7;  // 改为7条命

	public static int Rounds;

	void Start ()
	{
		SkillPoints = startSkillPoints;
		Lives = startLives;
		Rounds = 0;
	}

	// 添加技能点
	public static void AddSkillPoints(int amount)
	{
		SkillPoints += amount;
	}

	// 使用技能点
	public static bool SpendSkillPoints(int amount)
	{
		if (SkillPoints >= amount)
		{
			SkillPoints -= amount;
			return true;
		}
		return false;
	}

	// 检查是否有足够技能点
	public static bool HasEnoughSkillPoints(int amount)
	{
		return SkillPoints >= amount;
	}

}
