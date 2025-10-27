# 快速更新UI - 金币改时间系统

## 🎯 需要更新的UI元素

### 方法1：修改现有MoneyText（最简单）

1. **选中MoneyText GameObject**
2. **重命名：** `MoneyText` → `PlayTimeText`
3. **修改位置和样式**（可选）
4. **连接到UIManager：**
   - 选中UIManager
   - 拖入PlayTimeText到 **Play Time Text** 字段

### 方法2：创建新的时间显示

#### A. 创建游戏时长显示
1. 右键Canvas → **UI → TextMeshPro - Text**
2. 命名为 `PlayTimeText`
3. 设置：
   ```
   Text: 00:00（代码会自动更新）
   Font Size: 24
   Alignment: 居中
   Color: 黑色或自定义
   ```
4. 拖到UIManager的 **Play Time Text** 字段

#### B. 创建年龄显示（可选）
1. 右键Canvas → **UI → TextMeshPro - Text**
2. 命名为 `AgeText`
3. 设置：
   ```
   Text: 0天（代码会自动更新）
   Font Size: 20
   Alignment: 居中
   Color: 灰色
   ```
4. 拖到UIManager的 **Age Text** 字段

---

## 🎨 推荐布局示例

### 布局1：仅显示时长
```
┌─────────────────┐
│      猫咪       │
│      (=^･^=)    │
│                 │
│  [饱食度 ███▁▁] │
│  [心  情 ████▁] │
│  [清洁度 ████▁] │
│                 │
│  ⏱ 00:15:30     │  ← PlayTimeText
│                 │
│  [喂食][玩耍][清洁]
└─────────────────┘
```

### 布局2：显示时长+年龄
```
┌─────────────────┐
│      猫咪       │
│      (=^･^=)    │
│                 │
│  [饱食度 ███▁▁] │
│  [心  情 ████▁] │
│  [清洁度 ████▁] │
│                 │
│  ⏱ 00:15:30     │  ← PlayTimeText
│  🎂 0天         │  ← AgeText
│                 │
│  [喂食][玩耍][清洁]
└─────────────────┘
```

### 布局3：仅显示年龄
```
┌─────────────────┐
│      猫咪       │
│      (=^･^=)    │
│                 │
│  [饱食度 ███▁▁] │
│  [心  情 ████▁] │
│  [清洁度 ████▁] │
│                 │
│     3天         │  ← AgeText
│                 │
│  [喂食][玩耍][清洁]
└─────────────────┘
```

---

## ⚡ 3分钟快速设置

### 步骤1：选中原MoneyText
- 在Hierarchy中找到显示金币的TextMeshPro对象

### 步骤2：重命名
- 改名为 `PlayTimeText`

### 步骤3：连接
- 选中UIManager
- 把PlayTimeText拖到 **Play Time Text** 字段

### 步骤4：测试
- 运行游戏
- 应该看到 `00:00` 并且每秒增加

---

## 🔧 Inspector设置对照

### UIManager应该显示：

```
UI Manager (Script)
├── UI引用
│   ├── Selection Indicators [3]
│   ├── Stats Bars [3]
│   ├── Play Time Text ← 拖入PlayTimeText（必需）
│   ├── Age Text ← 拖入AgeText（可选）
│   ├── Sub Menu Panel
│   ├── Sub Menu Text
│   ├── Game Over Panel
│   └── Game Over Text
│
├── 喂食面板
│   ...
```

**注意：**
- `Play Time Text` 和 `Age Text` 可以不连接
- 不连接不会报错，只是不显示时间

---

## ✅ 测试步骤

1. **运行游戏**
2. **检查时间显示：**
   - PlayTimeText 应该显示 `00:00`
   - 每秒时间增加
3. **等待1分钟：**
   - 应该显示 `01:00`
4. **检查年龄显示：**
   - AgeText 应该显示 `0天`
   - 86400秒后变成 `1天`
5. **测试喂食/玩耍/清洁：**
   - 不再需要金币
   - 直接生效

---

## 💡 如果不想显示时间

### 完全不显示：
- 不连接Play Time Text和Age Text字段
- 游戏正常运行

### 只在游戏结束显示：
- 不连接Play Time Text和Age Text
- 游戏结束时会显示统计信息：
  ```
  BYE DAD
  
  存活: 0天
  游戏时长: 00:15:30
  ```

---

## 🎯 完成！

现在金币系统已经完全移除，改为时间追踪系统！

**关键变化：**
- ❌ 不再有金币
- ✅ 喂食、玩耍、清洁都免费
- ✅ 追踪游戏总时长
- ✅ 追踪宠物存活天数
- ✅ 游戏结束显示统计

查看 `金币改时间系统说明.md` 了解详细技术说明！

