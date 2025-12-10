using UnityEngine;

public class BlockArrowKeys : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow) ||
            Input.GetKey(KeyCode.DownArrow) ||
            Input.GetKey(KeyCode.LeftArrow) ||
            Input.GetKey(KeyCode.RightArrow))
        {
            Input.ResetInputAxes();
        }
    }
}

