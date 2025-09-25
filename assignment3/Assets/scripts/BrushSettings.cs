using UnityEngine;

[CreateAssetMenu(fileName = "BrushSettings", menuName = "FidgetPaint/Brush")]
public class BrushSettings : ScriptableObject
{
    [Header("Appearance")]
    public Gradient colorGradient;
    public AnimationCurve widthCurve = AnimationCurve.Linear(0, 0.12f, 1, 0.1f);

    [Header("Fidget Feel")]
    [Range(0f, 1f)] public float noiseAmount = 0.2f;
    [Range(0f, 1f)] public float magnetStrength = 0.35f;
    [Range(0f, 1f)] public float elasticReturn = 0.4f;
    public float pointMinDistance = 0.03f;
    public float maxPointPerSecond = 120f;

    [Header("SFX")]
    public AudioClip beginClip;
    public AudioClip drawClip;
    public AudioClip endClip;
}
