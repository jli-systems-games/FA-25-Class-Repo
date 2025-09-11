using UnityEngine;
using UnityEngine.UI;
public class StartControl : MonoBehaviour
{
    public GameObject[] scriptsToEnable; // 켜줄 스크립트가 붙은 오브젝트들
    Button btn;

    void Awake()
    {
        btn = GetComponent<Button>();

        // 처음엔 게임 스크립트들 비활성화
        foreach (var obj in scriptsToEnable)
            if (obj != null) obj.SetActive(false);

        btn.onClick.AddListener(OnStart);
    }

    void OnStart()
    {
        // 버튼은 사라지고
        gameObject.SetActive(false);

        // 게임 스크립트 오브젝트들 활성화
        foreach (var obj in scriptsToEnable)
            if (obj != null) obj.SetActive(true);
    }
}