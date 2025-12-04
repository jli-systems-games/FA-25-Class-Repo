using UnityEngine;
using System.Collections.Generic;

public class RandomWeaponGenerator : MonoBehaviour
{
    // 定义一个简单的类，用来配置每个武器的权重
    [System.Serializable]
    public class WeaponEntry
    {
        public string Name; // 备注名，方便你在编辑器里看是啥
        public GameObject Prefab; // 拖入 RiflePicker 或 SwordPicker
        [Range(1, 100)]
        public int Weight = 10; // 权重：数字越大，出现的概率越高
    }

    [Header("武器盲盒配置 (带权重)")]
    [Tooltip("注意：因为代码改了，之前的列表可能会清空，请重新拖拽配置")]
    public WeaponEntry[] Weapons;

    void Start()
    {
        SpawnRandomWeapon();
    }

    void SpawnRandomWeapon()
    {
        if (Weapons != null && Weapons.Length > 0)
        {
            // --- 权重随机算法 ---

            // 1. 算总权重 (比如 枪80 + 剑20 = 100)
            int totalWeight = 0;
            foreach (var entry in Weapons)
            {
                // 防止有人填0或负数导致BUG
                if (entry.Weight <= 0) entry.Weight = 1;
                totalWeight += entry.Weight;
            }

            // 2. 掷骰子 (比如掷出 0~100 之间的一个数)
            int randomValue = UnityEngine.Random.Range(0, totalWeight);

            // 3. 找出骰子落在哪个区间
            GameObject selectedPrefab = null;
            int currentSum = 0;

            foreach (var entry in Weapons)
            {
                currentSum += entry.Weight;
                // 如果随机数小于当前的累加值，说明落在这个区间了
                if (randomValue < currentSum)
                {
                    selectedPrefab = entry.Prefab;
                    // Debug.Log($"🎲 随机结果: {entry.Name} (权重: {entry.Weight}/{totalWeight})");
                    break;
                }
            }

            // 4. 生成
            if (selectedPrefab != null)
            {
                Instantiate(selectedPrefab, transform.position, transform.rotation);
            }
        }

        // 5. 销毁生成器
        Destroy(gameObject, 0.1f);
    }
}