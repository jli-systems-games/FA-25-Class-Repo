using UnityEngine;

public class UIInstructionManager : MonoBehaviour
{
    public static UIInstructionManager Instance;

    public GameObject instructionText;

    void Awake()
    {
        Instance = this;
        instructionText.SetActive(false);
    }

    public void Show()
    {
        instructionText.SetActive(true);
    }

    public void Hide()
    {
        instructionText.SetActive(false);
    }
}
