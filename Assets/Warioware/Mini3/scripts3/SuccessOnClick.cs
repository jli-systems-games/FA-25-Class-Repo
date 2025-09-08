using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SuccessOnClick : MonoBehaviour
{
    public GameObject successUI;   // 성공할 때 보여줄 UI (예: SUCCESS! 텍스트, 하트 이미지)
    public float showTime = 0.6f;  // 몇 초 동안 보여줄지

    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClicked);   // 버튼 클릭 이벤트 등록

        // 시작할 때 성공 UI는 꺼둠
        if (successUI != null) successUI.SetActive(false);
    }

    void OnClicked()
    {
        if (successUI != null)
        {
            StartCoroutine(ShowSuccessUI());
        }
    }

    IEnumerator ShowSuccessUI()
    {
        successUI.SetActive(true);
        yield return new WaitForSeconds(showTime);
        successUI.SetActive(false);
    }
}
