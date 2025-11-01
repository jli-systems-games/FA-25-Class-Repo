using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    public class TrailRendererScroller : MonoBehaviour
    {
        [SerializeField]
        protected Rigidbody m_Rigidbody;
        public Rigidbody Rigidbody
        {
            get { return m_Rigidbody; }
            set { m_Rigidbody = value; }
        }

        [SerializeField]
        protected TrailRenderer trailRenderer;

        [SerializeField]
        protected string textureKey = "_MainTex";

        [SerializeField]
        protected float scrollSpeedX = -5;

        [SerializeField]
        protected float tiling = 0.005f;
        
        // 缓存材质实例以避免每帧创建新副本
        protected Material cachedMaterial;


        protected void Reset()
        {
            m_Rigidbody = transform.parent.root.GetComponentInChildren<Rigidbody>();
            trailRenderer = GetComponent<TrailRenderer>();
        }
        
        protected void Start()
        {
            // 创建材质实例副本一次并缓存
            if (trailRenderer != null)
            {
                cachedMaterial = trailRenderer.material;
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

        // Update is called once per frame
        void Update()
        {
            if (cachedMaterial == null || m_Rigidbody == null) return;
            
            float scrollSpeed = scrollSpeedX;
            float nextOffset = m_Rigidbody.linearVelocity.magnitude * cachedMaterial.GetTextureScale(textureKey).x;
            cachedMaterial.SetTextureOffset(textureKey, new Vector2((-Time.time * nextOffset) % 1.0f, 0f));
        }
    }

}
