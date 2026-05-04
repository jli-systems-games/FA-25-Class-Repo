# Unity Player Controller & Test Scene Generator

## 快速开始 🚀

### 1. 导入脚本
将以下三个脚本放入你的Unity项目：
- `PlayerController.cs` - 放在 `Assets/Scripts/` 文件夹
- `Interactable.cs` - 放在 `Assets/Scripts/` 文件夹  
- `TestSceneGenerator.cs` - 放在 `Assets/Editor/` 文件夹（**必须**是Editor文件夹）

### 2. 一键生成测试场景
1. 在Unity顶部菜单栏，点击 `Tools` → `Generate Test Scene`
2. 在弹出的窗口中调整参数：
   - Floor Size: 地面大小
   - Interactable Objects: 可交互物体数量
   - Collectables: 可收集物品数量
   - Add Obstacles: 是否添加障碍物
   - Ground Color: 地面颜色
3. 点击 `Generate Scene` 按钮
4. 完成！按 Play 开始测试

### 3. 手动设置（如果需要）
如果你想手动创建玩家：

**必需组件：**
- CharacterController
- PlayerController script

**可选设置：**
- 在PlayerController的Inspector中调整参数
- 设置Ground Layer Mask（用于地面检测）
- 设置Interactable Layer Mask（用于交互检测）

## 控制方式 🎮

| 操作 | 按键 |
|------|------|
| 移动 | WASD / 方向键 |
| 跳跃 | 空格 |
| 冲刺 | Shift |
| 交互 | E |
| 视角 | 鼠标移动 |

## 核心功能说明 📚

### PlayerController
- **平滑移动**：使用加速度系统，移动更自然
- **相机相对移动**：根据相机朝向决定移动方向
- **跳跃系统**：物理感觉良好的跳跃
- **交互系统**：基于射线检测的交互，支持悬停反馈

### 交互系统
提供了两个示例：

**InteractableObject**（基础交互物体）
- 鼠标悬停时高亮显示
- 按E键触发交互
- 可自定义交互逻辑

**CollectableItem**（可收集物品）
- 自动旋转和浮动动画
- 交互后自动销毁
- 可扩展添加到库存系统

### 创建自定义交互物体
```csharp
public class MyCustomInteractable : MonoBehaviour, IInteractable
{
    public void Interact(GameObject player)
    {
        // 你的交互逻辑
        Debug.Log("自定义交互!");
    }
    
    public void OnHoverEnter()
    {
        // 悬停进入时的反馈
    }
    
    public void OnHoverExit()
    {
        // 悬停离开时的反馈
    }
}
```

## 进阶定制 🔧

### 调整移动手感
在PlayerController的Inspector中：
- `Walk Speed`: 行走速度（建议: 3-6）
- `Sprint Speed`: 冲刺速度（建议: 6-10）
- `Acceleration`: 加速度（数值越大，启动越快）
- `Deceleration`: 减速度（数值越大，停止越快）

### 调整跳跃手感
- `Jump Force`: 跳跃力度
- `Gravity`: 重力强度（负数，绝对值越大下落越快）
- `Ground Check Distance`: 地面检测距离

### 交互范围
- `Interaction Range`: 可交互距离（建议: 2-4米）

## 常见问题 ❓

**Q: 玩家穿模/卡在地面？**
A: 检查CharacterController的参数，特别是Radius和Height。确保Ground Check Distance设置合理。

**Q: 交互无反应？**
A: 确保：
1. 物体有Collider组件
2. 物体实现了IInteractable接口
3. Interactable Layer Mask设置正确
4. 物体在交互范围内

**Q: 移动方向不对？**
A: 确保Camera Transform已正确赋值到PlayerController。场景生成器会自动设置，但手动创建需要注意。

**Q: 跳跃太高/太低？**
A: 调整Jump Force和Gravity参数。这两个参数是联动的：
- Jump Force越大，跳得越高
- Gravity绝对值越大，下落越快

## 与Sailvage集成建议 🎯

考虑到Sailvage是roguelike游戏，你可能需要：

1. **添加冲刺能量系统**
   ```csharp
   private float sprintEnergy = 100f;
   private float sprintCost = 20f; // 每秒消耗
   ```

2. **添加受击反馈**
   ```csharp
   public void TakeDamage(float damage)
   {
       // 添加击退效果
       // 添加无敌帧
       // 触发动画
   }
   ```

3. **扩展交互系统**
   - 商店购买
   - 武器切换
   - 技能释放
   - NPC对话

4. **添加状态机**
   如果需要更复杂的状态控制（攻击、受伤、死亡等），建议实现一个状态机系统。

## 性能优化建议 ⚡

- 交互检测的Raycast可以不用每帧执行，可以改成每0.1秒检测一次
- 如果场景中可交互物体很多，考虑使用对象池
- Gizmos绘制只在Editor模式有效，不影响Build性能

## 下一步 🎨

1. 添加动画系统（Animator）
2. 实现战斗系统
3. 添加UI提示（交互提示、体力条等）
4. 集成你的Sailvage核心玩法
5. 添加音效反馈

---

**Have fun building Sailvage! 🌊**

如果有问题随时找我～这套系统已经考虑了roguelike的特性，很容易扩展！
