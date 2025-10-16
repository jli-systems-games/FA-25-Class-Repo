using UnityEngine;
using UnityEngine.UI;

public class BlockItemButton : MonoBehaviour
{
    public BuildManager buildManager;
    public GameObject blockPrefab;

    void Awake()
    {
        GetComponent<Button>().onClick.AddListener(() =>
        {
            if (buildManager && blockPrefab) buildManager.SelectPrefab(blockPrefab);
        });
    }
}