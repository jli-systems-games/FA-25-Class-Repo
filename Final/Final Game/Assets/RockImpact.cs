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

    // 【修改】参数接收 Collider[] 数组，彻底解决自身反弹问题
    public void ActivateAttack(GameObject thrower, Collider[] throwerColliders)
    {
        _isThrown = true;
        _thrower = thrower;
        _hasCollided = false;
        _hasBounced = false;
        _throwTime = Time.time;

        Collider rockCollider = GetComponent<Collider>();

        // 【关键】遍历玩家身上所有碰撞体（身体、手、头等），全部忽略
        if (rockCollider != null && throwerColliders != null)
        {
            foreach (var col in throwerColliders)
            {
                // 只忽略启用的碰撞体
                if (col != null && col.enabled && col.gameObject.activeInHierarchy)
                {
                    Physics.IgnoreCollision(rockCollider, col, true);
                    // 开启协程，保护期后恢复碰撞（可选）
                    StartCoroutine(ResetCollisionDelay(rockCollider, col));
                }
            }
        }

        // 5秒后自动销毁
        Invoke(nameof(DeactivateAttack), 5.0f);

        Debug.Log($"🪨 [发射] 石头已激活，忽略了玩家身上的 {throwerColliders.Length} 个碰撞体");
    }

    IEnumerator ResetCollisionDelay(Collider rock, Collider playerPart)
    {
        yield return new WaitForSeconds(ThrowerProtectionTime);
        // 如果对象还在，恢复碰撞检测
        if (rock != null && playerPart != null)
        {
            Physics.IgnoreCollision(rock, playerPart, false);
        }
    }

    void DeactivateAttack()
    {
        _isThrown = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown || _hasCollided) return;

        // 双重保险：虽然有物理忽略，代码层再防一次投掷者
        if (collision.gameObject == _thrower && !_hasBounced) return;

        // 速度检查
        float impactSpeed = 0f;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) impactSpeed = rb.linearVelocity.magnitude;
        else impactSpeed = collision.relativeVelocity.magnitude;

        // 【优化】对于撞墙的情况，RockBounceController 会处理，这里主要处理撞人
        // 尝试获取 Character 组件（包括父物体，防止撞到子碰撞体识别不到）
        Character hitCharacter = collision.gameObject.GetComponent<Character>();
        if (hitCharacter == null) hitCharacter = collision.gameObject.GetComponentInParent<Character>();

        if (hitCharacter != null &&
            hitCharacter.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
        {
            // --- 撞到活人 ---
            _hasCollided = true;
            Debug.Log($"🪨 [命中] 石头砸中了 {hitCharacter.name}！");

            // 立即停止
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true;
            }

            // 眩晕处理
            var freezeCtrl = hitCharacter.GetComponent<PlayerFreezeController>();
            if (freezeCtrl != null) freezeCtrl.ApplyFreeze(StunDuration);
            else
            {
                hitCharacter.Freeze();
                StartCoroutine(UnFreezeDelay(hitCharacter, StunDuration));
            }

            // 缴械处理
            CharacterHandleWeapon handleWeapon = hitCharacter.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon != null) handleWeapon.ChangeWeapon(null, "Weapon1");

            // 销毁石头
            StartCoroutine(SafeDestroyRoutine());
        }
        else
        {
            // --- 撞墙 ---
            // 标记为已反弹，允许后续砸中自己
            _hasBounced = true;
            // 注意：不要在这里处理反弹物理，交给 RockBounceController
        }
    }

    IEnumerator SafeDestroyRoutine()
    {
        // 禁用碰撞和渲染，制造“消失”的假象，然后销毁
        Collider[] cols = GetComponentsInChildren<Collider>(true);
        foreach (var c in cols) if (c != null) c.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.linearVelocity = Vector3.zero; }

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