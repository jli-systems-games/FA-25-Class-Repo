using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VSX.Utilities
{
    /// <summary>
    /// Stores a reference to a renderer and its color key, and makes it easy to modify its material properties.
    /// </summary>
    [System.Serializable]
    public class RendererColorController
    {

        public Renderer renderer;
        public int materialIndex = 0;
        public string colorID = "";
        public bool overrideHue = true;
        public bool overrideSaturation = true;
        public bool overrideValue = true;
        public bool preserveAlpha = true;

        protected float baseAlpha;
        
        // 缓存材质实例以避免反复创建副本
        protected Material cachedMaterial;
        protected Material[] cachedMaterials;


        public RendererColorController(Renderer renderer, string colorID = "")
        {
            this.renderer = renderer;
            this.colorID = colorID;

            Initialize();
        }


        public virtual void Initialize()
        {
            // 创建并缓存材质实例
            if (renderer != null)
            {
                if (colorID != "" && materialIndex >= 0)
                {
                    cachedMaterials = renderer.materials;
                    if (materialIndex < cachedMaterials.Length)
                    {
                        cachedMaterial = cachedMaterials[materialIndex];
                    }
                }
                else
                {
                    cachedMaterial = renderer.material;
                }
            }
            
            baseAlpha = GetColor().a;
        }
        
        /// <summary>
        /// 清理创建的材质实例
        /// </summary>
        public virtual void Cleanup()
        {
            if (cachedMaterials != null)
            {
                foreach (var mat in cachedMaterials)
                {
                    if (mat != null) Object.Destroy(mat);
                }
                cachedMaterials = null;
            }
            else if (cachedMaterial != null)
            {
                Object.Destroy(cachedMaterial);
                cachedMaterial = null;
            }
        }


        /// <summary>
        /// Set the alpha of this animated renderer. This will completely override the current value.
        /// </summary>
        /// <param name="alpha">The alpha for this renderer.</param>
        public virtual void SetAlpha(float alpha)
        {
            if (cachedMaterial == null) return;
            
            Color c;
            if (colorID != "")
            {
                c = cachedMaterial.GetColor(colorID);
            }
            else
            {
                c = cachedMaterial.color;
            }
            
            c.a = alpha;
            
            if (colorID != "")
            {
                cachedMaterial.SetColor(colorID, c);
            }
            else
            {
                cachedMaterial.color = c;
            }
        }


        public virtual void ApplyAlpha(float alpha)
        {
            SetAlpha(preserveAlpha ? baseAlpha * alpha : alpha);
        }


        /// <summary>
        /// Get the color of this animated renderer.
        /// </summary>
        /// <returns>The color.</returns>
        public virtual Color GetColor()
        {
            if (cachedMaterial == null) return Color.white;
            
            if (colorID != "")
            {
                return cachedMaterial.GetColor(colorID);
            }
            else
            {
                return cachedMaterial.color;
            }
        }


        public virtual void ApplyColor(Color c)
        {
            float hueOverrideValue, saturationOverrideValue, valueOverrideValue;
            Color.RGBToHSV(c, out hueOverrideValue, out saturationOverrideValue, out valueOverrideValue);

            float alpha = GetColor().a;
            float h, s, v;
            Color.RGBToHSV(GetColor(), out h, out s, out v);
            if (overrideHue) h = hueOverrideValue;
            if (overrideSaturation) s = saturationOverrideValue;
            if (overrideValue) v = valueOverrideValue;
            c = Color.HSVToRGB(h, s, v, true);
            c.a = alpha;

            SetColor(c);
        }


        /// <summary>
        /// Set the color of this animated renderer. This will completely override the current value.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public virtual void SetColor(Color newColor)
        {
            if (cachedMaterial == null) return;
            
            if (colorID != "")
            {
                cachedMaterial.SetColor(colorID, newColor);
            }
            else
            {
                cachedMaterial.color = newColor;
            }
        }
    }
}
