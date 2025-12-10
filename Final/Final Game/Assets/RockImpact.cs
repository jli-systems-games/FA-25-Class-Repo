using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;

public class RockImpactLogic : MonoBehaviour
{
    [Header("眩晕设置")]
    public float StunDuration = 2.0f; // 改为2秒
    public float MinImpactSpeed = 2.0f;
    public float ThrowerProtectionTime = 0.2f; // 发射后0.2秒内不检测投掷者

    private bool _isThrown = false;
    private GameObject _thrower;
    private bool _hasCollided = false;
    private bool _hasBounced = false;
    private float _throwTime; // 记录发射时间

    public void ActivateAttack(GameObject thrower)
    {
        _isThrown = true;
        _thrower = thrower;
        _hasCollided = false;
        _hasBounced = false;
        _throwTime = Time.time; // 记录发射时间

        // 5秒兜底销毁（延长一点）
        Invoke(nameof(DeactivateAttack), 5.0f);

        Debug.Log($"🪨 [发射] 石头已激活攻击，投掷者: {(thrower != null ? thrower.name : "null")}");
    }

    void DeactivateAttack()
    {
        _isThrown = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        // 1. 基础检查
        if (!_isThrown || _hasCollided) return;

        // 2. 投掷者保护：发射后0.2秒内 + 未反弹
        if (collision.gameObject == _thrower && !_hasBounced)
        {
            float timeSinceThrow = Time.time - _throwTime;
            if (timeSinceThrow < ThrowerProtectionTime)
            {
                Debug.Log($"🪨 [保护] 忽略投掷者碰撞（时间: {timeSinceThrow:F2}s）");
                return;
            }
        }

        // 3. 速度检查
        float impactSpeed = 0f;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
#if UNITY_6000_0_OR_NEWER
            impactSpeed = rb.linearVelocity.magnitude;
#else
            impactSpeed = rb.velocity.magnitude;
#endif
        }
        else
        {
            impactSpeed = collision.relativeVelocity.magnitude;
        }

        if (impactSpeed < MinImpactSpeed)
        {
            Debug.Log($"🪨 [速度不足] 速度: {impactSpeed:F1} < {MinImpactSpeed}");
            return;
        }

        // 4. 检测撞到的是不是角色
        Character hitCharacter = collision.gameObject.GetComponent<Character>();

        if (hitCharacter != null &&
            hitCharacter.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
        {
            // 撞到活人
            _hasCollided = true;

            Debug.Log($"🪨 [命中] 石头砸中了 {hitCharacter.name}！(速度: {impactSpeed:F1}){(collision.gameObject == _thrower ? " <反弹自伤>" : "")}");

            // 【关键】立即停止石头运动
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.isKinematic = true; // 立即设为kinematic，防止被推动
            }

            // 眩晕逻辑
            var freezeCtrl = hitCharacter.GetComponent<PlayerFreezeController>();
            if (freezeCtrl != null)
            {
                freezeCtrl.ApplyFreeze(StunDuration);
                Debug.Log($"🪨 [眩晕] 使用PlayerFreezeController，时长: {StunDuration}s");
            }
            else
            {
                hitCharacter.Freeze();
                StartCoroutine(UnFreezeDelay(hitCharacter, StunDuration));
                Debug.Log($"🪨 [眩晕] 使用Character.Freeze，时长: {StunDuration}s");
            }

            // 掉落武器逻辑
            CharacterHandleWeapon handleWeapon = hitCharacter.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon != null)
            {
                handleWeapon.ChangeWeapon(null, "Weapon1");
                Debug.Log($"🪨 [缴械] {hitCharacter.name} 掉落武器");
            }

            // 销毁石头
            StartCoroutine(SafeDestroyRoutine());
        }
        else
        {
            // 撞到墙壁或障碍物
            _hasBounced = true;
            Debug.Log($"🪨 [反弹] 撞到: {collision.gameObject.name}（已标记为反弹状态）");
        }
    }

    IEnumerator SafeDestroyRoutine()
    {
        Debug.Log("🪨 [销毁] 开始销毁石头");

        // 禁用碰撞器
        Collider[] cols = GetComponentsInChildren<Collider>(true);
        foreach (var c in cols)
        {
            if (c != null) c.enabled = false;
        }

        // 停止物理
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 隐藏渲染
        Renderer[] renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (r != null) r.enabled = false;
        }

        yield return new WaitForSeconds(0.1f);

        if (gameObject != null)
        {
            Destroy(gameObject);
            Debug.Log("🪨 [销毁] 石头已销毁");
        }
    }

    IEnumerator UnFreezeDelay(Character c, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (c != null)
        {
            c.UnFreeze();
            Debug.Log($"🪨 [解冻] {c.name} 已解冻");
        }
    }
}