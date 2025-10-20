using UnityEngine;

public class Customizer : MonoBehaviour
{
    public GameObject[] availableBlocks;

    private BuildManager build;

    void Start()
    {
        build = Object.FindFirstObjectByType<BuildManager>();
    }

    public void SelectBlock(int index)
    {
        if (!build) return;
        if (index < 0 || index >= availableBlocks.Length) return;

        var prefab = availableBlocks[index];
        build.SelectPrefab(prefab);
    }
}