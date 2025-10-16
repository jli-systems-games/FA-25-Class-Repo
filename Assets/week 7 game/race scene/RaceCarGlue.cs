using System.Collections;
using System.Linq;
using UnityEngine;

public class RaceCarGlue : MonoBehaviour
{
    [Header("주행 컨트롤러가 붙은 부모(CAR)")]
    public Transform carParent; // 보통 이 스크립트가 붙은 CAR 자신
    [Header("CAR에 붙어 있는 주행 컨트롤러")]
    public ArcadeCarController controller;

    [Header("커스터마이즈와 같은 순서의 CarSpec들")]
    public CarSpec[] carsSO;

    [Header("바로 적용할지 (Start에서 1프레임 대기 후)")]
    public bool applyOnStart = true;

    void Reset()
    {
        carParent = transform;
        controller = GetComponent<ArcadeCarController>();
    }

    IEnumerator Start()
    {
        if (!applyOnStart) yield break;

        // 스폰 직후를 대비해 한 프레임 대기
        yield return null;

        BindSpecToControllerAndColor();
    }

    /// <summary>
    /// 중앙 데이터의 선택값으로 SO를 고르고,
    /// CAR의 ArcadeCarController에 주입 + 스폰된 차의 색도 반영
    /// </summary>
    public void BindSpecToControllerAndColor()
    {
        // 0) 안전장치
        if (controller == null)
        {
            controller = (carParent ? carParent : transform).GetComponentInParent<ArcadeCarController>();
            if (!controller)
            {
                Debug.LogWarning("[RaceCarGlue] ArcadeCarController를 찾지 못했습니다. CAR에 붙여 주세요.");
                return;
            }
        }

        // 1) 중앙 데이터에서 현재 선택을 가져옴
        int idx = Mathf.Clamp(CentralData.Selected.index, 0, (carsSO?.Length ?? 0) - 1);
        CarSpec spec = (carsSO != null && carsSO.Length > 0) ? carsSO[idx] : null;

        if (spec == null)
        {
            Debug.LogWarning("[RaceCarGlue] carsSO가 비었거나 인덱스가 유효하지 않습니다.");
        }
        else
        {
            // 2) SO → 주행 파라미터 주입
            controller.ApplySpec(spec);
            controller.enabled = true;
            Debug.Log($"[RaceCarGlue] ApplySpec: {spec.name} / moveSpeed={controller.moveSpeed}");
        }

        // 3) 스폰된 차 찾기 (CAR 자식 우선, 없으면 씬에서 가장 최근 스폰 추정)
        GameObject spawned = FindSpawnedCar();
        if (spawned)
        {
            // 4) 색 반영 (CarColor 있으면 우선, 없으면 Renderer.material.color)
            ApplyColor(spawned, CentralData.Selected.color);
        }
        else
        {
            Debug.LogWarning("[RaceCarGlue] 스폰된 차를 찾지 못했습니다. RaceSpawnLoader의 spawn이 CAR 하위로 들어오게 하거나, 이름/태그를 확인하세요.");
        }
    }

    GameObject FindSpawnedCar()
    {
        Transform root = carParent ? carParent : transform;

        // (A) CAR 자식 중 프리팹 클론 찾기
        var child = root.GetComponentsInChildren<Transform>(true)
                        .Select(t => t.gameObject)
                        .FirstOrDefault(go =>
                            go != this.gameObject &&
                            (go.name.Contains("(Clone)") || go.GetComponent<CarColor>() || go.GetComponentInChildren<CarColor>(true)));

        if (child) return child;

        // (B) 씬 전체에서 가장 그럴듯한 후보
        var any = GameObject.FindObjectsOfType<CarColor>(true)
                            .Select(cc => cc.gameObject)
                            .FirstOrDefault();
        return any;
    }

    void ApplyColor(GameObject target, Color c)
    {
        var carColor = target.GetComponent<CarColor>() ?? target.GetComponentInChildren<CarColor>(true);
        if (carColor)
        {
            carColor.ApplyColor(c);
            return;
        }

        // 보급형 경로: 모든 Renderer에 색 덮어쓰기
        var rends = target.GetComponentsInChildren<Renderer>(true);
        foreach (var r in rends)
        {
            if (!r.sharedMaterial || !r.sharedMaterial.HasProperty("_Color")) continue;
            // 인스턴싱된 머티리얼로 착색
            var inst = r.material;
            if (inst.HasProperty("_Color")) inst.color = c;
        }
    }
}
