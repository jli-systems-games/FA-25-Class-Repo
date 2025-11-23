using UnityEngine;

public class SimpleWireframe : MonoBehaviour
{
    [Header("Wireframe Settings")]
    public bool enableWireframe = true;

    [Header("Optional: Only show wireframe for specific objects")]
    public bool allObjects = true;
    public GameObject[] specificObjects;

    void OnPreRender()
    {
        if (enableWireframe)
        {
            GL.wireframe = true;
        }
    }

    void OnPostRender()
    {
        GL.wireframe = false;
    }
}
