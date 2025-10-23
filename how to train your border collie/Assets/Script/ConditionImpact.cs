using UnityEngine;
using System.Collections;

public class PetActionTimer : MonoBehaviour
{
    public enum PetAction { Idle, Sleep, Play, Eat, Train }

    [Header("Data")]
    public BorderCollieStats stats;      

    [Header("Timer")]
    public float tickInterval = 1f;      

    public PetAction currentAction = PetAction.Idle;

    Coroutine loop;

  
    void OnDisable()
    {
        StopTick();
    }

    // button chaning status
    public void SetIdle() { SetAction(PetAction.Idle); }
    public void SetSleep() { SetAction(PetAction.Sleep); }
    public void SetPlay() { SetAction(PetAction.Play); }
    public void SetEat() { SetAction(PetAction.Eat); }
    public void SetTrain() { SetAction(PetAction.Train); }

    public void SetAction(PetAction a)
    {
        currentAction = a;
        RestartTick();
    }

    IEnumerator TickLoop()
    {
        var wait = new WaitForSeconds(tickInterval);
        while (true)
        {
            if (currentAction != PetAction.Idle)
            {
                Apply(GetDelta(currentAction));
            }
            yield return wait;
        }
    }

    ActionDelta GetDelta(PetAction a)
    {
        switch (a)
        {
            case PetAction.Sleep: return stats.sleepDelta;
            case PetAction.Play: return stats.playDelta;
            case PetAction.Eat: return stats.eatDelta;
            case PetAction.Train: return stats.trainDelta;
            default: return default;
        }
    }

    void Apply(ActionDelta d)
    {
        stats.Energy = Mathf.Clamp(stats.Energy + d.energy, 0, 100);
        stats.Happiness = Mathf.Clamp(stats.Happiness + d.happiness, 0, 100);
        stats.Fullness = Mathf.Clamp(stats.Fullness + d.fullness, 0, 100);
        stats.Skill = Mathf.Clamp(stats.Skill + d.skill, 0, 100);
    }


    public void StartTick()
    {
        if (loop == null) loop = StartCoroutine(TickLoop());
    }

    public void StopTick()
    {
        if (loop != null) { StopCoroutine(loop); loop = null; }
    }

    public void RestartTick()
    {
        StopTick();
        StartTick();
    }
}