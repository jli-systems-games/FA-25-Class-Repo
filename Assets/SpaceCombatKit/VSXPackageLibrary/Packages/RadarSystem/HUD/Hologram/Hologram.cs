using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.RadarSystem
{
    public class Hologram : MonoBehaviour
    {
        [SerializeField]
        protected Transform originalParent;
        public Transform OriginalParent { get { return originalParent; } }

        [SerializeField]
        protected Bounds bounds = new Bounds(Vector3.zero, new Vector3(10, 10, 10));
        public Bounds Bounds { get { return bounds; } }

        protected Material[] materials;
        public Material[] Materials { get { return materials; } }

        protected void Awake()
        {
            Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
            materials = new Material[renderers.Length];
            for (int i = 0; i < renderers.Length; ++i)
            {
                // 创建材质实例副本并缓存
                materials[i] = renderers[i].material;
            }

            originalParent = transform.parent;
        }
        
        protected void OnDestroy()
        {
            // 清理创建的材质实例，防止内存泄漏
            if (materials != null)
            {
                foreach (Material mat in materials)
                {
                    if (mat != null)
                    {
                        Destroy(mat);
                    }
                }
            }
        }

        // Show the bounding box visually in the editor scene view
        void OnDrawGizmosSelected()
        {
            Gizmos.matrix = Matrix4x4.TRS(transform.TransformPoint(bounds.center), transform.rotation, transform.lossyScale);
            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(Vector3.zero, bounds.size);
        }

        public virtual void SetLayer(int layer)
        {
            gameObject.layer = layer;

            foreach(Transform child in transform)
            {
                child.gameObject.layer = layer;
            }
        }
    }
}
