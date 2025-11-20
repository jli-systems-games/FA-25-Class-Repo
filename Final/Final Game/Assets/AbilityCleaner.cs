using UnityEngine;
using MoreMountains.TopDownEngine;

public class AbilityCleaner : MonoBehaviour
{
    void Start()
    {
        Destroy(GetComponent<CharacterCrouch>());
        Destroy(GetComponent<CharacterDash3D>());
        Destroy(GetComponent<CharacterDash2D>());
    }

    void Update()
    {
        // 拦截 Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // 什么都不做，相当于把这个按键吃掉
            // 如果你很担心其他脚本还能读到输入，可以顺便清一次输入轴
            Input.ResetInputAxes();
        }
    }
}
