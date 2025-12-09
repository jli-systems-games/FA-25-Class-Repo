using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;

public class RockImpactLogic : MonoBehaviour
{
    [Header("眩晕设置")]
    public float StunDuration = 2.0f;
    public float MinImpactSpeed = 2.0f;
    public float ThrowerProtectionTime = 0.2f;

    private bool _isThrown = false;
    private GameObject _thrower;
    private bool _hasCollided = false;
    private bool _hasBounced = false;
    private float _throwTime;

    // 【修改】增加传入 throwerCollider 参数，用于物理切断
    public void ActivateAttack(GameObject thrower, Collider throwerCollider)
    {
        _isThrown = true;
        _thrower = thrower;
        _hasCollided = false;
        _hasBounced = false;
        _throwTime = Time.time;

        // 【新增】物理层面切断联系：让石头和Player互相忽略碰撞
        Collider rockCollider = GetComponent<Collider>();
        if (rockCollider != null && throwerCollider != null)
        {
            Physics.IgnoreCollision(rockCollider, throwerCollider, true);
            // 开启一个协程，过一会再恢复碰撞（如果你想让反弹回来的石头能砸晕自己的话）
            // 如果你不想石头砸到自己，就不要恢复，或者在这里把 protection time 用上
            StartCoroutine(ResetCollisionDelay(rockCollider, throwerCollider));
        }

        // 5秒兜底销毁
        Invoke(nameof(DeactivateAttack), 5.0f);

        Debug.Log($"🪨 [发射] 石头已激活攻击，投掷者: {(thrower != null ? thrower.name : "null")}");
    }

    // 【新增】保护期过后恢复碰撞（可选，不需要可以删掉）
    IEnumerator ResetCollisionDelay(Collider rock, Collider player)
    {
        yield return new WaitForSeconds(ThrowerProtectionTime);
        if (rock != null && player != null)
        {
            Physics.IgnoreCollision(rock, player, false);
        }
    }

    void DeactivateAttack()
    {
        _isThrown = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown || _hasCollided) return;

        // 虽然有物理忽略，保留这个判断作为双重保险
        if (collision.gameObject == _thrower && !_hasBounced) return;

        // --- 以下是你原来的逻辑，完全没动 ---
        float impactSpeed = 0f;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) impactSpeed = rb.linearVelocity.magnitude;
        else impactSpeed = collision.relativeVelocity.magnitude;

        if (impactSpeed < MinImpactSpeed) return;

        Character hitCharacter = collision.gameObject.GetComponent<Character>();

        if (hitCharacter != null &&
            hitCharacter.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
        {
            _hasCollided = true;
            Debug.Log($"🪨 [命中] 石头砸中了 {hitCharacter.name}！(速度: {impactSpeed:F1})");

            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            var freezeCtrl = hitCharacter.GetComponent<PlayerFreezeController>();
            if (freezeCtrl != null) freezeCtrl.ApplyFreeze(StunDuration);
            else
            {
                hitCharacter.Freeze();
                StartCoroutine(UnFreezeDelay(hitCharacter, StunDuration));
            }

            CharacterHandleWeapon handleWeapon = hitCharacter.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon != null) handleWeapon.ChangeWeapon(null, "Weapon1");

            StartCoroutine(SafeDestroyRoutine());
        }
        else
        {
            _hasBounced = true;
            Debug.Log($"🪨 [反弹] 撞到: {collision.gameObject.name}");
        }
    }

    IEnumerator SafeDestroyRoutine()
    {
        Collider[] cols = GetComponentsInChildren<Collider>(true);
        foreach (var c in cols) if (c != null) c.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
        }

        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers) if (r != null) r.enabled = false;

        yield return new WaitForSeconds(0.1f);
        if (gameObject != null) Destroy(gameObject);
    }

    IEnumerator UnFreezeDelay(Character c, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (c != null) c.UnFreeze();
    }
}