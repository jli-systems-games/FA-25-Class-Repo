using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Prefab2Special : MonoBehaviour
{
    [Header("生成cg的列表")]
    public List<GameObject> prefabList = new List<GameObject>();

    [Header("cg的点")]
    public float minX = 4.8f;
    public float maxX = 12.1f;

    private bool isSpawning = false; // 生成我的cg期间要禁用p要不然容易不小心重新生成大概是这样

    void Update()
    {
        float x = transform.position.x;

        if (!isSpawning && x >= minX && x <= maxX)
        {
            if (Input.GetKeyDown(KeyCode.P))
            {
                SpawnRandomPrefab();
            }
        }
    }

    void SpawnRandomPrefab()
    {
        if (prefabList == null || prefabList.Count == 0) return;

        int index = Random.Range(0, prefabList.Count);
        GameObject obj = Instantiate(prefabList[index]);

        //这个就是生成了之后会再销毁要不然我缺的动画样机这一块
        StartCoroutine(DestroyAfterTime(obj, 14f));
        isSpawning = true; // 禁用 P防止cg重复跳出来
    }

    IEnumerator DestroyAfterTime(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (obj != null)
        {
            Destroy(obj);
            Debug.Log("销毁了 Prefab: " + obj.name);
        }

        isSpawning = false; // 消失了就可以继续生成我的奇妙cg
    }
}
