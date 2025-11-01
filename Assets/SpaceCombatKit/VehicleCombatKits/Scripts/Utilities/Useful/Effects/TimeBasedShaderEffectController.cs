using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Apply a time based value to a shader effect.
    /// </summary>
    public class TimeBasedShaderEffectController : MonoBehaviour
    {
        [Tooltip("The shader variable to control.")]
        [SerializeField]
        protected string shaderKey = "_UVOffsetY";

        [Tooltip("The renderer with the effect.")]
        [SerializeField]
        protected Renderer m_Renderer;

        [Tooltip("The time based effect multiplier.")]
        [SerializeField]
        protected float effectMultiplier = 1;
        
        // 缓存材质实例以避免每帧创建新副本
        protected Material cachedMaterial;


        // Called when the script is first added to a gameobject or reset in the inspector
        protected virtual void Reset()
        {
            m_Renderer = GetComponent<Renderer>();
        }
        
        protected virtual void Start()
        {
            // 创建材质实例副本一次并缓存
            if (m_Renderer != null)
            {
                cachedMaterial = m_Renderer.material;
            }
        }
        
        protected virtual void OnDestroy()
        {
            // 清理创建的材质实例
            if (cachedMaterial != null)
            {
                Destroy(cachedMaterial);
                cachedMaterial = null;
            }
        }

        // Called every frame
        protected virtual void Update()
        {
            if (cachedMaterial != null)
            {
                cachedMaterial.SetFloat(shaderKey, Time.realtimeSinceStartup * effectMultiplier);
            }
        }
    }
}