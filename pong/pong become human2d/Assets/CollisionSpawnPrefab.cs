using UnityEngine;

public class CollisionSpawnPrefab : MonoBehaviour
{
    [Header("����С��� (a~h)")]
    public GameObject[] targetsAH = new GameObject[8];   

    [Header("���ը��")]
    public GameObject hitPrefab;    
    public Transform hitParent;    
    public float prefabLifetime = 0f; //������ط�����������ǣ�����һ����ʱ����һ�����ʧ�����Һ���������Ķ���ϵͳ���������������ʺɽ�����һ���֡������Բ���
    public Vector3 hitOffset = Vector3.zero; 

    private Collider2D selfCol;

    void Awake()
    {
        selfCol = GetComponent<Collider2D>();
    }

    //��������Ϊ�������ײ�����ҵ�С������˸�rigidbodyҪ��Ȼ�ɴ಻�õġ�Ȼ�����b��ʼ�ܹ�����˶��������ø��ˡ�
    void OnCollisionEnter2D(Collision2D collision)
    {
        for (int i = 0; i < targetsAH.Length; i++)
        {
            if (targetsAH[i] && collision.gameObject == targetsAH[i])
            {
                Vector2 pos = collision.contactCount > 0
                    ? collision.GetContact(0).point
                    : collision.transform.position;
                SpawnHit(pos);
                return;
            }
        }
    }
    //��ȷ�汾rigibody�Ұ������֮�����һ�����İ���
    void OnTriggerEnter2D(Collider2D other)
    {
        for (int i = 0; i < targetsAH.Length; i++)
        {
            if (targetsAH[i] && other.gameObject == targetsAH[i])
            {
                Vector2 pos = other.ClosestPoint(transform.position);
                SpawnHit(pos);
                return;
            }
        }
    }

//�ⲿ���ò������ⲿ��ʵ������ȫ�Ǵ�T��ʦ����д�ģ���Ϊ����rigidbody���������ײ������ܲ�����һ��û��rigibody�İ汾��������һ�£����������֪��Ϊɶ�����ò��ˣ�����
//�����ⲿ��ʵ����û�ã������Ҿ���Ҳ���Ǹ�˼·����һ�Ժ�Ҫ�����Ҿ���������
    void Update()
    {
        if (!selfCol) return;

        foreach (var tgt in targetsAH)
        {
            if (tgt == null || !tgt.activeInHierarchy) continue;

            var tgtCol = tgt.GetComponent<Collider2D>();
            if (tgtCol && selfCol.IsTouching(tgtCol))
            {
                Vector2 pos = tgtCol.ClosestPoint(selfCol.bounds.center);
                SpawnHit(pos);
                return;
            }
        }
    }

    //�����ˣ���������
    //����ط������Ϲ��ż������Ҵ�����life�����Ǹ�С����������ʧ�Ĳ��֡��������Ҿ����������ù��������һ���ʹ���������animator����������
    //�ⲿ��������������Ϊ˵�����Ժ����õ�
    private void SpawnHit(Vector2 pos)
    {
        if (!hitPrefab) return;
        var go = Instantiate(hitPrefab, (Vector3)pos + hitOffset, Quaternion.identity, hitParent);
        if (prefabLifetime > 0f)
        {
            Destroy(go, prefabLifetime);
        }
    }
}
