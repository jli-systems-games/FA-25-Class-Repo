using UnityEngine;

public class DogAction : MonoBehaviour
{
    public BorderCollieStats stats; 

    public void Sleep()
    {
        stats.Energy += 10;
    }

    public void Play()
    {
        stats.Happiness += 10;
        stats.Energy -= 5;
        stats.Skill -= 2;
    }

    public void Eat()
    {
        stats.Fullness += 15;
    }

    public void Train()
    {
        stats.Skill += 10;
        stats.Energy -= 8;
        stats.Fullness -= 5;
        stats.Happiness -= 5;
    }
}