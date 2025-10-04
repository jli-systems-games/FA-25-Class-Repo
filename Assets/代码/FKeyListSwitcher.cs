using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class FKeyListSwitcher : MonoBehaviour
{
    [Header("物体列表")]
    public List<GameObject> list1;
    public List<GameObject> list2;
    public List<GameObject> list3;
    public List<GameObject> list4;   // 新增：等待结束后启用的 list4

    [Header("条件物体")]
    public GameObject objectA;   // 用于判断当下是否启用

    [Header("视频重置")]
    public VideoPlayer videoPlayer;
    public bool autoPlayAfterReset = false; // 重置后是否自动播放

    [Header("按键与时长")]
    public KeyCode triggerKey = KeyCode.F;
    public float delayWhenAInactive = 4f;   // A 未启用时等待 3 秒
    public float delayWhenAActive = 7f;     // A 启用时等待 6 秒

    private bool isRunning = false; // 锁，避免重复触发

    void Update()
    {
        if (Input.GetKeyDown(triggerKey) && !isRunning)
        {
            ResetVideo();
            StartCoroutine(SwitchRoutine());
        }
    }

    IEnumerator SwitchRoutine()
    {
        isRunning = true;

        bool aIsOn = (objectA != null && objectA.activeInHierarchy && objectA.activeSelf);

        if (!aIsOn)
        {
            // A 未启用 → 启用 list1，禁用 list2
            SetActiveList(list1, true);
            SetActiveList(list2, false);

            yield return new WaitForSeconds(delayWhenAInactive);

            // 启用 list4，禁用 list1
            SetActiveList(list4, true);
            SetActiveList(list1, false);
        }
        else
        {
            // A 启用中 → 启用 list3，禁用 list2
            SetActiveList(list3, true);
            SetActiveList(list2, false);

            yield return new WaitForSeconds(delayWhenAActive);

            // 启用 list4，禁用 list3
            SetActiveList(list4, true);
            SetActiveList(list3, false);
        }

        isRunning = false;
    }

    void ResetVideo()
    {
        if (videoPlayer == null) return;

        videoPlayer.Stop();
        videoPlayer.time = 0;
        if (autoPlayAfterReset)
            videoPlayer.Play();
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
