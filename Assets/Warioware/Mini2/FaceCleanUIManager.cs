using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class FaceCleanUIManager : MonoBehaviour
{
    public GameObject[] layers;     // 순서대로 꺼질 레이어(예: 1, 2, 3)
    public GameObject finalSprite;  // 마지막에 남을 스프라이트(예: final)
    public GameObject successUI;    // 성공 UI(비활성 시작 권장)

    public int clicksPerStep = 1;   // 한 장 없앨 때 필요한 클릭 수 (1 또는 2)
    public float successShowTime = 0.6f;
    public string nextScene = "Mini2";   // 성공 후 이동할 씬 이름 (원하면 공백으로 두면 이동 안 함)

    int clickBuffer = 0;    // 현재 스텝에서 누적 클릭 수
    int stepIndex = 0;      // 다음에 꺼질 layers 인덱스
    bool ended = false;

    void Start()
    {
        if (successUI) successUI.SetActive(false);
        // final은 계속 켜둔 상태에서 진행 (layers만 차례로 꺼짐)
    }

    void Update()
    {
        if (ended) return;

        if (Input.GetMouseButtonDown(0))
        {
            clickBuffer++;

            if (clickBuffer >= clicksPerStep)
            {
                clickBuffer = 0;
                HideNextLayer();
            }
        }
    }

    void HideNextLayer()
    {
        if (stepIndex < layers.Length && layers[stepIndex])
        {
            layers[stepIndex].SetActive(false);
            stepIndex++;
        }

        // 모든 레이어가 꺼졌으면(final만 남음) → 성공
        if (stepIndex >= layers.Length)
        {
            ended = true;
            if (successUI)
                StartCoroutine(ShowSuccessThenNext());
            else
                GoNext();
        }
    }

    IEnumerator ShowSuccessThenNext()
    {
        successUI.SetActive(true);
        yield return new WaitForSeconds(successShowTime);
        successUI.SetActive(false);
        GoNext();
    }

    void GoNext()
    {
        if (!string.IsNullOrEmpty(nextScene))
            SceneManager.LoadScene(nextScene);
    }
}