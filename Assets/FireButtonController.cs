using UnityEngine;
using UnityEngine.UI;

public class FireButtonController : MonoBehaviour
{
    public Button fireBtn;
    public WaterShooter shooter;
    public GameManager gameManager;

    private const int fireCost = 1000;

    void Start()
    {
        if (fireBtn == null)
            fireBtn = GetComponent<Button>();
    }

    void Update()
    {
        if (gameManager.CurrentScore < fireCost)
        {
            fireBtn.interactable = false;
        }
        else
        {
            fireBtn.interactable = true;
        }
    }

    public void OnFireButtonPressed()
    {
        if (gameManager.CurrentScore >= fireCost)
        {
            gameManager.SpendScoreInstant(fireCost);
            shooter.ActivateFireDrop();
        }
    }
}
