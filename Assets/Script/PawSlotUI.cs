using UnityEngine;
using TMPro;

public class PawSlotUI : MonoBehaviour
{
    public TMP_Text letterText;
    public char letter = 'A';

    public void SetLetter(char c)
    {
        letter = char.ToUpper(c);
        if (letterText) letterText.text = letter.ToString();
    }

    public void Show(bool on)
    {
        gameObject.SetActive(on);
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (letterText) letterText.text = char.ToUpper(letter).ToString();
    }
#endif
}