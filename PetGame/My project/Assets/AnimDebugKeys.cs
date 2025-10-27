using UnityEngine;

public class AnimDebugKeys : MonoBehaviour
{
    public Animator anim;
    void Start() { if (!anim) anim = GetComponentInChildren<Animator>(); }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) anim.SetTrigger("Jump");
        if (Input.GetKeyDown(KeyCode.Alpha2)) anim.SetTrigger("Drink");
        if (Input.GetKeyDown(KeyCode.Alpha3)) anim.SetTrigger("Kick");
        if (Input.GetKeyDown(KeyCode.Alpha4)) anim.SetTrigger("Punch");
        if (Input.GetKeyDown(KeyCode.Alpha5)) anim.SetTrigger("Die");
        if (Input.GetKeyDown(KeyCode.Alpha6)) anim.SetTrigger("Revive");
        if (Input.GetKeyDown(KeyCode.Alpha7)) anim.SetBool("Sad", !anim.GetBool("Sad"));
        if (Input.GetKeyDown(KeyCode.Alpha8)) anim.SetBool("IsDrunk", !anim.GetBool("IsDrunk"));
        if (Input.GetKeyDown(KeyCode.Alpha9)) anim.SetBool("IsInjured", !anim.GetBool("IsInjured"));
        if (Input.GetKeyDown(KeyCode.Alpha0)) anim.SetBool("Sleep", !anim.GetBool("Sleep"));
    }
}
