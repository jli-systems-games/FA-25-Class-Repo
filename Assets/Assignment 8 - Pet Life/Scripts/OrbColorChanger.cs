using UnityEngine;

public class OrbColorChanger : MonoBehaviour
{
    private Renderer orbRenderer;

    private void Start()
    {
        orbRenderer = GetComponent<Renderer>();
    }

    public void ChangeColor(string colorName)
    {
        Color newColor = new Color(1f, 1f, 1f, 0.5f);

        switch (colorName.ToLower())
        {
            case "red":
                newColor = new Color(1f, 0f, 0f, 0.5f);
                break;
            case "blue":
                newColor = new Color(0f, 0f, 1f, 0.5f);
                break;
            case "yellow":
                newColor = new Color(1f, 1f, 0f, 0.5f);
                break;
            case "white":
                newColor = new Color(1f, 1f, 1f, 0.5f);
                break;
            case "black":
                newColor = new Color(0f, 0f, 0f, 0.5f);
                break;
        }

        orbRenderer.material.color = newColor;
    }
}
