using DG.Tweening.Core.Easing;
using UnityEngine;

public class PottyZone : MonoBehaviour
{
    public GameManager gameManager;
    public bool isReal = false;
    public PottyFlowController flowController;
    private int hitCombo = 0;
    private int chaosHitCount = 0;
    private bool firstTimeHit = true;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Drop"))
            return;

        if (gameManager != null)
        {

            if (isReal)
            {
                int reward = gameManager.correctReward;

                if (gameManager.tripleScoreActive)
                    reward *= 3;

                gameManager.AddScore(reward);

                if (firstTimeHit)
                {
                    AchievementManager.Instance.Unlock("First Splash");
                    firstTimeHit = false;
                }

                hitCombo++;

                if (hitCombo == 10)
                {
                    AchievementManager.Instance.Unlock("Perfect Stream");
                }
                if (flowController != null && flowController.currentLevel == 5)
                {
                    chaosHitCount++;
                    if (chaosHitCount == 3)
                    {
                        AchievementManager.Instance.Unlock("Chaos Survivor");
                    }
                }
                if (flowController != null && flowController.currentLevel == 4 && flowController.firstShotThisLevel)
                {
                    AchievementManager.Instance.Unlock("Sniper Rookie");
                }
            }
            else
            {
                gameManager.AddScore(-gameManager.wrongPenalty);
                hitCombo = 0;
            }
        }

        Destroy(other.gameObject, 0.2f);
    }
}
