using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace VSX.UI
{

    [System.Serializable]
    public class ColorParameters
    {
        public float alpha;
        public float hue;
        public float saturation;
        public float lightness;

        public ColorParameters(Color col)
        {
            Color.RGBToHSV(col, out hue, out saturation, out lightness);
            alpha = col.a;
        }
    }

    /// <summary>
    /// Manages the color of UI elements and renderers.
    /// </summary>
    public class UIColorManager : MonoBehaviour
    {

        [SerializeField]
        protected bool preserveAlpha = true;  // Whether to preserve the original alpha of each UI element

        [SerializeField]
        protected bool preserveSaturation = false;  // Whether to preserve the original saturation of each UI element

        [SerializeField]
        protected bool preserveLightness = false;  // Whether to preserve the original lightness of each UI element

        [SerializeField]
        protected List<Image> images = new List<Image>();
        protected List<ColorParameters> imageColorParameters;

        [SerializeField]
        protected List<UVCText> texts = new List<UVCText>();
        protected List<ColorParameters> textColorParameters;

        [SerializeField]
        protected List<Renderer> renderers = new List<Renderer>();
        protected List<ColorParameters> rendererColorParameters;
        
        // 缓存材质实例以避免内存泄漏
        protected List<Material> cachedRendererMaterials = new List<Material>();


        protected virtual void Awake()
        {
            imageColorParameters = new List<ColorParameters>();
            for(int i = 0; i < images.Count; ++i)
            {
                imageColorParameters.Add(new ColorParameters(images[i].color));
            }

            textColorParameters = new List<ColorParameters>();
            for (int i = 0; i < texts.Count; ++i)
            {
                textColorParameters.Add(new ColorParameters(texts[i].color));
            }

            rendererColorParameters = new List<ColorParameters>();
            cachedRendererMaterials.Clear();
            for (int i = 0; i < renderers.Count; ++i)
            {
                // 创建材质实例并缓存
                Material mat = renderers[i].material;
                cachedRendererMaterials.Add(mat);
                rendererColorParameters.Add(new ColorParameters(mat.color));
            }
        }
        
        protected virtual void OnDestroy()
        {
            // 清理创建的材质实例
            foreach (Material mat in cachedRendererMaterials)
            {
                if (mat != null)
                {
                    Destroy(mat);
                }
            }
            cachedRendererMaterials.Clear();
        }
     
        /// <summary>
        /// Set the color of the UI elements.
        /// </summary>
        /// <param name="newColor">The new color.</param>
        public virtual void SetColor(Color newColor)
        {

            if (!gameObject.activeInHierarchy) return;

            float h, s, v, a;

            // Update color of images 
            for (int i = 0; i < imageColorParameters.Count; ++i)
            {
                Color.RGBToHSV(newColor, out h, out s, out v);
                a = newColor.a;

                if (preserveLightness) v = imageColorParameters[i].lightness;

                if (preserveSaturation) s = imageColorParameters[i].saturation;

                Color col = Color.HSVToRGB(h, s, v);
                col.a = a;

                if (preserveAlpha) col.a = imageColorParameters[i].alpha;

                images[i].color = col;
            }

            // Update the color of texts
            for (int i = 0; i < textColorParameters.Count; ++i)
            {
                Color.RGBToHSV(newColor, out h, out s, out v);
                a = newColor.a;

                if (preserveLightness) v = textColorParameters[i].lightness;

                if (preserveSaturation) s = textColorParameters[i].saturation;

                Color col = Color.HSVToRGB(h, s, v);
                col.a = a;

                if (preserveAlpha) col.a = textColorParameters[i].alpha;

                texts[i].color = col;
            }

            // Update the color of renderers
            for (int i = 0; i < rendererColorParameters.Count; ++i)
            {
                if (i >= cachedRendererMaterials.Count) continue;
                
                Color.RGBToHSV(newColor, out h, out s, out v);
                a = newColor.a;

                if (preserveLightness) v = rendererColorParameters[i].lightness;

                if (preserveSaturation) s = rendererColorParameters[i].saturation;

                Color col = Color.HSVToRGB(h, s, v);
                col.a = a;

                if (preserveAlpha) col.a = rendererColorParameters[i].alpha;

                // 使用缓存的材质实例
                cachedRendererMaterials[i].color = col;
            }
        }
    }
}