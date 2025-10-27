using UnityEngine;

public class SceneDecayConfig : MonoBehaviour
{
    public float decayInterval;
    public int HappyDecay;
    public int HungerDecay;
    public int HealthDecay;

    float timer;

    void Update()
    {
        if (GameState.Instance == null) return;

        timer += Time.deltaTime;

        if (timer >= decayInterval)
        {
            timer = 0f;
            GameState.Instance.Add(HappyDecay, HungerDecay, HealthDecay);
        }
    }
}