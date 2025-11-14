using UnityEngine;
using System.Collections;

[System.Serializable]
public class TurretBlueprint {

	public GameObject prefab;
	public int cost = 1;  // 所有武器都是1个技能点

	// 武器信息（UI显示用）
	public string weaponName;
	public string description;
	public int damage;
	public float fireRate;
	public float range;
	
	// 目标类型（0=全部，1=仅红色）
	public int targetType = 0;

	// 升级功能已删除
	// public GameObject upgradedPrefab;
	// public int upgradeCost;

	public int GetSellAmount ()
	{
		return cost;  // 返还全部技能点
	}

}
