using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public bool isReturningFromTransition = false;
    public bool isClipFullfilled = false;

    public void ResetState()
    {
        isReturningFromTransition = false;
    }
}
