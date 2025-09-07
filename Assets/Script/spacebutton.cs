using UnityEngine;
using System.Collections;
public class PlayAnimationOnSpace : MonoBehaviour
{
    //public Animator animator; 
    //public string animationdownTrigger = "down";
    //public string animationupTrigger = "up";
    //void Update()
    //{
    //    if (Input.GetKeyDown(KeyCode.Space))
    //    {
    //        animator.SetTrigger(animationdownTrigger);
    //    }
    //    if (Input.GetKeyUp(KeyCode.Space))
    //    {
    //        animator.SetTrigger(animationupTrigger);
    //    }
    //}
    public GameObject up;
    public GameObject down;
    public GameObject level;
    public GameObject start;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            up.SetActive(false);
            down.SetActive(true);
            StartCoroutine(ActiveNew());

        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            up.SetActive(true);
            down.SetActive(false);
        }
    }
    IEnumerator ActiveNew()
    {
     

        yield return new WaitForSeconds(2f);

        start.SetActive(false);
        level.SetActive(true);
    }
}
