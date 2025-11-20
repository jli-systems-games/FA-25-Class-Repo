using UnityEngine;
using System.Collections.Generic;
using MoreMountains.TopDownEngine;

public class AdvancedWall : MonoBehaviour
{
    [Header("阵营设置 (关键!)")]
    public string OwnerID = "Player1"; // 谁的墙？P1填 Player1，P2填 Player2

    [Header("视觉配置")]
    public GameObject WallBrickPrefab; // 拖入刚才做的 WallBrick
    public GameObject BreakEffect;     // 拖入 BrickExplosion
    public Transform BuildRoot;        // 砖块生成的起始点

    [Header("数值规则")]
    public float CurrentScore = 0f;
    public float ScorePerBrick = 4f;   // 4分 = 1块砖
    public float BrickHeight = 0.5f;   // 你的砖块高度 (Y轴Scale)

    // 用来存已经造好的砖块
    private List<GameObject> _builtBricks = new List<GameObject>();

    // ------------------- 建造逻辑 (自动感应) -------------------
    private void OnTriggerEnter(Collider other)
    {
        // 获取角色和武器
        CharacterHandleWeapon handleWeapon = other.GetComponent<CharacterHandleWeapon>();
        Character character = other.GetComponent<Character>();

        if (character == null || handleWeapon == null || handleWeapon.CurrentWeapon == null) return;

        // 只有“主人”才能建造 (P1 建 P1 的墙)
        if (character.PlayerID == OwnerID)
        {
            string weaponName = handleWeapon.CurrentWeapon.WeaponName;
            float addScore = 0f;

            // 判断石头类型
            if (weaponName == "SmallStone") addScore = 1f;
            else if (weaponName == "BigStone") addScore = 4f;

            // 如果是石头，就开始建造
            if (addScore > 0)
            {
                AddScore(addScore);
                // 没收石头
                handleWeapon.ChangeWeapon(null, "EmptyHands");
            }
        }
    }

    void AddScore(float score)
    {
        CurrentScore += score;

        // 计算应该有多少块砖
        // 例如：8分 = 2块，9分 = 2块，12分 = 3块
        int targetBrickCount = Mathf.FloorToInt(CurrentScore / ScorePerBrick);

        // 如果当前的砖不够，就补齐
        while (_builtBricks.Count < targetBrickCount)
        {
            SpawnBrick();
        }
    }

    void SpawnBrick()
    {
        if (WallBrickPrefab == null) return;

        // 计算新砖块的位置：起始点 + (当前层数 * 砖高)
        Vector3 spawnPos = BuildRoot.position + Vector3.up * (_builtBricks.Count * BrickHeight);

        // 生成砖块
        GameObject newBrick = Instantiate(WallBrickPrefab, spawnPos, Quaternion.identity);

        // 设为子物体，保持整洁
        newBrick.transform.SetParent(this.transform);

        _builtBricks.Add(newBrick);
    }

    // ------------------- 偷窃/破坏逻辑 (主动按键) -------------------
    private void OnTriggerStay(Collider other)
    {
        Character character = other.GetComponent<Character>();
        if (character == null) return;

        // 只有“敌人”才能偷 (ID 不等于 OwnerID)
        if (character.PlayerID != OwnerID)
        {
            bool stealInput = false;

            // P1 偷 P2 (按 F)
            if (character.PlayerID == "Player1" && Input.GetKeyDown(KeyCode.F)) stealInput = true;
            // P2 偷 P1 (按 回车)
            else if (character.PlayerID == "Player2" && Input.GetKeyDown(KeyCode.Return)) stealInput = true;

            if (stealInput)
            {
                StealBrick();
            }
        }
    }

    void StealBrick()
    {
        // 如果没有砖，偷个寂寞
        if (_builtBricks.Count == 0) return;

        // 1. 扣分 (扣掉整整一块砖的分数)
        // 比如现在 13分(3砖多一点)，偷一次变成 9分(2砖多一点)，最上面那块就没了
        CurrentScore = Mathf.Max(0, CurrentScore - ScorePerBrick);

        // 2. 找到最顶上的一块砖
        int lastIndex = _builtBricks.Count - 1;
        GameObject topBrick = _builtBricks[lastIndex];
        Vector3 effectPos = topBrick.transform.position;

        // 3. 销毁它
        _builtBricks.RemoveAt(lastIndex);
        Destroy(topBrick);

        // 4. 播放爆炸特效
        if (BreakEffect != null)
        {
            // 实例化特效，并在 2秒后自动删除
            GameObject fx = Instantiate(BreakEffect, effectPos, Quaternion.identity);
            Destroy(fx, 2f);
        }

        Debug.Log("偷窃成功！破坏了一层墙！");
    }
}