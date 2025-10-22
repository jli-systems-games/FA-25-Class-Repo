// GrandpaSmileOnContact2D.cs
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class GrandpaSmileOnContact2D : MonoBehaviour
{
    public GameObject smileUI;      // 2초간 켤 UI (기본 비활성)
    public float smileDuration = 2f;

    Coroutine co;

    void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col) col.isTrigger = true; // 얼굴 콜라이더는 트리거
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Sticker>() == null) return; // 스티커만 인정
        if (co != null) StopCoroutine(co);
        co = StartCoroutine(Smile());
    }

    IEnumerator Smile()
    {
        if (smileUI) smileUI.SetActive(true);
        yield return new WaitForSeconds(smileDuration);
        if (smileUI) smileUI.SetActive(false);
        co = null;
    }
}
