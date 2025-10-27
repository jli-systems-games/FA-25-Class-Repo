# SpriteAnimator 使用指南

## 🎯 概述

`SpriteAnimator` 是一个独立的Sprite序列动画控制器，可以附加到任何带有Image组件的GameObject上，自动循环播放sprite序列。

**优点：**
- ✅ 独立管理动画逻辑
- ✅ 可重用（任何地方都能用）
- ✅ 减少UIManager复杂性
- ✅ 简单易用

## 📋 快速设置

### 第1步：创建面板结构

```
Canvas
├── FeedPanel (GameObject/Panel)
│   ├── AnimImage (GameObject + Image + SpriteAnimator) ← 添加SpriteAnimator
│   └── WarningText (TextMeshPro)
│
├── PlayPanel (GameObject/Panel)
│   ├── AnimImage (GameObject + Image + SpriteAnimator)
│   └── WarningText (TextMeshPro)
│
└── CleanPanel (GameObject/Panel)
    ├── AnimImage (GameObject + Image + SpriteAnimator)
    └── WarningText (TextMeshPro)
```

### 第2步：设置AnimImage（以FeedPanel为例）

#### A. 创建AnimImage GameObject
1. 右键 **FeedPanel** → **UI → Image**
2. 命名为 `AnimImage`
3. 设置大小：比如 **300x300**
4. 设置位置：**居中**

#### B. 添加SpriteAnimator组件
1. 选中 **AnimImage**
2. 点击 **Add Component**
3. 搜索 `SpriteAnimator`
4. 添加

#### C. 配置SpriteAnimator
在Inspector中：
```
Sprite Animator (Script)
├── Sprites                      ← 拖入3张sprite
│   ├── Size: 3
│   ├── Element 0: feed_01
│   ├── Element 1: feed_02
│   └── Element 2: feed_03
│
├── Frame Rate: 0.2              ← 每帧间隔0.2秒
├── Play On Enable: ✓            ← 激活时自动播放
└── Loop: ✓                      ← 循环播放
```

### 第3步：连接到UIManager

选中 **UIManager**，在Inspector中：

```
UI Manager (Script)

喂食面板：
├── Feed Panel → 拖入 FeedPanel
├── Feed Animator → 拖入 FeedPanel/AnimImage (SpriteAnimator组件)
└── Feed Warning Text → 拖入 FeedPanel/WarningText

玩耍面板：
├── Play Panel → 拖入 PlayPanel
├── Play Animator → 拖入 PlayPanel/AnimImage
└── Play Warning Text → 拖入 PlayPanel/WarningText

清洁面板：
├── Clean Panel → 拖入 CleanPanel
├── Clean Animator → 拖入 CleanPanel/AnimImage
└── Clean Warning Text → 拖入 CleanPanel/WarningText
```

## 🎨 SpriteAnimator参数说明

### Sprites (Sprite数组)
- **说明：** 要循环播放的sprite序列
- **推荐：** 3-8张sprite
- **设置：** 
  1. 点击数组，设置Size（比如3）
  2. 逐个拖入sprite图片
  3. 按顺序排列（0, 1, 2...）

### Frame Rate (浮点数)
- **说明：** 每帧间隔时间（秒）
- **默认：** 0.2秒
- **调整：**
  - `0.1` = 快速动画
  - `0.2` = 中速动画（推荐）
  - `0.3` = 慢速动画

### Play On Enable (布尔值)
- **说明：** GameObject激活时自动播放
- **默认：** ✓ 勾选
- **推荐：** 保持勾选（方便使用）

### Loop (布尔值)
- **说明：** 是否循环播放
- **默认：** ✓ 勾选
- **推荐：** 
  - 勾选 = 无限循环
  - 不勾选 = 播放一次后停止

## 🔧 工作原理

### 自动播放流程：
1. **面板激活** → `panel.SetActive(true)`
2. **AnimImage激活** → `OnEnable()` 被调用
3. **SpriteAnimator启动** → 自动调用 `Play()`
4. **循环播放** → 按Frame Rate切换sprite
5. **面板关闭** → `OnDisable()` 自动停止动画

### 手动控制（可选）：
```csharp
// 获取SpriteAnimator组件
SpriteAnimator animator = animImage.GetComponent<SpriteAnimator>();

// 播放
animator.Play();

// 停止
animator.Stop();

// 暂停（保持当前帧）
animator.Pause();

// 恢复
animator.Resume();

// 重置到第一帧
animator.Reset();

// 设置到指定帧
animator.SetFrame(2);

// 检查是否正在播放
bool isPlaying = animator.IsPlaying();

// 获取当前帧
int frame = animator.GetCurrentFrame();

// 获取总帧数
int total = animator.GetFrameCount();
```

## 🎮 测试

### 1. 测试单个动画
1. 在Scene视图选中 **AnimImage**
2. 在Inspector中勾选/取消勾选GameObject
3. 应该看到sprite循环切换

### 2. 测试游戏流程
1. 运行游戏
2. 按空格键执行动作
3. 应该看到对应的动画播放
4. 10秒内再次按空格，显示警告文字（无动画）

## 📸 Inspector截图对照

### AnimImage应该有这些组件：
```
AnimImage (GameObject)
├── Rect Transform
├── Canvas Renderer
├── Image                        ← Unity自带
│   └── Source Image: (留空)
└── Sprite Animator (Script)     ← 我们添加的
    ├── Sprites [3]
    ├── Frame Rate: 0.2
    ├── Play On Enable: ✓
    └── Loop: ✓
```

## 💡 实用技巧

### 技巧1：快速复制设置
1. 设置好第一个AnimImage（喂食）
2. 复制AnimImage（Ctrl+C）
3. 粘贴到其他面板（Ctrl+V）
4. 只需要替换Sprites数组

### 技巧2：不同速度的动画
```
Feed: Frame Rate = 0.15  (快速进食)
Play: Frame Rate = 0.2   (中速玩耍)
Clean: Frame Rate = 0.25 (慢速清洁)
```

### 技巧3：无动画的面板
如果某个面板暂时没有sprite：
1. 不添加SpriteAnimator组件
2. 或者添加但不拖入sprites
3. UIManager会自动跳过动画

### 技巧4：单次播放动画
如果想要播放一次就停止：
1. **Loop** 取消勾选
2. 动画播放完会自动停止在最后一帧

## ⚠️ 常见问题

### 问题1：动画不播放
**检查：**
- [ ] AnimImage上是否有 **Image** 组件？
- [ ] AnimImage上是否有 **SpriteAnimator** 组件？
- [ ] Sprites数组是否已拖入sprite？
- [ ] Play On Enable 是否勾选？

### 问题2：动画太快/太慢
**解决：** 调整 **Frame Rate** 值
- 太快 → 增加值（比如0.3）
- 太慢 → 减小值（比如0.1）

### 问题3：动画只播放一次
**原因：** Loop 没有勾选
**解决：** 勾选 **Loop**

### 问题4：UIManager找不到Feed Animator字段
**原因：** UIManager的字段类型从Image改成了SpriteAnimator
**解决：** 
1. 把AnimImage GameObject（不是组件）拖到字段
2. Unity会自动找到SpriteAnimator组件

### 问题5：Sprites显示为Missing
**原因：** sprite没有正确导入
**解决：**
1. 选中sprite文件
2. Inspector中设置 **Texture Type: Sprite (2D and UI)**
3. 点击 **Apply**

## 🎯 完整检查清单

### 每个面板需要：
- [ ] FeedPanel/PlayPanel/CleanPanel GameObject
- [ ] AnimImage (子对象)
  - [ ] Image 组件
  - [ ] SpriteAnimator 组件
  - [ ] 已拖入3张sprite
- [ ] WarningText (子对象)
  - [ ] TextMeshPro 组件

### UIManager需要连接：
- [ ] Feed/Play/Clean Panel (3个GameObject)
- [ ] Feed/Play/Clean Animator (3个SpriteAnimator组件)
- [ ] Feed/Play/Clean Warning Text (3个TextMeshPro)

### 测试：
- [ ] 运行游戏
- [ ] 按空格键，看到动画
- [ ] 10秒内再按，看到警告文字
- [ ] 4秒后面板自动关闭

## 🌟 优势对比

### 使用SpriteAnimator（现在）：
```
✅ 每个面板独立管理动画
✅ UIManager代码简洁
✅ 可以在Inspector直接调整速度
✅ 可重用于其他地方
✅ 容易调试和修改
```

### 之前的方式（废弃）：
```
❌ UIManager管理所有动画
❌ 需要维护sprite数组
❌ 修改动画需要改代码
❌ 不可重用
```

---

**现在动画逻辑独立了，UIManager更简洁！** 🎉

## 🔗 其他用途

SpriteAnimator不仅可以用于面板动画，还可以用于：
- 角色行走动画
- UI按钮动画
- 加载动画
- 任何需要sprite序列切换的地方

只需要把脚本添加到带有Image组件的GameObject上就可以了！

