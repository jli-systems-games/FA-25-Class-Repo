using UnityEngine;
using UnityEngine.UI;

public enum EnemyType
{
	Normal,  // 蓝色 - 普通敌人
	Fast,    // 红色 - 快速敌人
	Heavy    // 黄色 - 重型敌人
}

public class Enemy : MonoBehaviour {

	public EnemyType enemyType = EnemyType.Normal;

	public float startSpeed = 10f;

	[HideInInspector]
	public float speed;

	public float startHealth = 100;
	private float health;

	public int worth = 50;

	public GameObject deathEffect;

	[Header("Unity Stuff")]
	public Image healthBar;

	private bool isDead = false;

	void Start ()
	{
		speed = startSpeed;
		health = startHealth;
	}

	public void TakeDamage (float amount)
	{
		health -= amount;

		healthBar.fillAmount = health / startHealth;

		if (health <= 0 && !isDead)
		{
			Die();
		}
	}

	public void Slow (float pct)
	{
		speed = startSpeed * (1f - pct);
	}

	void Die ()
	{
		isDead = true;

		// 技能点在休息时获得，击杀不给技能点
		// PlayerStats.Money += worth;

		// 生成死亡特效
		if (deathEffect != null)
		{
			// 使用敌人的位置，并确保在合适的高度
			Vector3 effectPos = transform.position;
			effectPos.y += 1.0f; // 抬高1.0单位，进一步远离地面
			
			GameObject effect = (GameObject)Instantiate(deathEffect, effectPos, Quaternion.identity);
			
			// 彻底禁用所有光照和阴影组件
			Light[] lights = effect.GetComponentsInChildren<Light>(true);
			foreach (Light light in lights)
			{
				light.enabled = false;
			}
			
			// 处理粒子系统
			ParticleSystem[] particleSystems = effect.GetComponentsInChildren<ParticleSystem>(true);
			if (particleSystems.Length > 0)
			{
				float maxDuration = 0f;
				foreach (ParticleSystem ps in particleSystems)
				{
					// 确保粒子系统使用World空间，避免跟随销毁的父对象
					var main = ps.main;
					main.simulationSpace = ParticleSystemSimulationSpace.World;
					
					// 停止发射新粒子
					ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
					
					// 计算粒子生命周期
					maxDuration = Mathf.Max(maxDuration, main.duration + main.startLifetime.constantMax);
					
					// 确保粒子渲染器设置正确，彻底禁用光照和阴影
					ParticleSystemRenderer renderer = ps.GetComponent<ParticleSystemRenderer>();
					if (renderer != null)
					{
						// 设置渲染层级
						renderer.sortingOrder = 1000;
						renderer.sortingLayerName = "Default";
						
						// 彻底禁用所有阴影和光照
						renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
						renderer.receiveShadows = false;
						renderer.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
						renderer.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
						
						// 尝试设置材质为Additive混合模式（如果可能）
						if (renderer.material != null)
						{
							// 禁用材质接收阴影
							renderer.material.SetFloat("_ReceiveShadows", 0);
							// 如果材质支持，尝试启用自发光
							if (renderer.material.HasProperty("_EmissionColor"))
							{
								renderer.material.EnableKeyword("_EMISSION");
							}
						}
					}
				}
				
				// 在粒子消失后销毁
				Destroy(effect, Mathf.Min(maxDuration + 0.5f, 2.5f));
			}
			else
			{
				// 非粒子系统
				Renderer[] renderers = effect.GetComponentsInChildren<Renderer>(true);
				foreach (Renderer rend in renderers)
				{
					rend.sortingOrder = 1000;
					// 彻底禁用所有阴影和光照
					rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
					rend.receiveShadows = false;
					rend.lightProbeUsage = UnityEngine.Rendering.LightProbeUsage.Off;
					rend.reflectionProbeUsage = UnityEngine.Rendering.ReflectionProbeUsage.Off;
				}
				
				Destroy(effect, 2f);
			}
		}

		WaveSpawner.EnemiesAlive--;

		Destroy(gameObject);
	}

}
