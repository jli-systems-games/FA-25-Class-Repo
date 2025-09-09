using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ButtonClickTwice : MonoBehaviour
{
    public GameObject targetSprite;   // 첫 번째 클릭 때 숨길 스프라이트
    public string nextScene = "Mini1";

    int clickCount = 0;
    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);

        if (targetSprite != null) targetSprite.SetActive(true); // 시작할 때 보이게
    }

    void OnClicked()
    {
        clickCount++;

        if (clickCount == 1)
        {
            if (targetSprite != null)
                targetSprite.SetActive(false); // 첫 번째 클릭 → 스프라이트 숨기기
        }
        else if (clickCount == 2)
        {
            SceneManager.LoadScene(nextScene); // 두 번째 클릭 → 씬 전환
        }
    }
}
