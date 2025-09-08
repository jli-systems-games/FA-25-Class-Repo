using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class MiniController : MonoBehaviour
{
    public GameObject successUI;      // 성공 팝업(비활성 시작)
    public string nextScene = "Mini02";
    public string failScene = "Fail";
    public float successShowTime = 0.6f;

    bool locked; // 중복 입력 방지

    void Start()
    {
        if (successUI) successUI.SetActive(false);
    }

    // Yes 버튼 OnClick에 연결
    public void OnYes()
    {
        if (locked) return;
        locked = true;
        StartCoroutine(ShowSuccessAndGoNext());
    }

    // No 버튼 OnClick에 연결
    public void OnNo()
    {
        if (locked) return;
        locked = true;
        SceneManager.LoadScene(failScene);
    }

    IEnumerator ShowSuccessAndGoNext()
    {
        if (successUI) successUI.SetActive(true);
        yield return new WaitForSeconds(successShowTime);
        SceneManager.LoadScene(nextScene);
    }
}
