using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSpriteSwitcher : MonoBehaviour
{
    [Header("Target")]
    public Image targetImage;

    [Header("Sprites (bind OR auto-load by name)")]
    public Sprite Idle, Calm, HighCraving, LowPhysical, Smoking, ChewGum, Breathe, Win, Fail;

    [Header("Auto-Load From Resources/arts (if true)")]
    public bool autoLoadFromResources = true;

    private bool playingOneShot = false;
    private bool finalLocked = false;  
    private string lastLoopPose = "Idle";
    private Dictionary<string, Sprite> map;

    void Awake()
    {
        if (targetImage == null) targetImage = GetComponent<Image>();
        map = new Dictionary<string, Sprite>();

        if (autoLoadFromResources)
        {
            LoadByName("Idle"); LoadByName("Calm"); LoadByName("HighCraving"); LoadByName("LowPhysical");
            LoadByName("Smoking"); LoadByName("ChewGum"); LoadByName("Breathe"); LoadByName("Win"); LoadByName("Fail");
        }
        else
        {
            SafePut("Idle", Idle); SafePut("Calm", Calm); SafePut("HighCraving", HighCraving); SafePut("LowPhysical", LowPhysical);
            SafePut("Smoking", Smoking); SafePut("ChewGum", ChewGum); SafePut("Breathe", Breathe); SafePut("Win", Win); SafePut("Fail", Fail);
        }

        if (targetImage != null && targetImage.sprite == null) SetPose("Idle");
    }

    void LoadByName(string name)
    {
        var sp = Resources.Load<Sprite>("arts/" + name);
        SafePut(name, sp);
    }

    void SafePut(string key, Sprite sp)
    {
        if (!map.ContainsKey(key)) map.Add(key, sp);
    }

    public void SetLoopPoseByValues(float craving, float physical, bool inEvent)
    {
        if (playingOneShot || finalLocked) return;

        string pose;
        if (physical <= 35f) pose = "LowPhysical";
        else if (craving >= 75f && inEvent) pose = "HighCraving";
        else if (craving <= 40f && !inEvent) pose = "Calm";
        else pose = "Idle";

        lastLoopPose = pose;
        SetPose(pose);
    }

    public void PlayOneShot(string pose, float holdSeconds = 0.8f)
    {
        if (finalLocked) return;
        StartCoroutine(CoOneShot(pose, holdSeconds));
    }

    IEnumerator CoOneShot(string pose, float hold)
    {
        playingOneShot = true;
        SetPose(pose);
        if (hold > 0f) yield return new WaitForSeconds(hold);
        playingOneShot = false;
        SetPose(lastLoopPose);
    }

    public void LockPose(string pose)
    {
        finalLocked = true;
        playingOneShot = true;
        SetPose(pose);
    }

    public void UnlockPose()
    {
        finalLocked = false;
        playingOneShot = false;
        SetPose(lastLoopPose);
    }

    public void SetPose(string poseName)
    {
        if (targetImage == null) return;
        if (map != null && map.TryGetValue(poseName, out var sp) && sp != null)
        {
            targetImage.sprite = sp;
        }
        else
        {
            var fallback = Resources.Load<Sprite>("arts/" + poseName);
            if (fallback != null)
            {
                if (!map.ContainsKey(poseName)) map.Add(poseName, fallback);
                targetImage.sprite = fallback;
            }
        }
    }
}
