using UnityEngine;
using System.Collections;

public class ClickToPlayAnimation : MonoBehaviour
{
    public Animator animator;              
    public string triggerName = "click";    
    public GameObject nextObject;
    public GameObject lastObject;
    public float animationLength = 4f;     

    void OnMouseDown()
    {
        StartCoroutine(PlayAndShowNext());
    }

    IEnumerator PlayAndShowNext()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        // wait for animation to finish
        yield return new WaitForSeconds(animationLength);

        if (nextObject != null)
        {
            nextObject.SetActive(true);
            lastObject.SetActive(false);
        }
    }
}
