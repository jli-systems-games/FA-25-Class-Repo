using DG.Tweening.Core.Easing;
using UnityEngine;

public class NPCHairDecay : MonoBehaviour
{
    public NPCHairController hairController;
    public float decayInterval = 30f;     // 평소엔 30초마다 단계+1
    public float acidRainMultiplier = 0.3f; // 산성비면 x0.3 = 더 빨리 빠짐

    float timer = 0f;

    void Update()
    {
        if (hairController == null) return;

        timer += Time.deltaTime;

        float currentInterval = decayInterval;

        // 산성비 상태일 때 속도 가속
        //if (GameManager.Instance.isAcidRain)
           // currentInterval *= acidRainMultiplier;

        if (timer >= currentInterval)
        {
            hairController.LoseHair(1);
            timer = 0f;
        }
    }
}
