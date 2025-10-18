using UnityEngine;
using TMPro;
using System.Collections;

public class TypeWriterText : MonoBehaviour
{
    public TMP_Text storyText;
    public string[] lines;
    public float delay = 0.04f;
    private int index = 0;

    void Start()
    {
        StartCoroutine(DisplayLine());
    }

    IEnumerator DisplayLine()
    {
        while (index < lines.Length)
        {
            storyText.text = "";
            foreach (char c in lines[index])
            {
                storyText.text += c;
                yield return new WaitForSeconds(delay);
            }

            
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0));
            index++;
        }

        
        GameObject namePanel = GameObject.Find("NamePanel");
        namePanel.SetActive(true);
    }
}
