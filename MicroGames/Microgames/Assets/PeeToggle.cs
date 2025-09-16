using UnityEngine;

public class PeeToggle : MonoBehaviour
{
    public ParticleSystem peeFX;       // 拖你的 Particle System
    public KeyCode key = KeyCode.Mouse0;  // 或 KeyCode.Space
    [Header("Drunk sway (optional)")]
    public Transform cameraOrRoot;     // 让它轻微左右晃
    public float swayAmp = 0.5f;
    public float swayFreq = 0.6f;

    bool playing;
    float swayT;

    void Update()
    {
        // 开关喷射
        if (Input.GetKeyDown(key)) StartPee();
        if (Input.GetKeyUp(key)) StopPee();

        // 醉汉左右轻微晃动
        if (cameraOrRoot)
        {
            swayT += Time.deltaTime * swayFreq;
            float off = Mathf.Sin(swayT) * swayAmp;
            cameraOrRoot.localRotation = Quaternion.Euler(0, 0, off);
        }
    }

    public void StartPee()
    {
        if (peeFX && !playing)
        {
            // 确保启用 Emission
            var em = peeFX.emission; em.enabled = true;
            peeFX.Play(true);
            playing = true;
        }
    }

    public void StopPee()
    {
        if (peeFX && playing)
        {
            // 停止但不清空已在空中的粒子
            peeFX.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            playing = false;
        }
    }
}
