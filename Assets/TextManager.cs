using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class TextManager : MonoBehaviour
{
    public TMP_Text displayText;
    public string[] lines;
    public HeadMovement headMovement;

    private int currentIndx = 0;

    private void Start()
    {
        if (lines.Length > 0)
            ShowLine(0);
    }

    public void ShowLine(int index)
    {
        if (index >= 0 && index < lines.Length)
        {
            currentIndx = index;
            displayText.text = lines[index];
        }
    }

    private void Update()
    {
        ShowLine(headMovement.scoreVal);
        if (headMovement.scoreVal == 6)
        {
            StartCoroutine(ToTheEnding());
        }
    }
    private IEnumerator ToTheEnding()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene("Ending");
    }
}
