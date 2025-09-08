using UnityEngine;

public class PingPongScoreTrigger : MonoBehaviour
{
    public PingPongGameManager gameMan;

    private bool isRestingOnPaddle = false;
    private float timeRestingOnPaddle = 0f;
    private float timeLimit = 2f;

    private void Update()
    {
        if (isRestingOnPaddle)
        {
            timeRestingOnPaddle += Time.deltaTime;
            if (timeRestingOnPaddle > timeLimit)
                gameMan.ResetScore();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "ping pong ball")
        {
            isRestingOnPaddle = true;
            timeRestingOnPaddle = 0f;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "ping pong ball")
        {
            gameMan.IncrementScore();
            isRestingOnPaddle = false;
        }
    }
}
