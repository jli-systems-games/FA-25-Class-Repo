using UnityEngine;
using UnityEngine.UI;

public class HeartGaugeController : MonoBehaviour
{
    public float maxValue = 100f;
    public float increasePerSecond = 15f;
    public Slider slider;
    public PlayerStateController player;

    float value = 0f;

    void Start() { value = 0f; ApplyUI(); }

    void Update()
    {
        if (player != null && player.IsHug())
        {
            value += increasePerSecond * Time.deltaTime;
            value = Mathf.Clamp(value, 0f, maxValue);
            ApplyUI();
        }
    }

    void ApplyUI()
    {
        if (!slider) return;
        slider.minValue = 0f;
        slider.maxValue = maxValue;
        slider.value = value;
    }

    public bool IsFull() => value >= maxValue;
}
