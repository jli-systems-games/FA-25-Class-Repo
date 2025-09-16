using UnityEngine;

public class PeeShooter : MonoBehaviour
{
    [Header("Refs")]
    public Transform bottleMuzzle; // 拖入瓶嘴子对象
    public ParticleSystem peeFX; // 粒子
    public GameObject probePrefab; // Probe prefab
    public AudioSource peeSound; // 音效（可选）
    [Header("Fire")]
    [Range(5f, 15f)]
    public float minLaunchSpeed = 5f; // 最小速度
    [Range(5f, 15f)]
    public float maxLaunchSpeed = 15f; // 最大速度
    public float probesPerSecond = 25f; // 密集度
    public float urinePerProbe = 1f;
    float cd;
    private ObjectPool<PeeProbe> probePool;
    private bool isPeeing = false; // 控制喷射状态

    void Start()
    {
        probePool = new ObjectPool<PeeProbe>(probePrefab, 100);
    }

    void Update()
    {
        bool canShoot = (GameManager.I == null || GameManager.I.CanShoot()) && GameManager.I.Urine > 0;
        if (peeFX && bottleMuzzle)
        {
            peeFX.transform.rotation = bottleMuzzle.rotation; // 粒子跟随瓶嘴方向
            var emission = peeFX.emission;
            emission.rateOverTime = 100f; // 固定喷 100 颗/秒

            // 强制10秒喷射逻辑
            if (canShoot && !isPeeing)
            {
                peeFX.Play();
                isPeeing = true;
                Invoke("StopPeeing", 10f); // 10秒后停止
            }
            else if (!canShoot && isPeeing)
            {
                peeFX.Stop();
                isPeeing = false;
            }
        }
        if (peeSound)
        {
            if (canShoot && !peeSound.isPlaying) peeSound.Play();
            if (!canShoot && peeSound.isPlaying) peeSound.Stop();
        }

        if (!canShoot) return;
        cd -= Time.deltaTime;
        if (cd <= 0f)
        {
            cd = 1f / probesPerSecond;
            FireOne();
        }
    }

    void StopPeeing()
    {
        if (peeFX && isPeeing)
        {
            peeFX.Stop();
            isPeeing = false;
        }
    }

    void FireOne()
    {
        if (!bottleMuzzle || !probePrefab) return;

        float randomSpeed = Random.Range(minLaunchSpeed, maxLaunchSpeed);
        Vector2 dir = bottleMuzzle.up * randomSpeed;
        var probe = probePool.Get();
        probe.transform.position = bottleMuzzle.position;
        probe.myPool = probePool;
        var rb = probe.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = dir; // 使用 velocity
        }
        GameManager.I?.RegisterShot();
        GameManager.I?.ConsumeUrine(urinePerProbe);
    }
}