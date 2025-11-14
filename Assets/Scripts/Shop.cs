using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour {

	[Header("武器蓝图配置")]
	public TurretBlueprint standardTurret;   // 标准炮塔
	public TurretBlueprint rapidTurret;      // 快速炮塔（仅攻击红色）
	public TurretBlueprint heavyTurret;      // 重型炮塔

	[Header("武器按钮引用")]
	public Button standardButton;
	public Button rapidButton;
	public Button heavyButton;
	public Button sellButton;

	BuildManager buildManager;
	private Button currentSelectedButton = null;  // 当前选中的按钮
	
	// 保存每个按钮的原始颜色
	private ColorBlock standardNormalColors;
	private ColorBlock rapidNormalColors;
	private ColorBlock heavyNormalColors;
	private ColorBlock sellNormalColors;
	
	private ColorBlock deleteSelectedColors;  // 拆卸选中颜色（红色）
	
	private bool isInitialized = false;  // 标记是否已初始化

	void Start ()
	{
		buildManager = BuildManager.instance;
		
		// 保存每个按钮的原始颜色
		if (standardButton != null)
		{
			standardNormalColors = standardButton.colors;
		}
		if (rapidButton != null)
		{
			rapidNormalColors = rapidButton.colors;
		}
		if (heavyButton != null)
		{
			heavyNormalColors = heavyButton.colors;
		}
		if (sellButton != null)
		{
			sellNormalColors = sellButton.colors;
		}
		
		// 创建拆卸选中状态的颜色（红色）
		if (sellButton != null)
		{
			deleteSelectedColors = sellNormalColors;
			deleteSelectedColors.normalColor = new Color(1f, 0.3f, 0.3f);  // 红色
			deleteSelectedColors.highlightedColor = new Color(1f, 0.4f, 0.4f);
			deleteSelectedColors.pressedColor = new Color(0.8f, 0.2f, 0.2f);
		}
		
		// 标记已初始化
		isInitialized = true;
		
		// 确保初始状态正确
		DeselectAll();
	}

	// 选择标准炮塔（等待点击格子放置）
	public void SelectStandardTurret ()
	{
		if (currentSelectedButton == standardButton)
		{
			// 再次点击同一按钮，取消选择
			DeselectAll();
			buildManager.ClearTurretSelection();
		}
		else
		{
			buildManager.SelectTurretToBuild(standardTurret);
			SetSelectedButton(standardButton);
		}
	}

	// 选择快速炮塔（等待点击格子放置）
	public void SelectRapidTurret()
	{
		if (currentSelectedButton == rapidButton)
		{
			// 再次点击同一按钮，取消选择
			DeselectAll();
			buildManager.ClearTurretSelection();
		}
		else
		{
			buildManager.SelectTurretToBuild(rapidTurret);
			SetSelectedButton(rapidButton);
		}
	}

	// 选择重型炮塔（等待点击格子放置）
	public void SelectHeavyTurret()
	{
		if (currentSelectedButton == heavyButton)
		{
			// 再次点击同一按钮，取消选择
			DeselectAll();
			buildManager.ClearTurretSelection();
		}
		else
		{
			buildManager.SelectTurretToBuild(heavyTurret);
			SetSelectedButton(heavyButton);
		}
	}

	// 进入拆卸模式（拆卸按钮）
	public void SelectSellMode()
	{
		if (currentSelectedButton == sellButton)
		{
			// 再次点击拆卸按钮，取消拆卸模式
			DeselectAll();
			buildManager.ExitSellMode();
		}
		else
		{
			buildManager.EnterSellMode();
			SetSelectedButton(sellButton, true);  // 使用红色
		}
	}

	// 设置选中的按钮
	void SetSelectedButton(Button button, bool isDeleteMode = false)
	{
		// 清除之前的选中状态
		DeselectAll();
		
		// 设置新的选中状态
		currentSelectedButton = button;
		if (button != null)
		{
			if (isDeleteMode)
			{
				button.colors = deleteSelectedColors;  // 拆卸模式：红色
			}
			else
			{
				// 根据按钮类型，使用对应的原始颜色来创建变暗效果
				ColorBlock originalColors = standardNormalColors;
				
				if (button == standardButton)
					originalColors = standardNormalColors;
				else if (button == rapidButton)
					originalColors = rapidNormalColors;
				else if (button == heavyButton)
					originalColors = heavyNormalColors;
				
				// 创建基于该按钮自己颜色的变暗版本
				ColorBlock darkColors = originalColors;
				darkColors.normalColor = originalColors.normalColor * 0.6f;
				darkColors.highlightedColor = originalColors.normalColor * 0.6f;
				darkColors.pressedColor = originalColors.pressedColor * 0.6f;
				
				button.colors = darkColors;  // 武器选择：变暗
			}
		}
	}

	// 取消所有按钮的选中状态
	public void DeselectAll()
	{
		// 如果还未初始化，直接返回（避免使用未初始化的颜色值）
		if (!isInitialized)
		{
			return;
		}
		
		// 恢复每个按钮自己的原始颜色
		if (standardButton != null) standardButton.colors = standardNormalColors;
		if (rapidButton != null) rapidButton.colors = rapidNormalColors;
		if (heavyButton != null) heavyButton.colors = heavyNormalColors;
		if (sellButton != null) sellButton.colors = sellNormalColors;
		
		currentSelectedButton = null;
	}

	// 获取武器蓝图（给NodeUI用于显示信息）
	public TurretBlueprint GetTurretBlueprint(int index)
	{
		switch (index)
		{
			case 0:
				return standardTurret;
			case 1:
				return rapidTurret;
			case 2:
				return heavyTurret;
			default:
				return null;
		}
	}

}
