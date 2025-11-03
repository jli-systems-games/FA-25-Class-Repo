using System.Collections.Generic;
using UnityEngine;

public class TargetItemManager : MonoBehaviour
{
    List<InteractableItem> currentTargets = new List<InteractableItem>();

    // UI는 나중에 연결할 거라 비워둠
    // public TextMeshProUGUI[] targetTexts;

    public void SetTargets(List<InteractableItem> targets)
    {
        currentTargets = targets;

        // TODO: UI 갱신
        // for (int i = 0; i < targetTexts.Length; i++)
        //     targetTexts[i].text = i < targets.Count ? targets[i].GetName() : "";
    }

    public void ClearTargets()
    {
        currentTargets.Clear();
        // TODO: UI 비우기
    }

    public void OnItemFound(InteractableItem foundItem)
    {
        if (currentTargets.Contains(foundItem))
        {
            currentTargets.Remove(foundItem);
            // TODO: UI에서 체크 표시
            // 만약 다 찾았으면 GameManager에 알리기
            if (currentTargets.Count == 0)
            {
                GameManager.Instance.OnTargetAllFound();
            }
        }
        else
        {
            // 목표가 아닌 물건을 찾았을 때 효과 넣어도 됨
            // Debug.Log("Not a target!");
        }
    }

    // 플레이어가 클릭했을 때 이름으로 확인하고 싶으면 이거 써도 됨
    public void OnItemFoundByName(string itemName)
    {
        InteractableItem hit = currentTargets.Find(i => i.GetName() == itemName);
        if (hit != null)
        {
            OnItemFound(hit);
        }
    }

    public List<InteractableItem> GetCurrentTargets()
    {
        return currentTargets;
    }
}
