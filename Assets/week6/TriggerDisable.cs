using UnityEngine;

public class TriggerDisable : MonoBehaviour
{
    [Tooltip("이 오브젝트가 들어왔을 때만 작동합니다.")]
    public GameObject target;               // 닿을 대상(콜라이더 있어야 함)

    [Tooltip("비활성화할 오브젝트(여러 개 가능)")]
    public GameObject[] toDisable;          // 닿으면 끌 오브젝트들

    void OnTriggerEnter(Collider other)
    {
        if (target != null && other.gameObject == target)
        {
            foreach (var go in toDisable)
            {
                if (go) go.SetActive(false);
            }
        }
    }
}
