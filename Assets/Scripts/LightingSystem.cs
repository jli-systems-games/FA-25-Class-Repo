using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 阳光系统管理器
/// 基于碰撞箱的阳光分布计算系统
/// </summary>
public class LightingSystem : MonoBehaviour
{
    public static LightingSystem Instance { get; private set; }

    [Header("阳光设置")]
    [SerializeField] private float baseLight = 100f; // 基础阳光值
    [SerializeField] private float minLight = 5f;     // 最低阳光值

    [Header("调试")]
    [SerializeField] private bool showDebugGizmos = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 计算碰撞箱区域的阳光值（使用区域重叠检测）
    /// </summary>
    public float GetLightAtCollider(BoxCollider2D sensorCollider)
    {
        if (sensorCollider == null) return baseLight;

        float lightValue = baseLight;  // 默认100%

        // 获取sensor的世界空间bounds
        Bounds sensorBounds = sensorCollider.bounds;

        // 检测与这个区域重叠的所有阴影碰撞箱
        Collider2D[] hits = Physics2D.OverlapBoxAll(
            sensorBounds.center,
            sensorBounds.size,
            0f,
            LayerMask.GetMask("PlantShadow")
        );

        // 调试日志
        Debug.Log($"[LightingSystem] 检测到 {hits.Length} 个阴影碰撞箱");

        // 累计所有覆盖该区域的阴影值
        foreach (Collider2D hit in hits)
        {
            Plant plant = hit.GetComponentInParent<Plant>();
            if (plant != null)
            {
                float shadowVal = plant.GetShadowValue();
                lightValue -= shadowVal;
                Debug.Log($"[LightingSystem] 阴影源: {plant.name}, 阴影值: {shadowVal}, 当前光照: {lightValue}");
            }
            else
            {
                Debug.LogWarning($"[LightingSystem] 碰撞箱 {hit.name} 找不到Plant组件！");
            }
        }

        // 限制在最小值
        lightValue = Mathf.Max(lightValue, minLight);

        return lightValue;
    }

    /// <summary>
    /// 计算指定位置的阳光值（点检测，用于调试）
    /// </summary>
    public float CalculateLightAtPosition(Vector2 position)
    {
        float lightValue = baseLight;  // 默认100%

        // 检测该位置是否被任何阴影碰撞箱覆盖
        Collider2D[] hits = Physics2D.OverlapPointAll(
            position,
            LayerMask.GetMask("PlantShadow")
        );

        // 累计所有覆盖该点的阴影值
        foreach (Collider2D hit in hits)
        {
            Plant plant = hit.GetComponentInParent<Plant>();
            if (plant != null)
            {
                lightValue -= plant.GetShadowValue();
            }
        }

        // 限制在最小值
        lightValue = Mathf.Max(lightValue, minLight);

        return lightValue;
    }

    /// <summary>
    /// 计算位置处的阳光百分比（兼容旧接口）
    /// </summary>
    public float GetLightPercentage(Vector2 position, float unused = 0f)
    {
        return CalculateLightAtPosition(position);
    }

    private void OnDrawGizmos()
    {
        if (!showDebugGizmos || !Application.isPlaying) return;

        // 可视化阳光分布（每隔0.5单位采样一次）
        for (float x = -10f; x <= 10f; x += 0.5f)
        {
            Vector2 samplePos = new Vector2(x, 0f);
            float light = CalculateLightAtPosition(samplePos);

            // 根据阳光值设置颜色
            float normalizedLight = Mathf.InverseLerp(minLight, baseLight, light);
            Color lightColor = Color.Lerp(Color.blue, Color.yellow, normalizedLight);
            lightColor.a = 0.5f;

            Gizmos.color = lightColor;
            Gizmos.DrawSphere(new Vector3(samplePos.x, 5f, 0f), 0.2f);
        }
    }
}