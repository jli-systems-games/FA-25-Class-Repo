using UnityEngine;
using UnityEngine.UI;
public class DrawScript : MonoBehaviour
{
    public RawImage canvasImage;
    public int textureWidth = 300;
    public int textureHeight = 300;
    public int brushSize = 4;

    private Texture2D drawTexture;
    private Color drawColor = Color.red;
    private Vector2? lastPos = null;

    void Start()
    {
        drawTexture = new Texture2D(textureWidth, textureHeight);
        for (int x = 0; x < textureWidth; x++)
            for (int y = 0; y < textureHeight; y++)
                drawTexture.SetPixel(x, y, Color.white);
        drawTexture.Apply();
        canvasImage.texture = drawTexture;
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasImage.rectTransform,
                Input.mousePosition,
                null,
                out localPoint
            );

            float px = (localPoint.x + canvasImage.rectTransform.rect.width / 2) / canvasImage.rectTransform.rect.width;
            float py = (localPoint.y + canvasImage.rectTransform.rect.height / 2) / canvasImage.rectTransform.rect.height;

            int x = Mathf.RoundToInt(px * textureWidth);
            int y = Mathf.RoundToInt(py * textureHeight);

            if (x >= 0 && x < textureWidth && y >= 0 && y < textureHeight)
            {
                Vector2 currentPos = new Vector2(x, y);

                if (lastPos != null)
                {
                    DrawLine(lastPos.Value, currentPos);
                }
                else
                {
                    DrawCircle(x, y);
                }

                lastPos = currentPos;
                drawTexture.Apply();
            }
        }
        else
        {
            lastPos = null;
        }
    }

    void DrawCircle(int cx, int cy)
    {
        for (int i = -brushSize; i <= brushSize; i++)
        {
            for (int j = -brushSize; j <= brushSize; j++)
            {
                if (cx + i >= 0 && cx + i < textureWidth && cy + j >= 0 && cy + j < textureHeight)
                {
                    if (i * i + j * j <= brushSize * brushSize)
                        drawTexture.SetPixel(cx + i, cy + j, drawColor);
                }
            }
        }
    }

    void DrawLine(Vector2 start, Vector2 end)
    {
        int steps = Mathf.CeilToInt(Vector2.Distance(start, end));
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps;
            int x = Mathf.RoundToInt(Mathf.Lerp(start.x, end.x, t));
            int y = Mathf.RoundToInt(Mathf.Lerp(start.y, end.y, t));
            DrawCircle(x, y);
        }
    }

    public void SaveAndNext()
    {
        DrawingStorage.savedTexture = drawTexture;
        UnityEngine.SceneManagement.SceneManager.LoadScene("SampleScene");
    }
}
