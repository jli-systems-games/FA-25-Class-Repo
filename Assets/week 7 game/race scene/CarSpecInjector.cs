using UnityEngine;

public class CarSpecInjector : MonoBehaviour
{
    public enum SpecMode { SingleSpec, ArrayBySavedIndex }

    [Header("주입 대상(컨트롤러)")]
    public ArcadeCarController controllerTarget;   // CarRoot(컨트롤러가 붙은 오브젝트) 드래그

    [Header("어떤 방식으로 SO를 고를지")]
    public SpecMode specMode = SpecMode.ArrayBySavedIndex;

    [Header("SingleSpec 모드용")]
    public CarSpec singleSpec;

    [Header("ArrayBySavedIndex 모드용")]
    public CarSpec[] carsSO;          // 커스터마이즈 씬과 같은 순서로
    public string indexKey = "car_index"; // PlayerPrefs 키

    [Header("시점")]
    public bool applyOnStart = true;  // 시작 시 자동 주입
    public bool forceEnableController = true; // 컨트롤러 꺼져 있으면 켬

    void Start()
    {
        if (applyOnStart)
            TryApply();
    }

    /// <summary>필요할 때 외부에서 호출해도 됨</summary>
    public void TryApply()
    {
        if (!controllerTarget)
        {
            Debug.LogWarning("[CarSpecInjector] controllerTarget이 비어 있습니다.");
            return;
        }

        CarSpec spec = null;

        if (specMode == SpecMode.SingleSpec)
        {
            spec = singleSpec;
        }
        else // ArrayBySavedIndex
        {
            if (carsSO == null || carsSO.Length == 0)
            {
                Debug.LogWarning("[CarSpecInjector] carsSO 배열이 비어 있습니다.");
                return;
            }
            int idx = Mathf.Clamp(PlayerPrefs.GetInt(indexKey, 0), 0, carsSO.Length - 1);
            spec = carsSO[idx];
        }

        if (!spec)
        {
            Debug.LogWarning("[CarSpecInjector] 선택된 CarSpec이 null 입니다.");
            return;
        }

        controllerTarget.ApplySpec(spec);
        if (forceEnableController) controllerTarget.enabled = true;

        Debug.Log($"[CarSpecInjector] '{controllerTarget.name}' 에 '{spec.name}' 적용 완료");
    }
}