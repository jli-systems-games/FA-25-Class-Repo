using UnityEngine;

public class P3ButtonDebug : MonoBehaviour
{
    void Update()
    {
        // 只测试第二个手柄（Joystick2）的前 10 个按钮
        for (int i = 0; i < 10; i++)
        {
            KeyCode code = (KeyCode)((int)KeyCode.Joystick2Button0 + i);
            if (Input.GetKeyDown(code))
            {
                Debug.Log($"[P3ButtonDebug] Joystick2 Button {i} DOWN : {code}");
            }
        }
    }
}
