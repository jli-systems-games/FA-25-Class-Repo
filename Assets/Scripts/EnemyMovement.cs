using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Enemy))]
public class EnemyMovement : MonoBehaviour {

	private Transform target;
	private int wavepointIndex = 0;

	private Enemy enemy;

	void Start()
	{
		enemy = GetComponent<Enemy>();

		// 安全检查：确保Waypoints已配置
		if (Waypoints.points == null || Waypoints.points.Length == 0)
		{
			Debug.LogError("Waypoints未配置！请在场景中创建Waypoints GameObject并添加路径点。");
			Destroy(gameObject);
			return;
		}

		target = Waypoints.points[0];
	}

	void Update()
	{
		Vector3 dir = target.position - transform.position;
		transform.Translate(dir.normalized * enemy.speed * Time.deltaTime, Space.World);

		if (Vector3.Distance(transform.position, target.position) <= 0.4f)
		{
			GetNextWaypoint();
		}

		enemy.speed = enemy.startSpeed;
	}

	void GetNextWaypoint()
	{
		if (wavepointIndex >= Waypoints.points.Length - 1)
		{
			// 到达最后一个路径点
			EndPath();
			return;
		}

		wavepointIndex++;
		target = Waypoints.points[wavepointIndex];
	}

	void EndPath()
	{
		// 敌人到达终点，减少生命值
		PlayerStats.Lives--;
		WaveSpawner.EnemiesAlive--;
		
		Debug.Log($"敌人到达终点！剩余生命值: {PlayerStats.Lives}");
		
		Destroy(gameObject);
	}

}
