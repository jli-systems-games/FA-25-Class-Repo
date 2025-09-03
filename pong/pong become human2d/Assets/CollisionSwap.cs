using UnityEngine;

public class CollisionSwap : MonoBehaviour
    //为了以防我以后看不懂
    //这个代码的意义就是，因为我想要每个人只能攻击对面的小礼盒
    //所以就是，我想的逻辑就是：当小球球碰到x（左边）或者y（右边）之后
    //才会启用对面物体的碰撞箱
    //所以碰撞之后，己方的碰撞箱是被禁用的
    //这个header是我抄的pongbasic里面的用法。。。问了一下t老师具体，感觉蛮好用的
    //这个我自己没写懂好吧好吧实际上是大T老师给的思路
{
    [Header("要替换的对应的东西")]
    [Tooltip("a,b,c,d,e,f,g,h")]
    public GameObject[] targetsAH = new GameObject[8];   
    [Tooltip("1,2,3,4,5,6,7,8")]
    public GameObject[] replacements18 = new GameObject[8]; 

    [Header("—— 触碰之后触发block ——")]
    [Tooltip("碰到x启用天使侧的碰撞箱")]
    public GameObject xTrigger;
    [Tooltip("碰到 y 启用恶魔侧的碰撞箱")]
    public GameObject yTrigger;

    [Header("仅仅改动碰撞箱的启用与否，不影响物体本身贴图啥的")]
    public bool onlyToggleCollidersOnXY = true;

    private Collider2D[] targetColsA = new Collider2D[4]; // a..d
    private Collider2D[] targetColsB = new Collider2D[4]; // e..h
    private Collider2D xCol, yCol;

    // 这里的逻辑是true => A启用 (abcd)，false => B启用 (efgh)
    private bool groupAEnabled = false;

    void Awake()
    {
        //抓走它们的 Collider
        for (int i = 0; i < 4; i++)
        {
            targetColsA[i] = SafeGetCollider2D(targetsAH[i]);       // a,b,c,d
            targetColsB[i] = SafeGetCollider2D(targetsAH[i + 4]);   // e,f,g,h
        }

        xCol = SafeGetCollider2D(xTrigger);
        yCol = SafeGetCollider2D(yTrigger);

        //初始八个东西全部禁用collider 
        SetGroupColliders(targetColsA, false);
        SetGroupColliders(targetColsB, false);
        groupAEnabled = false; // 最开始都关
    }

    // 这部分 负责启用我的小碰撞箱
    void OnTriggerEnter2D(Collider2D other) { HandleContact(other.gameObject); }
    void OnCollisionEnter2D(Collision2D collision) { HandleContact(collision.gameObject); }

    private void HandleContact(GameObject other)
    {
        if (other == null) return;

        // 肘击对应的东西启用
        if (xTrigger && other == xTrigger) { EnableGroupB_DisableGroupA(); return; }
        if (yTrigger && other == yTrigger) { EnableGroupA_DisableGroupB(); return; }

        //abcdefgh对应了12345678分别启用。为了防止我看不懂12345678是那个擦边的那部分。
        for (int i = 0; i < targetsAH.Length; i++)
        {
            if (targetsAH[i] && other == targetsAH[i])
            {
                targetsAH[i].SetActive(false);
                if (i < replacements18.Length && replacements18[i]) replacements18[i].SetActive(true);
                return;
            }
        }
    }


    private void EnableGroupB_DisableGroupA()
    {
        if (onlyToggleCollidersOnXY)
        {
            SetGroupColliders(targetColsA, false); // abcd 关
            SetGroupColliders(targetColsB, true);  // efgh 开
        }
        else
        {
            SetGroupActive(targetsAH, 0, 4, false); // abcd关
            SetGroupActive(targetsAH, 4, 4, true);  
        }
        groupAEnabled = false;
    }

    private void EnableGroupA_DisableGroupB()
    {
        if (onlyToggleCollidersOnXY)
        {
            SetGroupColliders(targetColsB, false); // efgh 关
            SetGroupColliders(targetColsA, true);  // abcdd 开
        }
        else
        {
            SetGroupActive(targetsAH, 4, 4, false); // efgh关
            SetGroupActive(targetsAH, 0, 4, true);  // abcd 开
        }
        groupAEnabled = true;
    }

    //以下都是自定义函数合计，为了触发那个碰撞的部分，
    //我感觉这部分应该不注释也能看懂所以
    private Collider2D SafeGetCollider2D(GameObject go)
    {
        if (go == null) return null;
        var col = go.GetComponent<Collider2D>();
        if (col == null) col = go.GetComponentInChildren<Collider2D>(true);
        return col;
    }

    private void SetGroupColliders(Collider2D[] group, bool enabled)
    {
        if (group == null) return;
        foreach (var c in group)
        {
            if (c != null) c.enabled = enabled;
        }
    }

    private void SetGroupActive(GameObject[] arr, int start, int count, bool active)
    {
        if (arr == null) return;
        for (int i = 0; i < count; i++)
        {
            int idx = start + i;
            if (idx >= 0 && idx < arr.Length && arr[idx] != null)
            {
                arr[idx].SetActive(active);
            }
        }
    }
}
