using UnityEngine;

public enum PetMood { Happy, Sad, Asleep, Injured, Drunk, Dying }
public enum PetMode { FollowMouse, Stage }
public enum StatType { HP, Mood, Energy }

[System.Serializable]
public struct PetVitals
{
    public int hp;
    public int mood;
    public int energy;
    public void Clamp()
    {
        hp = Mathf.Clamp(hp, 0, 100);
        mood = Mathf.Clamp(mood, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
    }
}
