using UnityEngine;

public class AvatarApply : MonoBehaviour
{
    public GameObject playerA_truck;
    public GameObject playerA_magician;
    public GameObject playerA_doctor;

    public GameObject playerB_truck;
    public GameObject playerB_magician;
    public GameObject playerB_doctor;

    void Start()
    {
        ApplyAvatarSelection();
    }

    void ApplyAvatarSelection()
    {
        playerA_truck.SetActive(false);
        playerA_magician.SetActive(false);
        playerA_doctor.SetActive(false);

        playerB_truck.SetActive(false);
        playerB_magician.SetActive(false);
        playerB_doctor.SetActive(false);

        int p1 = AvatarSelectionManager.player1Avatar;
        int p2 = AvatarSelectionManager.player2Avatar;

        switch (p1)
        {
            case 0: playerA_truck.SetActive(true); break;
            case 1: playerA_magician.SetActive(true); break;
            case 2: playerA_doctor.SetActive(true); break;
        }

        // Player B ∆Ù∂Ø∂‘”¶ avatar
        switch (p2)
        {
            case 0: playerB_truck.SetActive(true); break;
            case 1: playerB_magician.SetActive(true); break;
            case 2: playerB_doctor.SetActive(true); break;
        }
    }
}
