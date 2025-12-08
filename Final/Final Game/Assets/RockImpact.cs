using UnityEngine;
using MoreMountains.TopDownEngine;
using System.Collections;

public class RockImpactLogic : MonoBehaviour
{
    [Header("眩晕设置")]
    public float StunDuration = 1.0f;
    public float MinImpactSpeed = 2.0f;

    private bool _isThrown = false;
    private GameObject _thrower;
    private bool _hasCollided = false; // 防止多次碰撞

    public void ActivateAttack(GameObject thrower)
    {
        _isThrown = true;
        _thrower = thrower;
        Invoke("DeactivateAttack", 3.0f);
    }

    void DeactivateAttack()
    {
        _isThrown = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!_isThrown || collision.gameObject == _thrower || _hasCollided) return;

        // [安全优化] 碰撞速度检测
        float impactSpeed = collision.relativeVelocity.magnitude;
        if (impactSpeed < MinImpactSpeed) return;

        _hasCollided = true; // 标记已碰撞，防止后续逻辑重复执行

        Character hitCharacter = collision.gameObject.GetComponent<Character>();

        if (hitCharacter != null && hitCharacter.ConditionState.CurrentState != CharacterStates.CharacterConditions.Dead)
        {
            Debug.Log($"🪨 [命中] 石头砸中了 {hitCharacter.name}！(速度: {impactSpeed:F1})");

            // 眩晕逻辑
            var freezeCtrl = hitCharacter.GetComponent<PlayerFreezeController>();
            if (freezeCtrl != null) freezeCtrl.ApplyFreeze(StunDuration);
            else { hitCharacter.Freeze(); StartCoroutine(UnFreezeDelay(hitCharacter, StunDuration)); }

            // 缴械逻辑
            CharacterHandleWeapon handleWeapon = hitCharacter.GetComponent<CharacterHandleWeapon>();
            if (handleWeapon != null) handleWeapon.ChangeWeapon(null, "Weapon1");
        }

        // [核心防崩] 安全销毁流程
        StartCoroutine(SafeDestroyRoutine());
    }

    IEnumerator SafeDestroyRoutine()
    {
        // 1. 立刻关闭物理碰撞 (防止物理引擎计算爆炸)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        // 2. 关闭显示 (看起来像消失了)
        Renderer ren = GetComponent<Renderer>();
        if (ren != null) ren.enabled = false;

        // 3. 等待一帧 (让物理引擎缓口气)
        yield return null;

        // 4. 安全销毁
        Destroy(gameObject);
    }

    IEnumerator UnFreezeDelay(Character c, float delay)
    {
        yield return new WaitForSeconds(delay);
        c.UnFreeze();
    }
}