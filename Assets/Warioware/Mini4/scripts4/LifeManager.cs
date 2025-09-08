using UnityEngine;
using UnityEngine.SceneManagement;

public class LifeManager : MonoBehaviour
{
    public GameObject[] lifeIcons;   // 5개 아이콘(왼→오 순서로 드래그)
    public string failScene = "Fail";

    int lives;

    void Awake()
    {
        lives = lifeIcons.Length;
        for (int i = 0; i < lifeIcons.Length; i++)
            lifeIcons[i].SetActive(true);     // 시작은 모두 켜짐
    }

    public void LoseLife()
    {
        if (lives <= 0) return;

        lives--;
        if (lives >= 0 && lives < lifeIcons.Length)
            lifeIcons[lives].SetActive(false); // 오른쪽(마지막)부터 꺼짐

        if (lives <= 0)
            SceneManager.LoadScene(failScene); // 0이면 실패 씬
    }
}

