using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(AudioSource))]
public class FanZone : MonoBehaviour
{
    public KeyCode activateKey = KeyCode.LeftArrow; // 按什么键启动
    public Vector3 forceDirection = Vector3.right;  // 风向
    public float forceStrength = 80f;               // 风力
    public float animSpeed = 1f;                    // 动画速度

    List<Rigidbody> bodies = new List<Rigidbody>();
    Animator anim;
    AudioSource audioSrc;
    bool isActive = false; // 当前是否正在吹

    void Awake()
    {
        anim = GetComponent<Animator>();
        audioSrc = GetComponent<AudioSource>();

        // Collider 必须是 Trigger
        Collider col = GetComponent<Collider>();
        col.isTrigger = true;

        // 默认不转、不响
        if (anim != null) anim.speed = 0f;
        if (audioSrc != null)
        {
            audioSrc.loop = true;
            audioSrc.playOnAwake = false;
            audioSrc.Stop();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null && !bodies.Contains(rb))
        {
            bodies.Add(rb);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.attachedRigidbody;
        if (rb != null)
        {
            bodies.Remove(rb);
        }
    }

    void Update()
    {
        bool keyHeld = Input.GetKey(activateKey);

        // 只有状态变化时才处理动画和声音
        if (keyHeld != isActive)
        {
            isActive = keyHeld;

            // 动画开 / 关
            if (anim != null)
                anim.speed = isActive ? animSpeed : 0f;

            // 声音开 / 关
            if (audioSrc != null)
            {
                if (isActive && !audioSrc.isPlaying)
                    audioSrc.Play();
                else if (!isActive && audioSrc.isPlaying)
                    audioSrc.Stop();
            }
        }
    }

    void FixedUpdate()
    {
        if (!isActive) return;

        Vector3 dir = forceDirection;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        dir = dir.normalized;

        foreach (var rb in bodies)
        {
            if (rb == null) continue;
            rb.AddForce(dir * forceStrength, ForceMode.Acceleration);
        }
    }
}
