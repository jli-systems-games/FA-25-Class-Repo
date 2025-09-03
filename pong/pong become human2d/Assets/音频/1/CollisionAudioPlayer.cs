using UnityEngine;

public class CollisionAudioPlayer : MonoBehaviour
{
    [Header("恶魔碰撞 A (a, b, c, d)")]
    public GameObject[] groupA;

    [Header("天使碰撞 B (e, f, g, h)")]
    public GameObject[] groupB;

    [Header("恶魔银叫 A (1, 2, 3)")]
    public AudioClip[] audioGroupA;

    [Header("天使银叫 B (4, 5, 6)")]
    public AudioClip[] audioGroupB;

    private AudioSource audioSource;
    private bool[] groupAStates;
    private bool[] groupBStates;


    //如果有人在看这个，我滑跪。我只是一个可怜的美术原画姐，我真的不会写代码。
    //这个代码是在gpt老师的指导下写的。我能做的就是理解每一行的意义并且拼接一些我自己的idea。
    //比较高级的部分都不是我写的。我从b站里找实现过的人抄的。虽然我能看懂但是我绝对写不出来呵呵

    // If anyone is reading this, I’m sorry.  
    // I’m just a concept artist. I really don’t know how to code.  
    // This script was written under the guidance of GPT.  
    // The best I can do is understand what each line means and piece together some of my own ideas.  
    // The more advanced parts definitely aren’t written by me.  
    // I copied them from people who had already implemented it on Bilibili.  
    // I can understand it, but I could never write it myself, haha.

    void Start()
    {
        //给每个物体（没有的话）生成一个奇妙的audio播放器
    //因为我本来是没有个物体生成播放器的，实际上我最开始的思路是碰撞之后播放但是这个好像有点不行
    //所以现在的状态是物品消失再播放
    //所以这部分到底还要不要我存疑但是我不太敢删，，，，，，，
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

    
        groupAStates = new bool[groupA.Length];
        for (int i = 0; i < groupA.Length; i++)
        {
            if (groupA[i] != null) groupAStates[i] = groupA[i].activeSelf;
        }

        groupBStates = new bool[groupB.Length];
        for (int i = 0; i < groupB.Length; i++)
        {
            if (groupB[i] != null) groupBStates[i] = groupB[i].activeSelf;
        }
    }

    void Update()
    {
        //捕捉 GameObject 从启用 → 禁用 的瞬间如果禁用就启用音效
        //我原来的代码里是没有检查这个 if (groupA[i] == null) continue;的这部分问了gpt说可以加上防止报错？！
        //groupAStates[i] = groupA[i].activeSelf; 这部分是因为之前会莫名重复触发音效所以加上了
        //作用就是检查有没有开启过防止二次搞出问题赞赞赞

        for (int i = 0; i < groupA.Length; i++)
        {
            if (groupA[i] == null) continue;

            if (groupAStates[i] && !groupA[i].activeSelf) 
            {
                PlayRandomAudio(audioGroupA);
            }

            groupAStates[i] = groupA[i].activeSelf; 
        }

       
        for (int i = 0; i < groupB.Length; i++)
        {
            if (groupB[i] == null) continue;

            if (groupBStates[i] && !groupB[i].activeSelf) 
            {
                PlayRandomAudio(audioGroupB);
            }

            groupBStates[i] = groupB[i].activeSelf; 
        }
    }

    void PlayRandomAudio(AudioClip[] clips)
    {
        if (clips == null || clips.Length == 0) return;

        int index = Random.Range(0, clips.Length);
        audioSource.clip = clips[index];
        audioSource.Play();
        //纯粹的自定义函数。。。。
        // if (clips == null || clips.Length == 0) return;也是gpt debug的时候加的
        //我自己不是很能养成写防止报错的代码的习惯，，，，
        //就是随机一个数值并播放对应的音频。这个地方本来写的是length只有3，但是因为改成了数组（叫这个吗），就改成了clips.Length
        //什么时候才能有...就完善代码的意识。。。
    }
}