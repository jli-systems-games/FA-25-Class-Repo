using UnityEngine;
using System.Collections;
public class TriggerStartAfterDelay : MonoBehaviour
{
    public Animator animator;  
    public float delay = 10f;
    public float animetime = 6.7f;
    public AudioSource trainsound;
    public GameObject right;
    public GameObject wrong;
    public GameObject fail;
    public GameObject nextlevel;
    public GameObject nextUI;
    public GameObject thislevel;
    public GameObject trainright;
    public GameObject pull;

    void OnEnable()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        StartCoroutine(TriggerAfterDelay());
    }


    IEnumerator TriggerAfterDelay()
    {
        yield return new WaitForSeconds(delay);
        animator.SetTrigger("Start");
        trainsound.Play();
        if (right.activeSelf) 
        {
            StartCoroutine(Rightpath());
        } else
        {
            StartCoroutine(Wrongpath());
        }
    }

    IEnumerator Wrongpath()
    {
        yield return new WaitForSeconds(animetime);
        fail.SetActive(true);
    }

    IEnumerator Rightpath()
    {
        yield return new WaitForSeconds(animetime);
        nextlevel.SetActive(true);
        nextUI.SetActive(true);
        thislevel.SetActive(false);
        trainright.SetActive(false);
        pull.SetActive(false);

    }

}
