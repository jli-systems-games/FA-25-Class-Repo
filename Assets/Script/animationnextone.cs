using UnityEngine;
using System.Collections;

public class ClickToPlayAnimation : MonoBehaviour
{
    public Animator animator;
    public string triggerName = "click";
    public GameObject nextObject;
    public GameObject lastObject;
    public float animationLength = 4f;

    // Call this from Button OnClick
    public void OnButtonClick()
    {
        StartCoroutine(PlayAndShowNext());
    }

    IEnumerator PlayAndShowNext()
    {
        if (animator != null)
        {
            animator.SetTrigger(triggerName);
        }

        yield return new WaitForSeconds(animationLength);

        if (nextObject != null)
        {
            nextObject.SetActive(true);
        }

        if (lastObject != null)
        {
            lastObject.SetActive(false);
        }
    }
}
