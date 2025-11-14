using UnityEngine;

public class BuildManager : MonoBehaviour {

	public static BuildManager instance;

	void Awake ()
	{
		if (instance != null)
		{
			Debug.LogError("More than one BuildManager in scene!");
			return;
		}
		instance = this;
	}

	void Start()
	{
		shop = FindFirstObjectByType<Shop>();
	}

	[Header("特效")]
	public GameObject buildEffect;
	public GameObject sellEffect;

	private TurretBlueprint turretToBuild;
	private Node selectedNode;
	
	private bool isSellMode = false;  // 是否处于拆卸模式
	private Shop shop;  // Shop引用，用于取消按钮选中

	public bool CanBuild { get { return turretToBuild != null; } }
	public bool HasSkillPoints { get { return turretToBuild != null && PlayerStats.HasEnoughSkillPoints(turretToBuild.cost); } }
	public bool IsSellMode { get { return isSellMode; } }

	// 检查当前是否可以修改炮塔（不在游戏进行中）
	public bool CanModifyTurrets 
	{ 
		get 
		{ 
			return GamePhaseManager.Instance == null || GamePhaseManager.Instance.CanModifyTurrets(); 
		} 
	}

	public void SelectNode (Node node)
	{
		// 游戏进行中不能选择节点
		if (!CanModifyTurrets)
			return;

		selectedNode = node;
	}

	public void DeselectNode()
	{
		selectedNode = null;
	}

	// 获取当前选中的节点
	public Node GetSelectedNode()
	{
		return selectedNode;
	}

	public void SelectTurretToBuild (TurretBlueprint turret)
	{
		turretToBuild = turret;
		isSellMode = false;  // 退出拆卸模式
		DeselectNode();  // 清空之前选中的节点
	}
	
	// 清空武器选择（建造完成后调用）
	public void ClearTurretSelection()
	{
		turretToBuild = null;
		DeselectNode();
		
		// 取消Shop按钮的选中状态
		if (shop != null)
		{
			shop.DeselectAll();
		}
	}
	
	// 进入拆卸模式
	public void EnterSellMode()
	{
		isSellMode = true;
		turretToBuild = null;  // 清除建造选择
		DeselectNode();
	}
	
	// 退出拆卸模式
	public void ExitSellMode()
	{
		isSellMode = false;
		DeselectNode();
		
		// 取消Shop按钮的选中状态
		if (shop != null)
		{
			shop.DeselectAll();
		}
	}

	public TurretBlueprint GetTurretToBuild ()
	{
		return turretToBuild;
	}

	// 检查节点上的炮塔是否可以被移除（困难模式检查）
	public bool CanRemoveTurret(Node node)
	{
		if (GamePhaseManager.Instance != null)
		{
			return GamePhaseManager.Instance.CanRemoveTurret(node);
		}
		return true;
	}

}
