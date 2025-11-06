using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AssignTime : MonoBehaviour
{
    public TextMeshProUGUI currentTimeText;
    public TextMeshProUGUI highTimeText;

    void Start()
    {
        currentTimeText.text = Data.currentTime.ToString("F2");
        highTimeText.text = Data.highScore.ToString("F2");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SceneManager.LoadScene("Start Scene");
        }
    }
}
