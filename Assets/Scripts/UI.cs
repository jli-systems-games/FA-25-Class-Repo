using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public string targetTag = "NPC";
    public GameObject heartUIPrefab;   // 拖一个 UI Image 做的预制体
    public Canvas canvas;              // 拖场景里的 UI Canvas

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;

        if (heartUIPrefab && canvas)
        {
            // 生成 UI 爱心，直接作为 Canvas 的子物体
            GameObject heart = Instantiate(heartUIPrefab, canvas.transform);
            heart.transform.localPosition = Vector3.zero; // 屏幕正中
        }
    }
}