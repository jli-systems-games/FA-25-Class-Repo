using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Sprite序列动画控制器
/// 将此脚本附加到带有Image组件的GameObject上
/// 可以自动循环播放sprite序列
/// </summary>
[RequireComponent(typeof(Image))]
public class SpriteAnimator : MonoBehaviour
{
    [Header("动画设置")]
    [Tooltip("要循环播放的sprite序列")]
    public Sprite[] sprites;
    
    [Tooltip("每帧间隔时间（秒）")]
    public float frameRate = 0.2f;
    
    [Tooltip("游戏对象激活时自动播放")]
    public bool playOnEnable = true;
    
    [Tooltip("循环播放")]
    public bool loop = true;
    
    // 私有变量
    private Image targetImage;
    private Coroutine animationCoroutine;
    private int currentFrame = 0;
    private bool isPlaying = false;
    
    void Awake()
    {
        targetImage = GetComponent<Image>();
    }
    
    void OnEnable()
    {
        if (playOnEnable && sprites != null && sprites.Length > 0)
        {
            Play();
        }
    }
    
    void OnDisable()
    {
        Stop();
    }
    
    /// <summary>
    /// 开始播放动画
    /// </summary>
    public void Play()
    {
        if (sprites == null || sprites.Length == 0)
        {
            Debug.LogWarning($"SpriteAnimator ({gameObject.name}): 没有设置sprite序列！");
            return;
        }
        
        if (targetImage == null)
        {
            Debug.LogWarning($"SpriteAnimator ({gameObject.name}): 未找到Image组件！");
            return;
        }
        
        Stop(); // 停止之前的动画
        currentFrame = 0;
        animationCoroutine = StartCoroutine(PlayAnimation());
        isPlaying = true;
    }
    
    /// <summary>
    /// 停止播放动画
    /// </summary>
    public void Stop()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isPlaying = false;
    }
    
    /// <summary>
    /// 暂停动画（保持当前帧）
    /// </summary>
    public void Pause()
    {
        if (animationCoroutine != null)
        {
            StopCoroutine(animationCoroutine);
            animationCoroutine = null;
        }
        isPlaying = false;
    }
    
    /// <summary>
    /// 恢复播放
    /// </summary>
    public void Resume()
    {
        if (!isPlaying && sprites != null && sprites.Length > 0)
        {
            animationCoroutine = StartCoroutine(PlayAnimation());
            isPlaying = true;
        }
    }
    
    /// <summary>
    /// 重置到第一帧
    /// </summary>
    public void Reset()
    {
        Stop();
        currentFrame = 0;
        if (sprites != null && sprites.Length > 0 && targetImage != null)
        {
            targetImage.sprite = sprites[0];
        }
    }
    
    /// <summary>
    /// 设置到指定帧
    /// </summary>
    public void SetFrame(int frameIndex)
    {
        if (sprites == null || sprites.Length == 0) return;
        
        currentFrame = Mathf.Clamp(frameIndex, 0, sprites.Length - 1);
        if (targetImage != null)
        {
            targetImage.sprite = sprites[currentFrame];
        }
    }
    
    /// <summary>
    /// 动画播放协程
    /// </summary>
    private IEnumerator PlayAnimation()
    {
        while (true)
        {
            // 设置当前帧
            if (targetImage != null && sprites[currentFrame] != null)
            {
                targetImage.sprite = sprites[currentFrame];
            }
            
            // 等待下一帧
            yield return new WaitForSeconds(frameRate);
            
            // 切换到下一帧
            currentFrame++;
            
            // 检查是否结束
            if (currentFrame >= sprites.Length)
            {
                if (loop)
                {
                    currentFrame = 0; // 循环
                }
                else
                {
                    currentFrame = sprites.Length - 1;
                    Stop(); // 停止播放
                    yield break;
                }
            }
        }
    }
    
    /// <summary>
    /// 获取动画是否正在播放
    /// </summary>
    public bool IsPlaying()
    {
        return isPlaying;
    }
    
    /// <summary>
    /// 获取当前帧索引
    /// </summary>
    public int GetCurrentFrame()
    {
        return currentFrame;
    }
    
    /// <summary>
    /// 获取总帧数
    /// </summary>
    public int GetFrameCount()
    {
        return sprites != null ? sprites.Length : 0;
    }
}

