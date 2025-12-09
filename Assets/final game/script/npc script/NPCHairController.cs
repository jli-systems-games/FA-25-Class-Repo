using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCHairController : MonoBehaviour
{
    [Header("NPC 스프라이트 교체")]
    [Tooltip("NPC의 SpriteRenderer (이 컴포넌트의 이미지를 바꿉니다)")]
    public SpriteRenderer npcSpriteRenderer;

    [Tooltip("0 = 풍성, 5 = 대머리 (6개의 스프라이트 이미지를 순서대로 넣으세요)")]
    public Sprite[] npcSprites;  
    [Range(0, 5)]
    public int currentStage = 0;


    public string failSceneName = "fail";
    bool _hasTriggeredFail = false;

    void Start()
    {
        // SpriteRenderer가 연결되어 있는지 확인
        if (npcSpriteRenderer == null)
        {
            npcSpriteRenderer = GetComponent<SpriteRenderer>();
        }

        UpdateHairVisual();
    }

    public void SetStage(int stage)
    {
        // currentStage를 0 ~ 5 범위로 유지하고 (npcSprites.Length - 1)
        currentStage = Mathf.Clamp(stage, 0, npcSprites.Length - 1);
        UpdateHairVisual();

        // 🚨 게임 오버 조건 체크 (max stage에 도달)
        if (currentStage == npcSprites.Length - 1)
        {
            _hasTriggeredFail = true;

            if (!string.IsNullOrEmpty(failSceneName))
            {
                SceneManager.LoadScene(failSceneName);
            }
        }
    }

    void UpdateHairVisual()
    {
       
        if (npcSpriteRenderer == null || npcSprites == null || npcSprites.Length == 0)
        {
            Debug.LogError("NPCHairController에 SpriteRenderer 또는 Sprites가 연결되지 않았습니다!", this);
            return;
        }

        if (currentStage >= 0 && currentStage < npcSprites.Length)
        {
            // 현재 단계에 맞는 전체 스프라이트 이미지로 교체
            npcSpriteRenderer.sprite = npcSprites[currentStage];
        }
       
    }

    // (LoseHair, RecoverHair 함수는 기존과 동일하게 SetStage를 호출하므로 수정 불필요)
    public void LoseHair(int amount = 1)
    {
        SetStage(currentStage + amount);
    }

    public void RecoverHair(int amount = 1)
    {
        SetStage(currentStage - amount);
    }
}
