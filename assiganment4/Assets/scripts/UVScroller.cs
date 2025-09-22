using UnityEngine;

public class UVScroller : MonoBehaviour
{
    public Renderer targetRenderer;
    public Vector2 speed = new Vector2(0.03f, 0.015f);

    private Material mat; 
    private Vector2 uv; 

    void Start()
    {
        if (!targetRenderer) targetRenderer = GetComponent<Renderer>();
        mat = targetRenderer.material; 
    }

    void Update()
    {
        uv += speed * Time.deltaTime;
        mat.mainTextureOffset = uv;
    }
}
