using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    public GameObject target; // 켜고/끄고 싶은 UI 루트 (Panel/Canvas 등)

    // 버튼의 OnClick()에 이 함수를 연결하세요.
    public void Toggle()
    {
        if (!target) return;
        target.SetActive(!target.activeSelf);
    }
}