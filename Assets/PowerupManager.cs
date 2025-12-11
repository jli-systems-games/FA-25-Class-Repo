using UnityEngine;

public enum MainPowerup
{
    None,
    MechanicalArm,
    DevilsHorny,
    FroggyHat,
    Lipstick,
    MerryXmas,
    SpinningHat
}
public class PowerupManager : MonoBehaviour
{
    public static PowerupManager Instance;

    public MainPowerup currentMain = MainPowerup.None;
    public bool fireDropActive = false;
    public bool heartDropActive = false;
    public bool aimDropActive = false;

    public GameManager gameManager;
    public WaterShooter shooter;
    public PottyFlowController flowController;
    public PottyMover[] pottyMovers;
    public MusicManager musicManager;

    public GameObject[] mechanicalArmVisuals;
    public GameObject[] xMasVisuals;
    public GameObject[] devilsHornyVisuals;
    public GameObject[] froggyHatVisuals;
    public GameObject[] lipstickVisuals;
    public GameObject[] spinningHatVisuals;

    private void Awake()
    {
        Instance = this;
    }

    public void EquipMain(MainPowerup type)
    {
        ResetMainEffects();

        currentMain = type;

        switch (type)
        {
            case MainPowerup.MechanicalArm:
                ApplyMechanicalArm();
                break;
            case MainPowerup.DevilsHorny:
                ApplyDevilsHorny();
                break;
            case MainPowerup.FroggyHat:
                ApplyFroggyHat();
                break;
            case MainPowerup.Lipstick:
                ApplyLipstick();
                break;
            case MainPowerup.MerryXmas:
                ApplyMerryXmas();
                break;
            case MainPowerup.SpinningHat:
                ApplySpinningHat();
                break;
        }
        RefreshPottyVisuals();
    }

    public void EquipHeartDrop()
    {
        heartDropActive = true;
        shooter.ActivateHeartDrop();
    }
    public void EquipFireDrop()
    {
        fireDropActive = true;
        shooter.ActivateFireDrop();
    }
    public void EquipAimDrop()
    {
        aimDropActive = true;
        shooter.ActivateAimDrop();
    }

    void RefreshPottyVisuals()
    {
        SetVisualArrayActive(mechanicalArmVisuals, false);
        SetVisualArrayActive(devilsHornyVisuals, false);
        SetVisualArrayActive(froggyHatVisuals, false);
        SetVisualArrayActive(xMasVisuals, false);
        SetVisualArrayActive(lipstickVisuals, false);
        SetVisualArrayActive(spinningHatVisuals, false);

        switch (currentMain)
        {
            case MainPowerup.MechanicalArm:
                SetVisualArrayActive(mechanicalArmVisuals, true);
                break;

            case MainPowerup.DevilsHorny:
                SetVisualArrayActive(devilsHornyVisuals, true);
                break;

            case MainPowerup.FroggyHat:
                SetVisualArrayActive(froggyHatVisuals, true);
                break;

            case MainPowerup.Lipstick:
                SetVisualArrayActive(lipstickVisuals, true);
                break;

            case MainPowerup.SpinningHat:
                SetVisualArrayActive(spinningHatVisuals, true);
                break;

            case MainPowerup.MerryXmas:
                SetVisualArrayActive(xMasVisuals, true);
                break;

            case MainPowerup.None:
            default:
                break;
        }
    }

    void SetVisualArrayActive(GameObject[] arr, bool active)
    {
        if (arr == null) return;
        for (int i = 0; i < arr.Length; i++)
        {
            if (arr[i] != null)
                arr[i].SetActive(active);
        }
    }

    public void ResetMainEffects()
    {
        shooter.ResetRange();
        shooter.ResetAttackSpeed();

        if (flowController != null)
        {
            flowController.thresholdScale = 1f;
            flowController.extraScorePerPhase = 0;
        }

        if (musicManager != null)
        {
            musicManager.UseLipstickTrack(false);
        }
    }

    void ApplyFroggyHat()
    {
        if (pottyMovers != null)
        {
            for (int i = 0; i < pottyMovers.Length; i++)
            {
                if (pottyMovers[i] != null)
                    pottyMovers[i].enabled = true;
            }
        }
    }


    void ApplyMechanicalArm()
    {
        shooter.SetRangeMultiplier(3f);
        shooter.SetAttackSpeed(0.05f);
    }

    void ApplyDevilsHorny()
    {
        if (flowController != null)
        {
            flowController.thresholdScale = 0.7f;
        }
    }

    void ApplyLipstick()
    {
        if (musicManager != null)
        {
            musicManager.UseLipstickTrack(true);
        }
    }

    void ApplyMerryXmas()
    {
        if (gameManager == null) return;

        int amount = Random.Range(-1000, 12251);

        gameManager.AddScore(amount, true);
    }

    void ApplySpinningHat()
    {
        if (flowController != null)
        {
            flowController.extraScorePerPhase = 500;
        }
    }

    public bool IsLipstickEquipped()
    {
        return currentMain == MainPowerup.Lipstick;
    }
}
