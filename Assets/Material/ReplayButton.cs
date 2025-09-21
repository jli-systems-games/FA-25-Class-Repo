using UnityEngine;

public class ReplayButton : MonoBehaviour
{
    public SpinnerController motor;   // 피벗(실제로 도는 오브젝트)에 붙은 것
    public SpinCounter counter; // 같은 피벗에 붙은 것

    public void Replay()
    {
        // 비워놨으면 자동으로 씬에서 찾아서 연결
        if (!motor) motor = Object.FindFirstObjectByType<SpinnerController>();
        if (!counter) counter = Object.FindFirstObjectByType<SpinCounter>();

        if (motor) motor.ResetMotor(true);
        if (counter) counter.ResetMission();
    }
}
