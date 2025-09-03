using UnityEngine;

public class CollisionSpawnPrefab : MonoBehaviour
{
    [Header("奇妙小礼盒 (a~h)")]
    public GameObject[] targetsAH = new GameObject[8];   

    [Header("奇妙爆炸了")]
    public GameObject hitPrefab;    
    public Transform hitParent;    
    public float prefabLifetime = 0f; //对这个地方我曾经想的是，，有一个计时程序一会会消失但是我后面用奇妙的动画系统解决了所以这里是屎山代码的一部分。。。对不起
    public Vector3 hitOffset = Vector3.zero; 

    private Collider2D selfCol;

    void Awake()
    {
        selfCol = GetComponent<Collider2D>();
    }

    //哈哈特意为了这个碰撞检测给我的小球球加了个rigidbody要不然干脆不用的。然后这个b球开始很诡异的运动不过懒得改了。
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
    //正确版本rigibody我爱你加你之后简单了一倍爱心爱心
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

//这部分用不到哈这部分实际上完全是大T老师教我写的，因为用了rigidbody会奇妙的乱撞我想过能不能用一个没有rigibody的版本所以问了一下，但是这个不知道为啥反正用不了，，，
//所以这部分实际上没用，但是我觉得也算是个思路吧万一以后要用呢我就留下来了
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

    //生成了，给力给力
    //这个地方还有上古遗迹就是我打算用life控制那个小东西出现消失的部分。。但是我觉得这个玩意好诡异所以我还是使用了我最爱的animator（给力给力
    //这部分我留下来是因为说不定以后还能用到
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
