using UnityEngine;

public class AnimationTrigger : MonoBehaviour
{
    public Animator animator;
    public float checkInterval = 1f; 
    private float timer = 0f;

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= checkInterval)
        {
            timer = 0f;
            int randomNum = Random.Range(1, 101);
            Debug.Log("Random number: " + randomNum);

            if (randomNum % 11 == 0)
            {
                animator.ResetTrigger("jump");
                animator.SetTrigger("move");
            }
            else if (randomNum % 5 == 0)
            {
                animator.ResetTrigger("move");
                animator.SetTrigger("jump");
            }
            else
            {
                
                animator.ResetTrigger("move");
                animator.ResetTrigger("jump");
            }
        }
    }
}
