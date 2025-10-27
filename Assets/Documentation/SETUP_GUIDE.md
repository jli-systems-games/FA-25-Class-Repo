# 拓麻歌子游戏 - Unity 6 完整设置指南

## 游戏特性
✅ 完整的虚拟宠物养成系统
✅ 仅使用方向键和空格键操作
✅ 4个成长阶段：蛋 → 婴儿 → 儿童 → 成年
✅ 生命周期管理：饥饿、快乐、健康、年龄
✅ 互动功能：喂食、玩耍、治疗、清洁、睡觉
✅ 像素风格图形
✅ 自动动画系统

## 操作说明
- **空格键**：打开/确认菜单
- **↑/↓ 方向键**：选择菜单选项
- **←/→ 方向键**：关闭菜单

## Unity 项目设置步骤

### 第一步：创建新项目
1. 打开 Unity Hub
2. 点击 "New Project"
3. 选择 "2D (URP)" 模板
4. 命名为 "TamagotchiGame"
5. 点击 "Create Project"

### 第二步：创建脚本
1. 将以下三个脚本文件复制到 `Assets/Scripts` 文件夹：
   - TamagotchiGame.cs
   - PixelSpriteGenerator.cs
   - TamagotchiSetup.cs

### 第三步：创建UI场景

#### 1. 创建Canvas
- 右键 Hierarchy → UI → Canvas
- Canvas Scaler 设置：
  - UI Scale Mode: Scale With Screen Size
  - Reference Resolution: 320 x 480 (复古像素风格)
  - Match: 0.5

#### 2. 创建背景
- 右键 Canvas → UI → Image (命名为 "Background")
- Color: 淡绿色 (#C8E6C9) - 拓麻歌子经典颜色
- Anchor: Stretch/Stretch

#### 3. 创建宠物显示区域
- 右键 Canvas → UI → Image (命名为 "PetDisplay")
- 位置: X=0, Y=50, Width=200, Height=200
- Color: 白色

#### 4. 创建宠物图像
- 右键 PetDisplay → UI → Image (命名为 "PetImage")
- Width=128, Height=128
- Preserve Aspect: ✓

#### 5. 创建状态栏
在 Canvas 下创建以下 Text 元素：

**饥饿度文本** (命名为 "HungerText")
- 位置: X=-100, Y=150
- Text: "饥饿: 100"
- Font Size: 16
- Color: 黑色

**快乐度文本** (命名为 "HappinessText")
- 位置: X=100, Y=150
- Text: "快乐: 100"
- Font Size: 16
- Color: 黑色

**健康度文本** (命名为 "HealthText")
- 位置: X=-100, Y=120
- Text: "健康: 100"
- Font Size: 16
- Color: 黑色

**年龄文本** (命名为 "AgeText")
- 位置: X=100, Y=120
- Text: "年龄: 0岁"
- Font Size: 16
- Color: 黑色

**状态文本** (命名为 "StatusText")
- 位置: X=0, Y=-100
- Width=280, Height=60
- Text: "欢迎来到拓麻歌子！"
- Font Size: 14
- Color: 深灰色
- Alignment: Center
- Vertical Overflow: Truncate

#### 6. 创建菜单面板
- 右键 Canvas → UI → Panel (命名为 "MenuPanel")
- 位置: X=0, Y=-50
- Width=250, Height=200
- Color: 半透明白色 (R=1, G=1, B=1, A=0.9)

在 MenuPanel 下创建 6 个 Text 元素（MenuOption0-5）：
每个选项配置如下：
- Width=200, Height=25
- Font Size: 14
- Alignment: Left/Center

位置分配（Y坐标）：
- MenuOption0: Y=75  → "喂食"
- MenuOption1: Y=45  → "玩耍"
- MenuOption2: Y=15  → "治疗"
- MenuOption3: Y=-15 → "清洁"
- MenuOption4: Y=-45 → "睡觉"
- MenuOption5: Y=-75 → "状态"

#### 7. 创建光标图像（可选）
- 右键 MenuPanel → UI → Image (命名为 "CursorImage")
- Width=20, Height=20
- Color: 黄色

### 第四步：设置游戏对象

#### 1. 创建GameController
- 创建空GameObject (命名为 "GameController")
- 添加 TamagotchiGame 组件
- 添加 TamagotchiSetup 组件

#### 2. 连接引用
在 TamagotchiGame 组件中：

**UI References:**
- Hunger Text → HungerText
- Happiness Text → HappinessText
- Health Text → HealthText
- Age Text → AgeText
- Status Text → StatusText
- Pet Image → PetImage
- Menu Panel → MenuPanel
- Menu Options → 数组大小设为6，拖入所有 MenuOption0-5
- Cursor Image → CursorImage (可选)

#### 3. 连接Setup脚本
在 TamagotchiSetup 组件中：
- Game Controller → GameController (拖入TamagotchiGame组件)

### 第五步：项目设置

#### 1. Player Settings
- Edit → Project Settings → Player
- Resolution and Presentation:
  - Default Screen Width: 640
  - Default Screen Height: 960
  - Fullscreen Mode: Windowed

#### 2. 输入设置（已使用Input.GetKeyDown，无需额外设置）

### 第六步：测试游戏

1. 点击 Play 按钮
2. 等待10秒，蛋会孵化
3. 测试操作：
   - 按空格键打开菜单
   - 用方向键↑↓选择选项
   - 按空格键确认选择
   - 按方向键←或→关闭菜单

## 游戏机制详解

### 属性系统
- **饥饿度**: 每30秒 -10，低于20时健康 -5
- **快乐度**: 每45秒 -10，低于20时健康 -3
- **健康度**: 受饥饿、快乐、疾病影响
- **年龄**: 每60秒 +1岁

### 成长阶段
- **蛋期**: 0岁，孵化需要10秒
- **婴儿期**: 0-2岁
- **儿童期**: 3-6岁
- **成年期**: 7岁以上

### 菜单功能

1. **喂食**
   - 饥饿度 +30
   - 快乐度 +10
   - 播放进食动画

2. **玩耍**
   - 快乐度 +30
   - 饥饿度 -10
   - 播放快乐动画

3. **治疗**
   - 仅在生病时可用
   - 治愈疾病
   - 健康度 +30

4. **清洁**
   - 清除便便
   - 快乐度 +20
   - 可能治愈疾病

5. **睡觉**
   - 切换睡眠状态
   - 睡眠时属性下降速度减半

6. **状态**
   - 显示详细信息
   - 当前阶段、年龄、便便数量、健康状态

### 特殊机制

**便便系统**
- 每120秒产生1个便便
- 最多4个便便
- 便便会降低快乐度
- 3个以上便便会导致生病

**疾病系统**
- 低饥饿/快乐度可能导致生病
- 太多便便会生病
- 生病会降低健康度
- 需要用药物治疗

**死亡**
- 健康度降至0时死亡
- 显示幽灵精灵
- 游戏结束

## 优化建议

### 视觉优化
1. 添加像素化后处理效果
2. 添加屏幕边框装饰
3. 添加便便图标显示
4. 添加粒子效果（心形、星星等）

### 音效添加
1. 按键音效
2. 喂食音效
3. 玩耍音效
4. 警告音效
5. 背景音乐

### 功能扩展
1. 多个宠物品种
2. 小游戏系统
3. 成就系统
4. 保存/加载功能
5. 多语言支持

## 代码扩展示例

### 添加保存功能
```csharp
public void SaveGame()
{
    PlayerPrefs.SetFloat("Hunger", hunger);
    PlayerPrefs.SetFloat("Happiness", happiness);
    PlayerPrefs.SetFloat("Health", health);
    PlayerPrefs.SetInt("Age", age);
    PlayerPrefs.SetInt("Stage", (int)currentStage);
    PlayerPrefs.Save();
}

public void LoadGame()
{
    if (PlayerPrefs.HasKey("Hunger"))
    {
        hunger = PlayerPrefs.GetFloat("Hunger");
        happiness = PlayerPrefs.GetFloat("Happiness");
        health = PlayerPrefs.GetFloat("Health");
        age = PlayerPrefs.GetInt("Age");
        currentStage = (PetStage)PlayerPrefs.GetInt("Stage");
    }
}
```

### 添加音效
```csharp
public AudioClip buttonSound;
public AudioClip feedSound;
public AudioClip playSound;
AudioSource audioSource;

void PlaySound(AudioClip clip)
{
    audioSource.PlayOneShot(clip);
}
```

### 添加小游戏
```csharp
public void StartMiniGame()
{
    // 简单的按键游戏
    StartCoroutine(ButtonTimingGame());
}

IEnumerator ButtonTimingGame()
{
    statusText.text = "按空格键!";
    yield return new WaitForSeconds(Random.Range(1f, 3f));
    
    float startTime = Time.time;
    while (!Input.GetKeyDown(KeyCode.Space))
    {
        if (Time.time - startTime > 2f)
        {
            statusText.text = "太慢了!";
            yield break;
        }
        yield return null;
    }
    
    statusText.text = "太棒了! +20快乐";
    happiness += 20;
}
```

## 故障排除

### 问题1：精灵不显示
- 检查 PetImage 的 Image 组件是否启用
- 确认 Setup 脚本已正确执行
- 检查 Console 是否有错误

### 问题2：按键无响应
- 确认 Game 窗口处于焦点状态
- 检查菜单是否正确打开/关闭
- 查看 Input.GetKeyDown 是否正常工作

### 问题3：属性不变化
- 检查 Time.deltaTime 是否正常
- 确认游戏未暂停
- 查看 Update 方法是否被调用

### 问题4：动画不播放
- 确认动画精灵数组已填充
- 检查 animationSpeed 值
- 验证 currentAnimation 不为 null

## 完成！

现在你有了一个完整的拓麻歌子游戏！享受养育你的虚拟宠物吧！

记住：
- 定期喂食和玩耍
- 保持清洁
- 生病时及时治疗
- 好好照顾你的宠物，让它健康成长！
