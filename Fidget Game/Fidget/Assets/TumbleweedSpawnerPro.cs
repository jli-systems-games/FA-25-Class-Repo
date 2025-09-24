using UnityEngine;

public class TumbleweedSpawnerPro : MonoBehaviour
{
    [Header("Refs")]
    public Camera cam;
    public GameObject prefab;

    [Header("Spawn Area (in front of camera)")]
    public float spawnDistance = 30f;      // �������Լ30m
    public float lateralSpread = 10f;      // ����ɢ��
    [Range(0, 80)] public float halfFovDeg = 25f; // ǰ�����ΰ��
    public float verticalDrop = 12f;       // �ӿ����������
    public float maxGroundSnap = 40f;
    public LayerMask groundMask;

    [Header("Timing")]
    public float interval = 8f;            // ÿ8��ˢ1��
    float nextTime;

    [Header("Motion")]
    public float minSpeed = 4f, maxSpeed = 7f;
    public float minTorque = 8f, maxTorque = 14f;
    public float lifeTime = 25f;

    public enum DirectionBias { RandomInCone, TowardsView }
    [Header("Smart Direction")]
    public DirectionBias directionBias = DirectionBias.TowardsView; // ?Ĭ�ϳ�����
    [Range(0, 1)] public float viewBiasStrength = 0.7f;  // 0=��ƫ�ã�1=ǿ�ҳ�����

    [Header("Avoid Car Head-on")]
    public Transform car;                  // ���������ɲ�����ñ����壩
    [Range(0, 80)] public float avoidConeDeg = 18f; // ���⡰ֱָ����/������ļн�
    public float avoidTurnDeg = 25f;       // ���б���׶ʱ�����ƫת�ĽǶ�

    void Reset()
    {
        cam = Camera.main;
        int g = LayerMask.NameToLayer("ground");
        if (g >= 0) groundMask = 1 << g;
    }

    void Update()
    {
        if (!cam || !prefab) return;
        if (Time.time >= nextTime)
        {
            nextTime = Time.time + interval;
            SpawnOne();
        }
    }

    void SpawnOne()
    {
        // ���� 1) ȡǰ�������ڵ�һ���������� ����
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 rgt = cam.transform.right; rgt.y = 0; rgt.Normalize();

        float ang = Random.Range(-halfFovDeg, halfFovDeg) * Mathf.Deg2Rad;
        Vector3 coneDir = (fwd * Mathf.Cos(ang) + rgt * Mathf.Sin(ang)).normalized;

        // ���� 2) �������ɵ㣨ǰ���̶����� + ����ɢ����������� ����
        float side = Random.Range(-lateralSpread, lateralSpread);
        Vector3 pos = cam.transform.position + coneDir * spawnDistance + rgt * side;

        Vector3 castStart = pos + Vector3.up * verticalDrop;
        if (Physics.Raycast(castStart, Vector3.down, out var hit, maxGroundSnap, groundMask, QueryTriggerInteraction.Ignore))
            pos = hit.point + Vector3.up * 0.05f;
        else
            pos = castStart + Vector3.down * (verticalDrop * 0.5f);

        // ���� 3) ʵ���� ����
        var go = Instantiate(prefab, pos, Quaternion.identity);
        var rb = go.GetComponent<Rigidbody>();

        // ���� 4) ������������ ������������ + ���ã�
        Vector3 viewDir = (cam.transform.forward); viewDir.y = 0; viewDir.Normalize();
        Vector3 moveDir;

        if (directionBias == DirectionBias.TowardsView)
        {
            // �� coneDir �� viewDir ֮���ֵ��ƫ������
            moveDir = Vector3.Slerp(coneDir, viewDir, Mathf.Clamp01(viewBiasStrength)).normalized;
        }
        else
        {
            moveDir = coneDir;
        }

        // �ܳ�ֱײ�����ƶ����򼸺�ֱָ������������ƫת
        Vector3 toCam = (cam.transform.position - pos); toCam.y = 0; toCam.Normalize();
        float cosA = Vector3.Dot(moveDir, toCam); // Խ�ӽ�1Խֱ�����
        float threshold = Mathf.Cos(avoidConeDeg * Mathf.Deg2Rad);
        if (cosA > threshold)
        {
            // ѡ��һ����ƫ��/ƫ�ҡ��������
            float sideSign = (Random.value < 0.5f ? -1f : 1f);
            moveDir = Quaternion.Euler(0, sideSign * avoidTurnDeg, 0) * moveDir;
        }

        // ���� 5) �����ٶ�/Ť�أ����˻�Ϊ���� mover������
        float speed = Random.Range(minSpeed, maxSpeed);

        if (rb)
        {
            rb.linearVelocity = moveDir.normalized * speed + new Vector3(0, rb.linearVelocity.y, 0);
            Vector3 torque = Vector3.Cross(Vector3.up, moveDir).normalized * Random.Range(minTorque, maxTorque);
            rb.AddTorque(torque, ForceMode.VelocityChange);
        }
        else
        {
            go.AddComponent<TumbleweedMover>().Init(moveDir.normalized * speed, lifeTime);
        }

        Destroy(go, lifeTime);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!cam) return;
        Gizmos.color = new Color(0.3f, 1f, 0.6f, 0.7f);
        Vector3 fwd = cam.transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 p = cam.transform.position;
        Vector3 c1 = Quaternion.Euler(0, halfFovDeg, 0) * fwd;
        Vector3 c2 = Quaternion.Euler(0, -halfFovDeg, 0) * fwd;
        Gizmos.DrawLine(p, p + c1 * spawnDistance);
        Gizmos.DrawLine(p, p + c2 * spawnDistance);
    }
#endif
}

public class TumbleweedMover : MonoBehaviour
{
    Vector3 v; float lifeLeft;
    public void Init(Vector3 vel, float life) { v = vel; lifeLeft = life; }
    void Update() { transform.position += v * Time.deltaTime; lifeLeft -= Time.deltaTime; if (lifeLeft <= 0) Destroy(gameObject); }
}
