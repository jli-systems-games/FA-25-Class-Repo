using UnityEngine;
using UnityEngine.EventSystems;

public class Node : MonoBehaviour {

	public Color hoverColor;
	public Color notEnoughMoneyColor;
    public Vector3 positionOffset;

	[HideInInspector]
	public GameObject turret;
	[HideInInspector]
	public TurretBlueprint turretBlueprint;
	[HideInInspector]
	public bool isUpgraded = false;

	private Renderer rend;
	private Color startColor;

	BuildManager buildManager;

	void Start ()
	{
		rend = GetComponent<Renderer>();
		startColor = rend.material.color;

		buildManager = BuildManager.instance;
    }

	public Vector3 GetBuildPosition ()
	{
		return transform.position + positionOffset;
	}

	void OnMouseDown ()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		// 游戏进行中不能操作
		if (!buildManager.CanModifyTurrets)
			return;

		// 拆卸模式
		if (buildManager.IsSellMode)
		{
			if (turret != null)
			{
				SellTurret();
				buildManager.ExitSellMode();  // 拆卸后退出拆卸模式
			}
			return;
		}

		// 建造模式
		if (turret != null)
		{
			return;
		}

		// 空节点，检查是否已选择武器
		if (!buildManager.CanBuild)
		{
			return;
		}

		// 直接建造
		TurretBlueprint blueprint = buildManager.GetTurretToBuild();
		BuildTurret(blueprint);
		buildManager.ClearTurretSelection();  // 建造后清空选择，需要重新选择武器
	}

	void BuildTurret (TurretBlueprint blueprint)
	{
		if (!PlayerStats.HasEnoughSkillPoints(blueprint.cost))
		{
			return;
		}

		PlayerStats.SpendSkillPoints(blueprint.cost);

		GameObject _turret = (GameObject)Instantiate(blueprint.prefab, GetBuildPosition(), Quaternion.identity);
		turret = _turret;

		turretBlueprint = blueprint;

		// 设置炮塔的目标类型
		Turret turretScript = _turret.GetComponent<Turret>();
		if (turretScript != null)
		{
			turretScript.targetType = blueprint.targetType;
		}

		GameObject effect = (GameObject)Instantiate(buildManager.buildEffect, GetBuildPosition(), Quaternion.identity);
		Destroy(effect, 5f);
	}

	// 公共方法供NodeUI调用
	public void TriggerBuild()
	{
		if (!buildManager.CanBuild)
			return;

		BuildTurret(buildManager.GetTurretToBuild());
	}

	// 升级功能已删除
	// 所有武器都是1个技能点，不需要升级

	public void SellTurret ()
	{
		// 检查是否可以删除（困难模式检查）
		if (!buildManager.CanRemoveTurret(this))
		{
			return;
		}

		// 返还技能点
		PlayerStats.AddSkillPoints(turretBlueprint.cost);

		GameObject effect = (GameObject)Instantiate(buildManager.sellEffect, GetBuildPosition(), Quaternion.identity);
		Destroy(effect, 5f);

		Destroy(turret);
		turret = null;
		turretBlueprint = null;
		isUpgraded = false;
	}

	void OnMouseEnter ()
	{
		if (EventSystem.current.IsPointerOverGameObject())
			return;

		// 游戏进行中不能操作
		if (!buildManager.CanModifyTurrets)
			return;

		// 拆卸模式下，鼠标悬停在有炮塔的节点上显示高亮
		if (buildManager.IsSellMode)
		{
			if (turret != null)
			{
				rend.material.color = hoverColor;
			}
			return;
		}

		// 建造模式下，只有空节点才显示预览
		if (turret != null)
			return;

		if (!buildManager.CanBuild)
			return;

		// 悬停显示红色预览
		if (buildManager.HasSkillPoints)
		{
			rend.material.color = hoverColor;
		} else
		{
			rend.material.color = notEnoughMoneyColor;
		}
	}

	void OnMouseExit ()
	{
		rend.material.color = startColor;
    }

}
