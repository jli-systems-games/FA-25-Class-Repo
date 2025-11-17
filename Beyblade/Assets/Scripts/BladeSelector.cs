using UnityEngine;
using UnityEngine.UI;

public class BladeSelector : MonoBehaviour
{
    public Image display;
    public Sprite[] options;
    public Text nameText;

    private int currentIndex = 0;

    void Start()
    {
        UpdateDisplay();
    }

    public void Next()
    {
        currentIndex = (currentIndex + 1) % options.Length;
        UpdateDisplay();
    }

    public void Previous()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = options.Length - 1;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        display.sprite = options[currentIndex];
        nameText.text = options[currentIndex].name;
    }

    public int GetIndex()
    {
        return currentIndex;
    }
}
