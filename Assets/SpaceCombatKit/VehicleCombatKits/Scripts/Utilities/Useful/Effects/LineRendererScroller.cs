using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.UniversalVehicleCombat
{
    /// <summary>
    /// Scrolls the UV position of a line renderer (e.g. for a beam).
    /// </summary>
    public class LineRendererScroller : MonoBehaviour
    {
        [SerializeField]
        protected LineRenderer lineRenderer;

        [SerializeField]
        protected float scrollSpeedX = -5;

        [SerializeField]
        protected float tiling = 0.005f;
        
        // 缓存材质实例以避免每帧创建新副本
        protected Material cachedMaterial;


        protected void Reset()
        {
            lineRenderer = GetComponent<LineRenderer>();
        }
        
        protected void Start()
        {
            // 创建材质实例副本一次并缓存
            if (lineRenderer != null)
            {
                cachedMaterial = lineRenderer.material;
            }
        }
        
        protected void OnDestroy()
        {
            // 清理创建的材质实例
            if (cachedMaterial != null)
            {
                Destroy(cachedMaterial);
                cachedMaterial = null;
            }
        }

        // Called every frame
        private void Update()
        {
            if (cachedMaterial == null || lineRenderer == null) return;

            float length = Vector3.Distance(lineRenderer.GetPosition(0), lineRenderer.GetPosition(1));
            float scrollSpeed = scrollSpeedX;
            float nextTiling = tiling * length;

            cachedMaterial.SetTextureOffset("_MainTex", new Vector2((Time.time * scrollSpeed) % 1.0f, 0f));
            cachedMaterial.SetTextureScale("_MainTex", new Vector2(nextTiling, 1));

        }
    }
}