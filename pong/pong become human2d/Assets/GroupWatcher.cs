using UnityEngine;

public class GroupWatcher : MonoBehaviour
{
    [Header("恶魔 A (a,b,c,d)")]
    public GameObject[] groupABCD = new GameObject[4];

    [Header("天使 B (e,f,g,h)")]
    public GameObject[] groupEFGH = new GameObject[4];

    [Header("被控制之目标")]
    public GameObject obj1;  //恶魔的战败cg
    public GameObject obj2;  // 记得禁用子弹
    public GameObject obj3;  // 天使的战败cg
    public GameObject obj4;  // 额这个是重启的按钮。实际上我只需要控制button就好了但是我控制整个canvas。。好吧

    void Update()
    {
     //检测有没有全部一起全部被禁用
     //为了防止遥远的未来我看不懂自己为什么这么写。。额因为。。这个启用战胜cg的逻辑实际上
     //是碰撞之后，小礼盒会消失然后
     //小礼盒全部被禁用之后就可以启用战胜cg了
     //但是最开始playtest的时候我发现这个有一个问题就是。。因为子弹没有被禁用的话就会继续撞然后有概率出现一个cg重复现实的问题
     //所以cg被启用之后我还把子弹禁用了如此美丽的屎山代码
        if (AllDisabled(groupABCD))
        {
            if (obj1) obj1.SetActive(true);
            if (obj4) obj4.SetActive(true);   //同样是为了保证我以后看得懂这是干啥的，这是为了启用那个，重启的小按键
            if (obj2) obj2.SetActive(false);
        }

//同理可得
        if (AllDisabled(groupEFGH))
        {
            if (obj3) obj3.SetActive(true);
            if (obj4) obj4.SetActive(true);  
            if (obj2) obj2.SetActive(false);
        }
    }

    bool AllDisabled(GameObject[] group)
    {
        foreach (var go in group)
        {
            if (go != null && go.activeInHierarchy) return false;
        }
        return true;
    }
    //激情检测系统
}
