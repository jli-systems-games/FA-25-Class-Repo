using UnityEngine;

public class NPCHairController : MonoBehaviour
{
    [Tooltip("0 = 풍성, 5 = 대머리")]
    public GameObject[] hairStages;  
    [Range(0, 5)]
    public int currentStage = 0;

    void Start()
    {
        UpdateHairVisual();
    }

    public void SetStage(int stage)
    {
        currentStage = Mathf.Clamp(stage, 0, hairStages.Length - 1);
        UpdateHairVisual();
    }

    void UpdateHairVisual()
    {
        for (int i = 0; i < hairStages.Length; i++)
        {
            if (hairStages[i] != null)
                hairStages[i].SetActive(i == currentStage);
        }
    }

    public void LoseHair(int amount = 1)
    {
        SetStage(currentStage + amount);
    }

    public void RecoverHair(int amount = 1)
    {
        SetStage(currentStage - amount);
    }
}
