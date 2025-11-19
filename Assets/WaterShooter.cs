using UnityEngine;
using System.Collections;

public class WaterShooter : MonoBehaviour
{
    public GameObject dropPrefab;
    public Transform shootPoint;
    public float shootInterval = 0.1f;
    public float minRange = 3f;
    public float maxRange = 5f;
    public float shootSpeed = 10f;

    bool isShooting = false;

    void Update()
    {
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
        while (isShooting)
        {
            Shoot();
            yield return new WaitForSeconds(shootInterval);
        }
    }

    void Shoot()
    {
        GameObject drop = Instantiate(dropPrefab, shootPoint.position, Quaternion.identity);

        float randomRange = Random.Range(minRange, maxRange);

        drop.GetComponent<WaterDrop>().Init(shootPoint.up, shootSpeed, randomRange);
    }
}
