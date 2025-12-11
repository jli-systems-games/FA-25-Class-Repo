using UnityEngine;
using MoreMountains.TopDownEngine;
using MoreMountains.Tools;
using System.Collections;
using System.Collections.Generic;

public class GrenadeBullet : MonoBehaviour
{
    [Header("爆炸范围设置")]
    public float ExplosionRadius = 5f;
    public LayerMask PlayerLayer;

    [Header("击退设置")]
    public float KnockbackForce = 20f;

    [Header("减速设置")]
    public float SlowDuration = 6f;
    [Range(0.1f, 1f)]
    public float SlowFactor = 0.3f;

    [Header("延迟设置")]
    public float ExplosionDelay = 2f;  // 👈 和武器设置保持一致

    [Header("调试")]
    public bool ShowGizmos = true;

    private DamageOnTouch _damageOnTouch;
    private bool _hasExploded = false;

    void Awake()
    {
        _damageOnTouch = GetComponent<DamageOnTouch>();
    }

    void OnEnable()
    {
        _hasExploded = false;
        // 👇 子弹启用后立即开始倒计时
        StartCoroutine(ExplosionTimer());
    }

    IEnumerator ExplosionTimer()
    {
        // 等待延迟时间
        yield return new WaitForSeconds(ExplosionDelay);

        // 时间到，立即触发爆炸效果
        Debug.Log($"⏰ 爆炸倒计时结束，触发爆炸！时间: {Time.time}");
        TriggerExplosion();
    }

    void TriggerExplosion()
    {
        if (_hasExploded) return;
        _hasExploded = true;

        Vector3 explosionCenter = transform.position;

        Debug.Log($"💣 榴弹爆炸! 位置: {explosionCenter}, 时间: {Time.time}");

        Collider[] hitColliders = Physics.OverlapSphere(explosionCenter, ExplosionRadius, PlayerLayer);
        List<Character> affectedCharacters = new List<Character>();

        foreach (Collider hit in hitColliders)
        {
            if (_damageOnTouch != null && _damageOnTouch.Owner != null)
            {
                if (hit.gameObject == _damageOnTouch.Owner ||
                    hit.transform.IsChildOf(_damageOnTouch.Owner.transform))
                {
                    continue;
                }
            }

            Character character = hit.GetComponent<Character>();
            if (character == null)
            {
                character = hit.GetComponentInParent<Character>();
            }

            if (character != null && !affectedCharacters.Contains(character))
            {
                affectedCharacters.Add(character);
                ApplyExplosionEffect(character, explosionCenter);
            }
        }

        Debug.Log($"💥 榴弹影响了 {affectedCharacters.Count} 个角色, 时间: {Time.time}");
    }

    void ApplyExplosionEffect(Character character, Vector3 explosionCenter)
    {
        var explosionController = character.GetComponent<PlayerExplosionController>();
        if (explosionController == null)
        {
            Debug.LogWarning($"⚠️ {character.name} 没有 PlayerExplosionController！");
            return;
        }

        Vector3 direction = (character.transform.position - explosionCenter).normalized;
        explosionController.ApplyExplosion(direction, KnockbackForce, SlowDuration, SlowFactor);

        Debug.Log($"💥 {character.name} 被炸飞！时间: {Time.time}");
    }

    void OnDrawGizmosSelected()
    {
        if (ShowGizmos)
        {
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.3f);
            Gizmos.DrawSphere(transform.position, ExplosionRadius);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, ExplosionRadius);
        }
    }
}