using UnityEngine;
using System.Collections;

public class NewAnimationController : MonoBehaviour
{
    public Animator spider;
    public string idleTrigger = "IsItchy";

    void OnMouseDown()
    {
        spider.SetTrigger(idleTrigger);
    }
}
