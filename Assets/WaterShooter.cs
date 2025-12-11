using UnityEngine;
using System.Collections;

public class WaterShooter : MonoBehaviour
{
    public GameObject waterDropPrefab;
    public GameObject fireDropPrefab;
    public GameObject heartDropPrefab;
    public GameObject aimDropPrefab;
    [HideInInspector] public GameObject dropPrefab;
    public Transform shootPoint;

    public float baseMinRange = 7f;
    public float baseMaxRange = 10f;
    public float rangeMultiplier = 1f;
    public float shootInterval = 0.1f;
    private float defaultShootInterval;

    public float shootSpeed = 10f;

    public GameManager gameManager;

    bool isShooting = false;
    bool fireMode = false;
    bool heartMode = false;
    bool aimMode = false;

    void Awake()
    {
        defaultShootInterval = shootInterval;
    }

    void Start()
    {
        dropPrefab = waterDropPrefab;
    }

    public void SetRangeMultiplier(float m)
    {
        rangeMultiplier = m;
    }
    public void SetAttackSpeed(float speed)
    {
        shootInterval = speed;
    }
    public void ResetAttackSpeed()
    {
        shootInterval = defaultShootInterval;
    }

    public void ResetRange()
    {
        rangeMultiplier = 1f;
    }

    void Update()
    {
        if (gameManager == null || !gameManager.canShoot)
        {
            isShooting = false;
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (!isShooting)
            {
                isShooting = true;
                StartCoroutine(ShootLoop());
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }
    }

    IEnumerator ShootLoop()
    {
        while (isShooting && gameManager != null && gameManager.canShoot)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void Shoot()
    {
        GameObject drop = Instantiate(dropPrefab, shootPoint.position, Quaternion.identity);
        float randomRange = Random.Range(baseMinRange, baseMaxRange);
        drop.GetComponent<WaterDrop>().Init(transform.up, shootSpeed, randomRange);
    }

    public void ActivateFireDrop()
    {
        fireMode = true;
        dropPrefab = fireDropPrefab;

        if (gameManager != null)
        {
            gameManager.tripleScoreActive = true;
            gameManager.doubleScoreActive = false;
            gameManager.accuracyBonusActive = false;
        }
    }
    public void ActivateHeartDrop()
    {
        heartMode = true;
        dropPrefab = heartDropPrefab;

        if (gameManager != null)
        {
            gameManager.doubleScoreActive = true;
            gameManager.tripleScoreActive = false;
            gameManager.accuracyBonusActive = false;
        }
    }
    public void ActivateAimDrop()
    {
        aimMode = true;
        dropPrefab = aimDropPrefab;

        if (gameManager != null)
        {
            gameManager.accuracyBonusActive = true;
            gameManager.doubleScoreActive = false;
            gameManager.tripleScoreActive = false;
        }
    }
}
