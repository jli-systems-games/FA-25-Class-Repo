using UnityEngine;

// 单个敌人生成配置
[System.Serializable]
public class EnemySpawn {
	public GameObject enemy;
	public int count;
}

// 波次配置（可包含多种敌人）
[System.Serializable]
public class Wave {

	public EnemySpawn[] enemies;  // 支持多种敌人混合
	public float rate;  // 生成速率（敌人/秒）

}
