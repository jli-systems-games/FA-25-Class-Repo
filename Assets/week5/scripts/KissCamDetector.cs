using UnityEngine;

public class KissCamDetector : MonoBehaviour
{
    public KissCamController cam;
    public PlayerStateController player;

    void Update()
    {
        if (cam == null || player == null) return;

        if (cam.GetState() == KissCamState.Moving && cam.IsAtCheckpoint() && player.IsHug())
        {
            GameManager.I?.GameOver();
            return;
        }

        if (cam.GetState() == KissCamState.Stopping && player.IsHug())
        {
            GameManager.I?.GameOver();
            return;
        }
    }
}
