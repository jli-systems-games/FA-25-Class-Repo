# 快速参考卡片

## 🚀 5分钟快速开始

1. 打开Unity，加载项目
2. 打开场景：`Assets/Scenes/TamagotchiHome.unity`
3. 菜单栏：`Tamagotchi → Quick Setup Scene`
4. 点击 Play ▶️

就这么简单！

## ⌨️ 操作控制

| 按键 | 功能 |
|------|------|
| ↑ ↓ | 选择菜单（喂食/玩耍/清洁） |
| ← → | 选择子选项 |
| 空格 | 确认 |
| ESC | 暂停/继续 |
| H | 显示/隐藏帮助 |
| F1 | 调试信息 |

## 📊 游戏数据

### 属性衰减
- 饱食度：-0.5/秒
- 心情：-0.3/秒
- 清洁度：-0.2/秒

### 互动效果

**喂食**
- 普通猫粮 ($5)：+20饱食，+5心情
- 高级猫粮 ($15)：+35饱食，+15心情
- 零食 ($10)：+10饱食，+20心情

**玩耍**
- 逗猫棒：+25心情，-5饱食
- 毛线球：+20心情，-5饱食

**清洁**
- 普通洗澡 ($5)：+30清洁，+5心情
- 高级香波 ($12)：+50清洁，+15心情

### 失败条件
- 任意属性 < 10
- 持续 30 秒
- → 仙女化 → 游戏结束

## 🎨 自定义

### 调整难度
在 Hierarchy 中选择 `CatManager`：

**简单**
- Hunger Decay: 0.2
- Happiness Decay: 0.15
- Hygiene Decay: 0.1
- Critical Duration: 60

**困难**
- Hunger Decay: 1.0
- Happiness Decay: 0.8
- Hygiene Decay: 0.6
- Critical Duration: 15

### 添加精灵
运行游戏后，在 Hierarchy 中找到 `CatDisplay`，在 Inspector 中分配精灵。

### 禁用占位符
在 Hierarchy 中找到 `PlaceholderSpriteGenerator`，取消勾选 `Use Placeholders`。

## 📁 重要文件

```
Assets/
├── Scenes/TamagotchiHome.unity  ← 主场景
├── Scripts/
│   ├── GameInitializer.cs       ← 添加这个到场景
│   ├── CatManager.cs            ← 核心逻辑
│   ├── UIManager.cs             ← UI控制
│   └── ...
├── README.md                    ← 完整文档
├── SETUP_GUIDE.md               ← 设置指南
└── DEPLOYMENT_CHECKLIST.md      ← 部署清单
```

## 🔧 常用菜单

Unity 菜单栏 → `Tamagotchi`：
- Quick Setup Scene - 一键设置
- Remove All Game Objects - 清理场景
- Documentation - 打开文档

## 🐛 问题排查

**没有UI？**
→ 确保场景中有 `GameInitializer`

**小猫不显示？**
→ 正常，需要添加精灵或启用占位符

**按键无反应？**
→ 点击 Game 窗口激活

**属性变化太快/慢？**
→ 调整 CatManager 的衰减速率

## 📞 获取帮助

1. 查看 README.md
2. 查看 SETUP_GUIDE.md
3. 查看代码注释
4. 按 F1 查看调试信息

## 🎯 核心脚本说明

| 脚本 | 功能 |
|------|------|
| GameInitializer | 🔑 必须添加到场景 |
| CatManager | 🐱 小猫逻辑 |
| UIManager | 🖼️ UI控制 |
| InputManager | ⌨️ 输入处理 |
| GameManager | 🎮 游戏控制 |

## 💡 提示

- 游戏会自动创建所有必要的对象
- 占位符可以让你立即测试游戏
- 所有参数都可以在 Inspector 中调整
- 按 F1 查看实时游戏数据
- 使用编辑器菜单快速设置场景

---

**需要更多信息？** 查看 README.md 📚

