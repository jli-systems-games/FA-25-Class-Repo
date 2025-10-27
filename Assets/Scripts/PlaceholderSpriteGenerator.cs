using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 生成占位符精灵，用于测试
/// 在没有美术资源时，自动生成简单的彩色方块作为占位符
/// </summary>
public class PlaceholderSpriteGenerator : MonoBehaviour
{
    [Header("是否使用占位符")]
    public bool usePlaceholders = true;
    
    void Start()
    {
        if (!usePlaceholders) return;
        
        // 等待一帧，确保UIBuilder已经创建了UI
        Invoke(nameof(GeneratePlaceholders), 0.1f);
    }
    
    void GeneratePlaceholders()
    {
        // 为小猫图像生成占位符
        CatAnimationController catAnim = FindFirstObjectByType<CatAnimationController>();
        if (catAnim != null)
        {
            catAnim.idleSprite = CreateColoredSprite(Color.gray, "Idle");
            catAnim.sleepingSprite = CreateColoredSprite(Color.blue, "Sleeping");
            catAnim.eatingSprite = CreateColoredSprite(Color.yellow, "Eating");
            catAnim.playingSprite = CreateColoredSprite(Color.green, "Playing");
            catAnim.bathingSprite = CreateColoredSprite(Color.cyan, "Bathing");
            catAnim.happySprite = CreateColoredSprite(new Color(1f, 0.5f, 0.8f), "Happy");
            catAnim.sadSprite = CreateColoredSprite(new Color(0.5f, 0.5f, 0.8f), "Sad");
            catAnim.sickSprite = CreateColoredSprite(new Color(0.6f, 0.4f, 0.2f), "Sick");
            catAnim.dyingSprite = CreateColoredSprite(new Color(0.3f, 0.3f, 0.3f), "Dying");
            
            // 仙女化动画序列
            catAnim.angelSprites = new Sprite[5];
            for (int i = 0; i < 5; i++)
            {
                float alpha = 1f - (i * 0.2f);
                Color angelColor = new Color(1f, 1f, 1f, alpha);
                catAnim.angelSprites[i] = CreateColoredSprite(angelColor, $"Angel_{i}");
            }
            
            // 设置初始精灵
            if (catAnim.catImage != null)
            {
                catAnim.catImage.sprite = catAnim.idleSprite;
            }
        }
        
        // 菜单图标已移除，因为用户使用自定义UI
        // 不再需要生成占位符图标
        
        Debug.Log("占位符精灵已生成。游戏可以运行，但建议添加真实的像素艺术资源以获得更好的视觉效果。");
    }
    
    /// <summary>
    /// 创建纯色精灵
    /// </summary>
    Sprite CreateColoredSprite(Color color, string name = "Placeholder")
    {
        int size = 256;
        Texture2D texture = new Texture2D(size, size);
        
        // 填充纯色
        Color[] pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }
        
        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point; // 像素风格
        texture.Apply();
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
        sprite.name = name;
        
        return sprite;
    }
    
    /// <summary>
    /// 创建带边框的图标精灵
    /// </summary>
    Sprite CreateIconSprite(Color color)
    {
        int size = 128;
        Texture2D texture = new Texture2D(size, size);
        
        Color[] pixels = new Color[size * size];
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                int index = y * size + x;
                
                // 创建边框
                if (x < 5 || x >= size - 5 || y < 5 || y >= size - 5)
                {
                    pixels[index] = Color.black;
                }
                else
                {
                    pixels[index] = color;
                }
            }
        }
        
        texture.SetPixels(pixels);
        texture.filterMode = FilterMode.Point;
        texture.Apply();
        
        Sprite sprite = Sprite.Create(
            texture,
            new Rect(0, 0, size, size),
            new Vector2(0.5f, 0.5f),
            100f
        );
        
        return sprite;
    }
}

