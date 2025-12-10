using UnityEngine;
using MoreMountains.TopDownEngine;

public class P2NumpadMovement : MonoBehaviour
{
    public float inputValue = 1f;      // 每次按键的输入强度

    private Character _character;
    private CharacterMovement _movement;

    void Awake()
    {
        _character = GetComponent<Character>();
        _movement = GetComponent<CharacterMovement>();
    }

    void Update()
    {
        if (_character == null || _movement == null)
            return;

        // 死亡就不要动
        if (_character.ConditionState.CurrentState == CharacterStates.CharacterConditions.Dead)
        {
            _movement.SetHorizontalMovement(0f);
            _movement.SetVerticalMovement(0f);
            return;
        }

        float h = 0f;
        float v = 0f;

        // 小键盘 5 = 上
        if (Input.GetKey(KeyCode.Keypad5))
            v += inputValue;

        // 小键盘 2 = 下
        if (Input.GetKey(KeyCode.Keypad2))
            v -= inputValue;

        // 小键盘 1 = 左
        if (Input.GetKey(KeyCode.Keypad1))
            h -= inputValue;

        // 小键盘 3 = 右
        if (Input.GetKey(KeyCode.Keypad3))
            h += inputValue;

        // 把移动输入丢给 TopDown Engine 的 CharacterMovement
        _movement.SetHorizontalMovement(h);
        _movement.SetVerticalMovement(v);
    }
}
