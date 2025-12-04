using UnityEngine;
using System.Collections;

public class WaterShooter : MonoBehaviour
{
    public GameObject waterDropPrefab;
    public GameObject fireDropPrefab;
    [HideInInspector] public GameObject dropPrefab;
    public Transform shootPoint;
    public float shootInterval = 0.1f;
    public float minRange = 7f;
    public float maxRange = 10f;
    public float shootSpeed = 10f;

    public GameManager gameManager;

    bool isShooting = false;
    bool fireMode = false;

    void Start()
    {
        dropPrefab = waterDropPrefab;
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
        float randomRange = Random.Range(minRange, maxRange);
        drop.GetComponent<WaterDrop>().Init(transform.up, shootSpeed, randomRange);
    }

    public void ActivateFireDrop()
    {
        fireMode = true;
        dropPrefab = fireDropPrefab;

        if (gameManager != null)
        {
            gameManager.tripleScoreActive = true;
        }
    }
}
