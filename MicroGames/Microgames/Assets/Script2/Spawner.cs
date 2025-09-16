using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Header("Prefab")]
    public GameObject carPrefab;

    [Header("Spawn Positions")]
    public float leftSpawnX = -12f;   // 左车从左侧（负 X）进场
    public float rightSpawnX = 12f;   // 右车从右侧（正 X）进场
    public float spawnY = -3.4f;

    [Header("Timing")]
    public float firstDelay = 1f;      // 开场等一会儿再刷左车
    public float delayAfterLeftGone = 5f; // 左车离开后再等 5 秒刷右车

    [Header("Speeds")]
    public float leftCarSpeed = 8f;  // 左车向右（正）
    public float rightCarSpeed = 12f;  // 右车向左（负）

    [Header("SFX (optional)")]
    public AudioSource carSpawnAudioSource;
    public AudioClip leftSpawnClip;
    public AudioClip rightSpawnClip;

    bool leftSpawned = false;
    bool rightSpawned = false;

    void Start()
    {
        Invoke(nameof(SpawnLeft), firstDelay);
    }

    void SpawnLeft()
    {
        if (leftSpawned) return;
        leftSpawned = true;

        var go = Instantiate(carPrefab, new Vector3(leftSpawnX, spawnY, 0f), Quaternion.identity);
        var car = go.GetComponent<CarController>();
        if (car != null)
        {
            car.speed = Mathf.Abs(leftCarSpeed); // 向右
            car.isFinalCar = false;
            Debug.Log("[Spawner] Spawned LEFT car at " + leftSpawnX + " speed=" + car.speed);
        }

        if (carSpawnAudioSource && leftSpawnClip) carSpawnAudioSource.PlayOneShot(leftSpawnClip);

        // 安排 5 秒后生成右车
        Invoke(nameof(SpawnRight), delayAfterLeftGone);
    }

    void SpawnRight()
    {
        if (rightSpawned) return;
        rightSpawned = true;

        var go = Instantiate(carPrefab, new Vector3(rightSpawnX, spawnY, 0f), Quaternion.identity);

        // 让车图在视觉上朝左（可选）
        var s = go.transform.localScale;
        s.x = -Mathf.Abs(s.x);
        go.transform.localScale = s;

        var car = go.GetComponent<CarController>();
        if (car != null)
        {
            car.speed = -Mathf.Abs(rightCarSpeed); // 向左
            car.isFinalCar = true;                 // ★ 这辆是最终车
            Debug.Log("[Spawner] Spawned RIGHT FINAL car at " + rightSpawnX + " speed=" + car.speed);
        }

        if (carSpawnAudioSource && rightSpawnClip) carSpawnAudioSource.PlayOneShot(rightSpawnClip);
    }
}
