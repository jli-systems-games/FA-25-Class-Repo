using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 控制小猫的动画显示
/// 由于是像素风格，使用精灵切换来模拟动画
/// </summary>
public class CatAnimationController : MonoBehaviour
{
    [Header("动画精灵")]
    public Sprite idleSprite;
    public Sprite sleepingSprite;
    public Sprite eatingSprite;
    public Sprite playingSprite;
    public Sprite bathingSprite;
    public Sprite happySprite;
    public Sprite sadSprite;
    public Sprite sickSprite;
    public Sprite dyingSprite;
    public Sprite[] angelSprites; // 仙女化动画序列
    
    [Header("组件引用")]
    public Image catImage;
    
    [Header("动画设置")]
    public float animationSpeed = 0.2f; // 动画帧切换速度
    public float angelAnimationSpeed = 0.3f; // 仙女化动画速度
    
    private CatState currentState = CatState.Idle;
    private int currentFrame = 0;
    private float animationTimer = 0f;
    private bool isPlayingAngelAnimation = false;
    
    void Update()
    {
        // 更新动画
        UpdateAnimation();
    }
    
    /// <summary>
    /// 设置动画状态
    /// </summary>
    public void SetState(CatState state)
    {
        if (currentState != state)
        {
            currentState = state;
            currentFrame = 0;
            animationTimer = 0f;
            
            if (state == CatState.Angel)
            {
                isPlayingAngelAnimation = true;
            }
            
            UpdateSprite();
        }
    }
    
    /// <summary>
    /// 更新动画
    /// </summary>
    private void UpdateAnimation()
    {
        if (catImage == null) return;
        
        // 仙女化动画特殊处理
        if (isPlayingAngelAnimation && angelSprites != null && angelSprites.Length > 0)
        {
            animationTimer += Time.deltaTime;
            
            if (animationTimer >= angelAnimationSpeed)
            {
                animationTimer = 0f;
                currentFrame++;
                
                if (currentFrame >= angelSprites.Length)
                {
                    currentFrame = angelSprites.Length - 1; // 停在最后一帧
                }
                
                catImage.sprite = angelSprites[currentFrame];
            }
        }
        else
        {
            // 简单的闲置动画（可以添加更多帧）
            if (currentState == CatState.Idle || currentState == CatState.Happy)
            {
                animationTimer += Time.deltaTime;
                
                if (animationTimer >= animationSpeed)
                {
                    animationTimer = 0f;
                    // 这里可以添加闲置动画的帧切换逻辑
                }
            }
        }
    }
    
    /// <summary>
    /// 更新精灵显示
    /// </summary>
    private void UpdateSprite()
    {
        if (catImage == null) return;
        
        Sprite spriteToShow = idleSprite;
        
        switch (currentState)
        {
            case CatState.Idle:
                spriteToShow = idleSprite;
                break;
            case CatState.Sleeping:
                spriteToShow = sleepingSprite;
                break;
            case CatState.Eating:
                spriteToShow = eatingSprite;
                break;
            case CatState.Playing:
                spriteToShow = playingSprite;
                break;
            case CatState.Bathing:
                spriteToShow = bathingSprite;
                break;
            case CatState.Happy:
                spriteToShow = happySprite;
                break;
            case CatState.Sad:
                spriteToShow = sadSprite;
                break;
            case CatState.Sick:
                spriteToShow = sickSprite;
                break;
            case CatState.Dying:
                spriteToShow = dyingSprite;
                break;
            case CatState.Angel:
                if (angelSprites != null && angelSprites.Length > 0)
                {
                    spriteToShow = angelSprites[0];
                }
                break;
        }
        
        if (spriteToShow != null)
        {
            catImage.sprite = spriteToShow;
        }
    }
}

