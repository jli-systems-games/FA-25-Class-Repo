using UnityEngine;
using System.Collections;

public class Loveaudio : MonoBehaviour
{
    [Header("目标识别")]
    public string targetTag = "NPC";

    [Header("爱心对象(场景里已有，默认禁用)")]
    public GameObject heart;
    public float xRotation = 25f;
    public Vector3 offset = new Vector3(0f, 1.4f, 0f);

    [Header("显示时长范围 (秒)")]
    public float minShowTime = 0.5f;
    public float maxShowTime = 1.5f;
    public float cooldown = 0.4f;

    [Header("音效")]
    public AudioClip loveSfx;
    [Range(0f,1f)] public float volume = 0.9f;

    float lastTime;
    Coroutine hideCo;

    void Start()
    {
        if (heart != null)
        {
            heart.SetActive(false);
            if (heart.transform.parent != transform)
                heart.transform.SetParent(transform, worldPositionStays: false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(targetTag)) return;
        if (Time.time - lastTime < cooldown) return;
        lastTime = Time.time;

        if (heart != null)
        {
            heart.SetActive(true);
            heart.transform.localPosition = offset;
            heart.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            float a = minShowTime, b = maxShowTime;
            if (b < a) { float tmp = a; a = b; b = tmp; }
            float duration = Random.Range(a, b);

            if (Hud.I != null)
                Hud.I.RegisterKiss(duration);

            if (hideCo != null) StopCoroutine(hideCo);
            hideCo = StartCoroutine(HideAfter(duration));
        }

        if (loveSfx) AudioSource.PlayClipAtPoint(loveSfx, transform.position, volume);
    }

    IEnumerator HideAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (heart) heart.SetActive(false);
        hideCo = null;
    }
}