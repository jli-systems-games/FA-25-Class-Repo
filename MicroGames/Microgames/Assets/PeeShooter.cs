using UnityEngine;

public class PeeShooter : MonoBehaviour
{
    public Transform muzzle;           // �� Muzzle������ڣ�
    public ParticleSystem peeFX;       // �� peeFX�����ӣ�
    public GameObject probePrefab;     // �� Probe.prefab���������õģ�

    [Header("Launch")]
    public float launchSpeed = 10f;
    public float probesPerSecond = 20f;

    float cd;

    void Update()
    {
        // ���ӳ����� GunPivot һ�²���������
        if (peeFX)
        {
            peeFX.transform.rotation = transform.rotation;
            if (!peeFX.isPlaying) peeFX.Play();
        }

        // �̶�Ƶ�ʷ��䡰�ж����衱�����ɼ�С��
        cd -= Time.deltaTime;
        if (cd <= 0f)
        {
            cd = 1f / probesPerSecond;
            SpawnProbe();
        }
    }

    void SpawnProbe()
    {
        if (!probePrefab || !muzzle) return;

        var go = Instantiate(probePrefab, muzzle.position, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody2D>();

        // �� Muzzle �ı��� +X�����ᣩ������
        Vector2 dir = muzzle.right;
        rb.linearVelocity = dir.normalized * launchSpeed;

        GameManager.I?.RegisterShot();
    }
}
