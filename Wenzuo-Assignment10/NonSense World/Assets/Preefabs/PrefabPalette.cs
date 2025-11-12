using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PrefabPalette", menuName = "Nonsense/Prefab Palette")]
public class PrefabPalette : ScriptableObject
{
    [Header("用来当“脚下平台”的 Prefab（楼块、屋顶、道路、地板、招牌大板等）")]
    public List<GameObject> platformPrefabs = new();

    [Header("连接小件（可空，没就用程序化木板）")]
    public GameObject bridgePrefab; // 可为空；为空就用 Cube 生成窄桥

    [Header("随缘点缀（不影响可达性）")]
    public List<GameObject> propPrefabs = new();

    [Header("稀有件（关卡目标，从这里抽一个）")]
    public List<GameObject> rarePrefabs = new();
}
