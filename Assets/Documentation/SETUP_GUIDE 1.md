# 拓麻歌子风格 Unity 游戏 - 设置指南

## 快速开始

### 1. 场景设置

1. 打开 Unity 编辑器
2. 打开场景：`Assets/Scenes/TamagotchiHome.unity`
3. 在场景中创建一个空的 GameObject，命名为 "GameInitializer"
4. 添加 `GameInitializer` 脚本到这个 GameObject

### 2. 运行游戏

点击 Unity 编辑器的 Play 按钮，游戏会自动：
- 创建所有必要的管理器（GameManager, CatManager, InputManager, UIManager）
- 构建完整的UI界面
- 初始化小猫的属性

## 操作说明

### 基本操作
- **↑/↓ 键**：在菜单中上下选择（喂食、玩耍、清洁）
- **←/→ 键**：在子菜单中左右选择不同选项
- **空格键**：确认选择
- **ESC 键**：暂停/继续游戏

### 游戏机制

#### 小猫属性
- **饱食度（Hunger）**：0-100，每秒减少 0.5
- **心情（Happiness）**：0-100，每秒减少 0.3
- **清洁度（Hygiene）**：0-100，每秒减少 0.2

#### 菜单选项

**喂食（Feed）**
- 普通猫粮（$5）：+20 饱食度，+5 心情
- 高级猫粮（$15）：+35 饱食度，+15 心情
- 零食（$10）：+10 饱食度，+20 心情

**玩耍（Play）**
- 逗猫棒：+25 心情（消耗 5 饱食度）
- 毛线球：+20 心情（消耗 5 饱食度）

**清洁（Clean）**
- 普通洗澡（$5）：+30 清洁度，+5 心情
- 高级香波（$12）：+50 清洁度，+15 心情

#### 失败条件
当任意属性低于 10 点并持续 30 秒时，小猫会"仙女化"飞走，游戏结束。

## 自定义设置

### 调整属性衰减速率

在游戏运行时，选择 Hierarchy 中的 "CatManager"，在 Inspector 中可以调整：
- `Hunger Decay Rate`：饱食度衰减速率
- `Happiness Decay Rate`：心情衰减速率
- `Hygiene Decay Rate`：清洁度衰减速率
- `Critical Threshold`：危险阈值
- `Critical Duration`：危险状态持续多久会死亡

### 添加小猫精灵图像

1. 在 Hierarchy 中找到自动创建的 "CatDisplay" GameObject
2. 选择它的 `CatAnimationController` 组件
3. 在 Inspector 中为以下状态分配精灵：
   - Idle Sprite（闲置）
   - Sleeping Sprite（睡觉）
   - Eating Sprite（吃饭）
   - Playing Sprite（玩耍）
   - Bathing Sprite（洗澡）
   - Happy Sprite（开心）
   - Sad Sprite（伤心）
   - Sick Sprite（生病）
   - Dying Sprite（濒死）
   - Angel Sprites（仙女化动画序列）

### 添加菜单图标

1. 在 Hierarchy 中找到自动创建的菜单图标（Icon_Feed, Icon_Play, Icon_Clean）
2. 为每个图标的 Image 组件分配对应的精灵图像

### 使用像素字体

1. 在 Hierarchy 中找到 "UIBuilder" GameObject
2. 在 Inspector 中的 `Pixel Font` 字段中分配 TextMesh Pro 字体资源
3. 项目中已包含 `LowGothic_8x10_Regular SDF` 字体，可以使用

## 项目结构

```
Assets/
├── Scenes/
│   └── TamagotchiHome.unity      # 主场景
├── Scripts/
│   ├── GameEnums.cs               # 游戏枚举定义
│   ├── CatData.cs                 # 小猫数据类
│   ├── CatManager.cs              # 小猫管理器
│   ├── GameManager.cs             # 游戏管理器
│   ├── InputManager.cs            # 输入管理器
│   ├── UIManager.cs               # UI管理器
│   ├── UIBuilder.cs               # UI构建器
│   ├── CatAnimationController.cs # 小猫动画控制器
│   ├── CatAnimationBridge.cs     # 动画桥接脚本
│   └── GameInitializer.cs        # 游戏初始化脚本
└── Resources/
    └── Sprites/                   # 精灵资源
```

## 常见问题

### Q: 游戏运行时没有显示UI？
A: 确保场景中有 GameInitializer 脚本，它会自动创建所有必要的UI元素。

### Q: 小猫没有显示图像？
A: 需要手动为 CatAnimationController 分配精灵图像。在游戏运行时，找到 CatDisplay GameObject 并分配精灵。

### Q: 如何调整游戏难度？
A: 在 CatManager 的 Inspector 中调整属性衰减速率。降低衰减速率会使游戏更简单，提高则更困难。

### Q: 如何修改初始金钱？
A: 在 CatData.cs 中修改 `money` 的初始值，或在 CatManager 的 Inspector 中调整。

## 扩展功能建议

1. **添加更多动画帧**：为每个状态创建多帧动画
2. **添加音效**：为每个动作添加音效
3. **添加背景音乐**：添加循环播放的背景音乐
4. **添加小游戏**：创建可以赚钱的小游戏
5. **添加成长系统**：小猫在不同年龄阶段有不同外观
6. **添加装饰系统**：允许玩家购买装饰品

## 技术支持

如有问题，请检查：
1. Unity 版本是否为 2020.3 或更高
2. TextMesh Pro 包是否已导入
3. 所有脚本是否都在 Assets/Scripts 文件夹中
4. 场景中是否有 GameInitializer

---

**设计者：** Manus  
**实现日期：** 2025年10月22日

