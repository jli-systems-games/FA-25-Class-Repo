using System.Collections;
using UnityEngine;
using TMPro;
public class TypewriterEffect : MonoBehaviour
{
    public TextMeshProUGUI text;
    public float speed = 0.05f;

    void Start()
    {
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        string fullText = text.text;  
        text.text = "";               
        foreach (char c in fullText)
        {
            text.text += c;
            yield return new WaitForSeconds(speed);
        }
    }
}