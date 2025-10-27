using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 游戏中使用的枚举定义
/// </summary>

// 小猫的动画状态
public enum CatState
{
    Idle,       // 闲置
    Sleeping,   // 睡觉
    Eating,     // 吃饭
    Playing,    // 玩耍
    Bathing,    // 洗澡
    Happy,      // 开心
    Sad,        // 伤心
    Sick,       // 生病
    Dying,      // 濒死
    Angel       // 仙女化
}

// 菜单选项
public enum MenuOption
{
    Feed,       // 喂食
    Play,       // 玩耍
    Clean       // 清洁
}

// 食物类型
public enum FoodType
{
    BasicFood,      // 普通猫粮
    PremiumFood,    // 高级猫粮
    Snack          // 零食
}

// 玩具类型
public enum ToyType
{
    Stick,      // 逗猫棒
    Ball        // 毛线球
}

// 清洁用品类型
public enum CleanType
{
    BasicBath,      // 普通洗澡
    PremiumBath     // 高级香波
}

