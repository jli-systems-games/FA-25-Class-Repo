using UnityEngine;

public class RunGameFlow2 : MonoBehaviour
{
    public KeyCode driveKey = KeyCode.Space;
    public float sfxInterval = 0.7f;

    float timer;
    bool heldPrev;

    void Start()
    {
        AudioHub.Ensure();
    }

    void Update()
    {
        bool held = Input.GetKey(driveKey);
        if (held)
        {
            timer -= Time.deltaTime;
            if (!heldPrev || timer <= 0f)
            {
                PlayAllWheelRunClips();
                timer = sfxInterval;
            }
        }
        else
        {
            timer = 0f;
        }
        heldPrev = held;
    }

    void PlayAllWheelRunClips()
    {
        if (Data.vehicle.wheels == null) return;
        for (int i = 0; i < Data.vehicle.wheels.Count; i++)
        {
            int idx = Data.vehicle.wheels[i].itemIndex;
            if (idx < 0 || idx >= Data.lib.wheelItems.Count) continue;
            var wi = Data.lib.wheelItems[idx];
            var clip = wi.runClip ? wi.runClip : wi.selectClip;
            if (clip) AudioHub.Play2D(clip, 1f);
        }
    }
}
