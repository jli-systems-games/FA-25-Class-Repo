using UnityEngine;
using UnityEngine.UI;

public class NodeUI : MonoBehaviour {

	public GameObject buildMenuUI;  // 建造菜单（显示3种武器）
	public GameObject sellMenuUI;   // 拆除菜单（显示拆除选项）

	[Header("武器选择按钮")]
	public Button standardTurretButton;
	public Button rapidTurretButton;
	public Button heavyTurretButton;

	[Header("武器信息显示")]
	public GameObject weaponInfoPanel;
	public Text weaponNameText;
	public Text weaponStatsText;
	public Text weaponDescriptionText;

	[Header("拆除信息")]
	public Text sellInfoText;
	public Button sellButton;

	private Node target;
	private Shop shop;

	void Start()
	{
		shop = FindFirstObjectByType<Shop>();
		
		// 为武器按钮添加悬停事件
		AddHoverListeners();
	}

	void AddHoverListeners()
	{
		if (standardTurretButton != null)
		{
			AddHoverListener(standardTurretButton, 0);
		}
		if (rapidTurretButton != null)
		{
			AddHoverListener(rapidTurretButton, 1);
		}
		if (heavyTurretButton != null)
		{
			AddHoverListener(heavyTurretButton, 2);
		}
	}

	void AddHoverListener(Button button, int turretIndex)
	{
		UnityEngine.EventSystems.EventTrigger trigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
		
		// 鼠标进入
		var entryEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
		entryEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
		entryEnter.callback.AddListener((data) => { ShowWeaponInfo(turretIndex); });
		trigger.triggers.Add(entryEnter);
		
		// 鼠标离开
		var entryExit = new UnityEngine.EventSystems.EventTrigger.Entry();
		entryExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
		entryExit.callback.AddListener((data) => { HideWeaponInfo(); });
		trigger.triggers.Add(entryExit);
	}

	public void SetTarget (Node _target)
	{
		target = _target;
		transform.position = target.GetBuildPosition();

		if (target.turret == null)
		{
			// 空节点，显示建造菜单
			ShowBuildMenu();
		}
		else
		{
			// 已有炮塔，显示拆除菜单
			ShowSellMenu();
		}
	}

	void ShowBuildMenu()
	{
		if (buildMenuUI != null)
			buildMenuUI.SetActive(true);
		if (sellMenuUI != null)
			sellMenuUI.SetActive(false);
	}

	void ShowSellMenu()
	{
		if (buildMenuUI != null)
			buildMenuUI.SetActive(false);
		if (sellMenuUI != null)
			sellMenuUI.SetActive(true);

		// 更新拆除信息
		if (sellInfoText != null && target.turretBlueprint != null)
		{
			sellInfoText.text = $"拆除返还: {target.turretBlueprint.cost} 技能点";
		}

		// 检查是否可以拆除（困难模式）
		if (sellButton != null)
		{
			bool canRemove = BuildManager.instance.CanRemoveTurret(target);
			sellButton.interactable = canRemove;
			
			if (!canRemove && sellInfoText != null)
			{
				sellInfoText.text = "困难模式：之前部署的武器无法删除";
			}
		}
	}

	void ShowWeaponInfo(int turretIndex)
	{
		if (weaponInfoPanel == null || shop == null) return;

		TurretBlueprint blueprint = shop.GetTurretBlueprint(turretIndex);
		if (blueprint == null) return;

		weaponInfoPanel.SetActive(true);

		if (weaponNameText != null)
			weaponNameText.text = blueprint.weaponName;

		if (weaponStatsText != null)
		{
			weaponStatsText.text = $"成本: {blueprint.cost} 技能点\n" +
			                       $"伤害: {blueprint.damage}\n" +
			                       $"射速: {blueprint.fireRate:F1}秒\n" +
			                       $"射程: {blueprint.range}";
		}

		if (weaponDescriptionText != null)
			weaponDescriptionText.text = blueprint.description;
	}

	void HideWeaponInfo()
	{
		if (weaponInfoPanel != null)
			weaponInfoPanel.SetActive(false);
	}

	public void Hide ()
	{
		if (buildMenuUI != null)
			buildMenuUI.SetActive(false);
		if (sellMenuUI != null)
			sellMenuUI.SetActive(false);
		if (weaponInfoPanel != null)
			weaponInfoPanel.SetActive(false);
	}

	// 选择标准炮塔
	public void SelectStandardTurret()
	{
		if (shop != null)
		{
			shop.SelectStandardTurret();
			target.TriggerBuild();  // 触发建造
		}
		BuildManager.instance.DeselectNode();
	}

	// 选择快速炮塔
	public void SelectRapidTurret()
	{
		if (shop != null)
		{
			shop.SelectRapidTurret();
			target.TriggerBuild();
		}
		BuildManager.instance.DeselectNode();
	}

	// 选择重型炮塔
	public void SelectHeavyTurret()
	{
		if (shop != null)
		{
			shop.SelectHeavyTurret();
			target.TriggerBuild();
		}
		BuildManager.instance.DeselectNode();
	}

	// 拆除炮塔
	public void Sell ()
	{
		target.SellTurret();
		BuildManager.instance.DeselectNode();
	}

}
