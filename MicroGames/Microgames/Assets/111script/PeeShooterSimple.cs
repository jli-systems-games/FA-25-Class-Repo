using UnityEngine;

public class PeeShooterSimple : MonoBehaviour
{
    [Header("Refs")]
    public DrunkAim aimer;         // �������Ǹ� DrunkAim
    public Transform muzzle;       // ƿ��
    public ParticleSystem peeFX;   // ����
    public GameObject probePrefab; // Probe.prefab��Rigidbody2D + Trigger��

    [Header("Fire")]
    public float launchSpeed = 12f;
    public float probesPerSecond = 22f;
    public float urinePerProbe = 1f;

    float cd;

    void Update()
    {
        bool allow = (GameManager.I == null) || GameManager.I.CanShoot();

        if (peeFX && aimer && muzzle)
        {
            Vector3 dir = (aimer.AimPoint - muzzle.position);
            if (dir.sqrMagnitude > 1e-6f)
                peeFX.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);

            if (allow && !peeFX.isPlaying) peeFX.Play();
            if (!allow && peeFX.isPlaying) peeFX.Stop();
        }

        if (!allow) return;

        cd -= Time.deltaTime;
        if (cd <= 0f)
        {
            cd = 1f / Mathf.Max(1f, probesPerSecond);
            FireOne();
        }
    }

    void FireOne()
    {
        if (!aimer || !muzzle || !probePrefab) return;

        Vector3 dir3 = (aimer.AimPoint - muzzle.position);
        if (dir3.sqrMagnitude < 1e-6f) return;
        Vector2 v = new Vector2(dir3.x, dir3.y).normalized * launchSpeed;

        var go = Instantiate(probePrefab, muzzle.position, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody2D>();
        if (rb)
        {
            rb.linearVelocity = v;
            rb.gravityScale = 0f;     // ֱ�ߣ�����׹
            rb.linearDamping = 0f; rb.angularDamping = 0f;
        }

        GameManager.I?.RegisterShot();
        GameManager.I?.ConsumeUrine(urinePerProbe);
    }
}
