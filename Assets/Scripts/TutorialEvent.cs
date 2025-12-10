using UnityEngine;

[System.Serializable]
public class TutorialEvent
{
    [Header("Event Settings")]
    public string eventName;
    
    [Header("GameObjects Control")]
    [Tooltip("这些对象会在事件触发时被激活")]
    public GameObject[] objectsToActivate;
    
    [Tooltip("这些对象会在事件触发时被停用")]
    public GameObject[] objectsToDeactivate;
    
    [Header("Visual Effects")]
    [Tooltip("要高亮的对象")]
    public GameObject objectToHighlight;
    
    [Tooltip("箭头指示器要指向的对象")]
    public GameObject arrowTarget;
    
    [Header("Animation")]
    [Tooltip("要播放的动画名称")]
    public string animationToPlay;
    
    [Tooltip("播放动画的Animator组件")]
    public Animator animator;
    
    [Header("Audio")]
    [Tooltip("要播放的音效")]
    public AudioClip soundEffect;
    
    [Range(0f, 1f)]
    public float soundVolume = 1f;
    
    [Header("Delay")]
    [Tooltip("延迟多少秒后执行（0=立即执行）")]
    public float delaySeconds = 0f;
}
