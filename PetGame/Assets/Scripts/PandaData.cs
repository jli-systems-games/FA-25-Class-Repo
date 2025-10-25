using UnityEngine;

[CreateAssetMenu(fileName = "NewPandaData", menuName = "PetSim/Panda Data", order = 0)]
public class PandaData : ScriptableObject
{
    [Header("Panda Info")]
    public string pandaName = "Pandy";
    [TextArea(2, 4)]
    public string description = "A gentle panda who loves bamboo and naps.";

    [Header("Starting Stats (0–100)")]
    [Range(0f, 100f)] public float startHunger = 100f;
    [Range(0f, 100f)] public float startEnergy = 100f;
    [Range(0f, 100f)] public float startFun = 100f;
    [Range(0f, 100f)] public float startCleanliness = 100f;

    [Header("Stat Drain Rates (per second)")]
    public float hungerDrain = 5f;
    public float energyDrain = 2.5f;
    public float funDrain = 3f;
    public float cleanlinessDrain = 3.5f;

    [Header("Regen Amounts (when actions are performed)")]
    public float feedGain = 30f;
    public float sleepGain = 40f;
    public float playGain = 40f;
    public float washGain = 40f;

    [Header("Audio / Visual Settings (optional)")]
    public AudioClip eatClip;
    public AudioClip sleepClip;
    public AudioClip washClip;
    public AudioClip playClip;
    public Sprite pandaPortrait;
}