using UnityEngine;
using System.Collections;
public class gohome : MonoBehaviour
{


    public Animator animator;
    public string triggerName = "go";
    public GameObject nextObject;
    public GameObject lastObject;
    public float animationLength = 7f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
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
                lastObject.SetActive(false);
            }
        }
    }
}
