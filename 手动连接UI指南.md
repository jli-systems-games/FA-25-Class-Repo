# 手动连接UI指南

既然你已经制作了自己的Canvas，现在只需要把UI元素连接到游戏系统。

## 🎯 必须连接的组件

### 1. UIManager（UI管理器）

在场景中找到或创建一个GameObject，添加 `UIManager` 脚本，然后连接以下UI元素：

#### 必须的引用：
- **Menu Icons（菜单图标数组，3个）** - 可选，如果你只用黑点指示器可以不连接
  - 0: 喂食图标
  - 1: 玩耍图标  
  - 2: 清洁图标

- **Selection Indicators（选中指示器数组，3个）** - 黑点GameObject
  - 0: 喂食选项的黑点
  - 1: 玩耍选项的黑点
  - 2: 清洁选项的黑点
  - ⚠️ 这些GameObject在场景中应该初始都是隐藏的（取消勾选）

- **Stats Bars（状态条数组，3个）**
  - 0: 饱食度条（Image组件，Type设为Filled）
  - 1: 心情条（Image组件，Type设为Filled）
  - 2: 清洁度条（Image组件，Type设为Filled）

- **Money Text**：显示金钱的TextMeshPro文本

#### 可选的引用：
- **Sub Menu Panel**：子菜单面板（GameObject）
- **Sub Menu Text**：子菜单文本（TextMeshPro）
- **Game Over Panel**：游戏结束面板（GameObject）
- **Game Over Text**：游戏结束文本（TextMeshPro）

### 2. CatManager（小猫管理器）

在场景中找到或创建一个GameObject，添加 `CatManager` 脚本。

不需要手动连接任何UI，但你可以在Inspector中调整：
- Hunger Decay Rate：饱食度衰减速率（默认0.5）
- Happiness Decay Rate：心情衰减速率（默认0.3）
- Hygiene Decay Rate：清洁度衰减速率（默认0.2）
- Critical Threshold：危险阈值（默认10）
- Critical Duration：危险持续时间（默认30秒）

### 3. 小猫显示（可选）

如果你想显示小猫动画：

1. 在Canvas中创建一个Image GameObject
2. 添加 `CatAnimationController` 脚本
3. 添加 `CatAnimationBridge` 脚本
4. 在CatAnimationController中设置：
   - Cat Image：这个Image组件本身
   - 各种状态的Sprite（Idle, Eating, Playing等）

## 📋 快速设置步骤

### 步骤1：创建GameInitializer
1. 在场景中创建空GameObject，命名为"GameInitializer"
2. 添加 `GameInitializer` 脚本
3. 点击Play，系统会自动创建核心管理器

### 步骤2：手动添加UIManager到场景
1. 在Hierarchy中创建空GameObject，命名为"UIManager"
2. 添加 `UIManager` 脚本
3. 把你的UI元素拖到UIManager的对应字段

### 步骤3：测试
点击Play按钮，检查：
- ✅ 按↑↓键，黑点应该在菜单项之间移动
- ✅ 状态条应该随时间减少
- ✅ 金钱应该更新
- ✅ 按空格键应该打开子菜单

## 🎨 推荐的UI结构

```
Canvas
├── MainPanel（主面板）
│   ├── MenuArea（左侧菜单区）
│   │   ├── MenuItem1（喂食项）
│   │   │   ├── SelectionDot（黑点，默认隐藏）→ 拖到 UIManager.selectionIndicators[0]
│   │   │   ├── FeedIcon（喂食图标）
│   │   │   └── FeedBar（饱食度条）→ 拖到 UIManager.statsBars[0]
│   │   ├── MenuItem2（玩耍项）
│   │   │   ├── SelectionDot（黑点，默认隐藏）→ 拖到 UIManager.selectionIndicators[1]
│   │   │   ├── PlayIcon（玩耍图标）
│   │   │   └── PlayBar（心情条）→ 拖到 UIManager.statsBars[1]
│   │   ├── MenuItem3（清洁项）
│   │   │   ├── SelectionDot（黑点，默认隐藏）→ 拖到 UIManager.selectionIndicators[2]
│   │   │   ├── CleanIcon（清洁图标）
│   │   │   └── CleanBar（清洁度条）→ 拖到 UIManager.statsBars[2]
│   │
│   ├── CatArea（右侧小猫区）
│   │   └── CatImage（小猫图片）+ CatAnimationController
│   │
│   ├── TopBar（顶部信息栏）
│   │   └── MoneyText → 拖到 UIManager.moneyText
│   │
│   ├── SubMenuPanel → 拖到 UIManager.subMenuPanel
│   │   └── SubMenuText → 拖到 UIManager.subMenuText
│   │
│   └── GameOverPanel → 拖到 UIManager.gameOverPanel
│       └── GameOverText → 拖到 UIManager.gameOverText
```

## 💡 提示

### 黑点指示器设置 ⭐ 重要！
1. 在每个菜单项的左边创建一个黑点GameObject（可以是Image、Sprite或任何可见对象）
2. **所有黑点初始必须隐藏**（取消勾选GameObject）
3. 游戏运行时会自动显示当前选中项的黑点

### 状态条设置
状态条必须使用 **Image** 组件，并设置：
- Image Type: **Filled**
- Fill Method: **Horizontal**
- Fill Origin: **Left**
- Fill Amount: **1**（初始值）

### 状态条颜色变化（可选）
如果你想要状态条根据数值变色（绿→黄→红）：
1. 在UIManager的Inspector中勾选 `Use Status Bar Colors`
2. 不勾选则保持你自己设置的颜色

### 子菜单和游戏结束面板
这两个面板初始应该是 **隐藏** 的（取消勾选GameObject）

### 如果不需要某些功能
如果你不需要子菜单或游戏结束面板，可以不连接，但游戏会在Console显示警告。

## ❓ 常见问题

**Q: 按键没反应？**
A: 确保InputManager已创建（GameInitializer会自动创建）

**Q: 状态条不动？**
A: 确保状态条的Image Type设为Filled

**Q: 菜单选择没有黑点显示？**
A: 确保selectionIndicators数组正确连接了3个GameObject，并且它们初始是隐藏状态

**Q: 金钱不更新？**
A: 确保moneyText连接了TextMeshPro组件

## 🎮 完成后

所有连接完成后，点击Play，你的自定义UI就能正常工作了！

游戏会自动：
- ✅ 更新状态条
- ✅ 显示选中的菜单（黑点）
- ✅ 显示金钱
- ✅ 响应键盘输入
- ✅ 处理游戏逻辑

享受你的游戏吧！🎉

