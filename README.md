# 拓麻歌子风格 Unity 游戏

一款受经典拓麻歌子游戏启发的 Unity 虚拟宠物游戏，玩家需要照顾一只虚拟小猫。

![游戏类型](https://img.shields.io/badge/类型-虚拟宠物-brightgreen)
![Unity版本](https://img.shields.io/badge/Unity-2020.3+-blue)
![开发状态](https://img.shields.io/badge/状态-完成-success)

## 🎮 游戏特色

- **简洁操作**：仅使用方向键和空格键进行所有操作
- **像素风格**：复古的像素艺术风格界面
- **属性管理**：管理小猫的饱食度、心情和清洁度
- **多种互动**：喂食、玩耍、清洁三大互动系统
- **失败机制**：照顾不周会导致小猫"仙女化"飞走

## 🚀 快速开始

### 前置要求

- Unity 2020.3 或更高版本
- TextMesh Pro 包（通常已内置）

### 安装步骤

1. 克隆或下载此项目
2. 使用 Unity Hub 打开项目
3. 打开场景：`Assets/Scenes/TamagotchiHome.unity`
4. 在场景中创建空的 GameObject，命名为 "GameInitializer"
5. 添加 `GameInitializer` 脚本到该 GameObject
6. 点击 Play 按钮运行游戏

就这么简单！游戏会自动创建所有必要的管理器和UI。

## 🎯 游戏玩法

### 操作控制

| 按键 | 功能 |
|------|------|
| ↑/↓ | 在主菜单中上下选择 |
| ←/→ | 在子菜单中左右选择选项 |
| 空格 | 确认选择 |
| ESC | 暂停/继续游戏 |
| H | 显示/隐藏控制提示 |

### 小猫属性

| 属性 | 范围 | 衰减速率 | 说明 |
|------|------|----------|------|
| 饱食度 | 0-100 | 0.5/秒 | 需要定期喂食 |
| 心情 | 0-100 | 0.3/秒 | 通过玩耍和照顾提升 |
| 清洁度 | 0-100 | 0.2/秒 | 需要定期清洁 |

### 互动选项

#### 🍖 喂食（Feed）
- **普通猫粮** ($5)：+20 饱食度，+5 心情
- **高级猫粮** ($15)：+35 饱食度，+15 心情
- **零食** ($10)：+10 饱食度，+20 心情

#### 🎾 玩耍（Play）
- **逗猫棒**：+25 心情（消耗 5 饱食度）
- **毛线球**：+20 心情（消耗 5 饱食度）

#### 🛁 清洁（Clean）
- **普通洗澡** ($5)：+30 清洁度，+5 心情
- **高级香波** ($12)：+50 清洁度，+15 心情

### 失败条件

⚠️ 当任意属性低于 10 点并持续 30 秒时，小猫会"仙女化"飞走，游戏结束。

## 📁 项目结构

```
Assets/
├── Scenes/
│   └── TamagotchiHome.unity          # 主游戏场景
├── Scripts/
│   ├── GameEnums.cs                  # 游戏枚举定义
│   ├── CatData.cs                    # 小猫数据类
│   ├── CatManager.cs                 # 小猫管理器（核心逻辑）
│   ├── GameManager.cs                # 游戏流程管理器
│   ├── InputManager.cs               # 输入处理管理器
│   ├── UIManager.cs                  # UI显示和交互管理器
│   ├── UIBuilder.cs                  # 运行时UI构建器
│   ├── CatAnimationController.cs    # 小猫动画控制器
│   ├── CatAnimationBridge.cs        # 动画状态桥接
│   ├── GameInitializer.cs           # 游戏初始化脚本
│   ├── PlaceholderSpriteGenerator.cs # 占位符精灵生成器
│   └── ControlsHint.cs              # 控制提示显示
├── Resources/
│   ├── Sprites/                      # 精灵资源
│   └── LowGothic_8x10_Regular SDF.asset  # 像素字体
├── SETUP_GUIDE.md                    # 详细设置指南
└── README.md                         # 本文件
```

## 🎨 自定义

### 添加自定义精灵

游戏默认使用彩色占位符。要添加真实的像素艺术：

1. 运行游戏后，在 Hierarchy 中找到 "CatDisplay"
2. 在 Inspector 中的 `CatAnimationController` 组件中分配精灵：
   - Idle Sprite（闲置）
   - Eating Sprite（吃饭）
   - Playing Sprite（玩耍）
   - Bathing Sprite（洗澡）
   - Happy Sprite（开心）
   - Sad Sprite（伤心）
   - Sick Sprite（生病）
   - Angel Sprites（仙女化序列）

### 调整游戏难度

在 Hierarchy 中选择 "CatManager"，在 Inspector 中调整：

- `Hunger Decay Rate`：饱食度衰减速率（默认 0.5）
- `Happiness Decay Rate`：心情衰减速率（默认 0.3）
- `Hygiene Decay Rate`：清洁度衰减速率（默认 0.2）
- `Critical Threshold`：危险阈值（默认 10）
- `Critical Duration`：危险持续时间（默认 30秒）

降低衰减速率会使游戏更容易，提高则更困难。

### 禁用占位符

如果你已经添加了自己的精灵资源：

1. 在 Hierarchy 中找到 "PlaceholderSpriteGenerator"
2. 在 Inspector 中取消勾选 `Use Placeholders`

## 🏗️ 架构设计

### 核心系统

1. **GameManager**：总体游戏流程控制，暂停/继续，时间缩放
2. **CatManager**：管理小猫的所有属性和行为，处理互动逻辑
3. **InputManager**：统一的输入处理，事件驱动
4. **UIManager**：UI显示更新，菜单导航，游戏结束处理

### 设计模式

- **单例模式**：所有管理器使用单例模式，便于全局访问
- **事件驱动**：使用 C# 事件进行组件间通信，降低耦合
- **组件化**：功能模块化，易于扩展和维护

### 数据流

```
InputManager → UIManager → CatManager → UIManager
     ↓            ↓           ↓           ↓
  输入事件    菜单选择    属性更新    UI刷新
```

## 🔧 扩展建议

### 已实现的功能
- ✅ 基础属性系统
- ✅ 三大互动系统（喂食、玩耍、清洁）
- ✅ 金钱系统
- ✅ 失败条件和游戏结束
- ✅ 完整的UI系统
- ✅ 键盘控制

### 未来可扩展功能
- ⭐ 小游戏系统（赚取金钱）
- ⭐ 成长系统（不同年龄阶段）
- ⭐ 装饰系统（购买装饰品）
- ⭐ 音效和背景音乐
- ⭐ 保存/加载系统
- ⭐ 多只宠物
- ⭐ 成就系统
- ⭐ 日夜循环

## 📝 开发笔记

### 技术要点

1. **运行时UI生成**：使用 `UIBuilder` 在运行时动态创建UI，避免手动编辑复杂的场景文件
2. **占位符系统**：`PlaceholderSpriteGenerator` 允许在没有美术资源时测试游戏
3. **桥接模式**：`CatAnimationBridge` 连接逻辑和表现层
4. **事件系统**：使用 C# 事件实现松耦合的组件通信

### 性能优化

- 使用对象池（可选）
- 减少每帧的属性更新
- 使用 TextMesh Pro 提高文本渲染性能
- 像素艺术使用 Point 过滤模式

## 🐛 已知问题

目前没有已知的重大问题。如果发现问题，请检查：

1. Unity 版本是否兼容
2. TextMesh Pro 是否正确导入
3. 所有脚本是否在 Scripts 文件夹中
4. 场景中是否有 GameInitializer

## 📄 许可证

本项目仅供学习和参考使用。

## 👨‍💻 作者

**设计者：** Manus  
**实现日期：** 2025年10月22日

## 🙏 致谢

灵感来源于经典的拓麻歌子（Tamagotchi）电子宠物游戏。

---

**祝你玩得开心！记得好好照顾你的小猫！** 🐱
