using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class RandomSwitcher : MonoBehaviour
{
    [Header("物体列表")]
    public List<GameObject> list1;
    public List<GameObject> list2;
    public List<GameObject> list3;

    [Header("视频控制")]
    public GameObject objectA;      // A：启用/禁用的物体（UI等）
    public VideoPlayer videoPlayer; // 播放器本体（输出到 RawImage 的 RenderTexture）

    [Header("时间设置")]
    public Vector2 waitRange = new Vector2(7f, 12f); // list1 等待区间
    public float listActiveTime = 5f;                // list2/3 启用总时长
    public float pulseTime = 3f;                     // A 每次播放的时长

    private void Start()
    {
        // 初始化一次状态（只在开局运行）
        SetActiveList(list1, false);
        SetActiveList(list1, true);

        SetActiveList(list2, false);
        SetActiveList(list3, false);

        if (objectA) objectA.SetActive(false);
        if (videoPlayer)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
    }

    // 每次启用时重新启动协程
    private void OnEnable()
    {
        StartCoroutine(SwitchRoutine());
    }

    // 禁用时停止所有协程，防止残留
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    IEnumerator SwitchRoutine()
    {
        while (true)
        {
            // 在 list1 状态随机等
            yield return new WaitForSeconds(Random.Range(waitRange.x, waitRange.y));

            // 切到 list2 或 list3
            SetActiveList(list1, false);
            List<GameObject> chosenList = (Random.value <= 0.4f) ? list2 : list3;
            SetActiveList(chosenList, true);

            // —— 边沿1：启用时脉冲一次
            yield return StartCoroutine(PlayPulseOnce());

            // 在本阶段剩余时间里等待到“禁用时刻”
            float remain = Mathf.Max(0f, listActiveTime - pulseTime);
            yield return new WaitForSeconds(remain);

            // 准备禁用 chosenList
            SetActiveList(chosenList, false);

            // —— 边沿2：禁用时再脉冲一次
            yield return StartCoroutine(PlayPulseOnce());

            // 回到 list1
            SetActiveList(list1, true);
        }
    }

    // 一次脉冲：A开+视频从0播pulseTime秒 → 关 + 重置
    IEnumerator PlayPulseOnce()
    {
        if (objectA) objectA.SetActive(true);

        if (videoPlayer)
        {
            videoPlayer.time = 0;
            videoPlayer.Play();
        }

        yield return new WaitForSeconds(pulseTime);

        if (objectA) objectA.SetActive(false);

        if (videoPlayer)
        {
            videoPlayer.Stop();
            videoPlayer.time = 0;
        }
    }

    void SetActiveList(List<GameObject> list, bool state)
    {
        if (list == null) return;
        foreach (var go in list)
        {
            if (go) go.SetActive(state);
        }
    }
}
