using UnityEngine;
using UnityEngine.InputSystem;
// From Youtube, Bilibili, and Comments
public class PlayerControl : MonoBehaviour
{
    public InputActionReference moveAction;  // 移动输入
    public float speed = 10f;   //玩家移动速度
    public InputActionReference swingAction;    //挥拍输入
    public bool isRightSide = false;    //检测玩家
    public Transform paddleVisual; //旋转的visual中心

    public float restAngle = 0f;    //回正角度
    public float swingAngle = 30f;   //挥拍角度
    public float swingSpeed = 200f;  //击球速度

    public float padding = 0.2f;    //控制网球拍于相机边缘的具体举例
    private float minX, maxX, minY, maxY;   //移动范围限制
    private Rigidbody2D rb;
    private bool isSwinging = false;    //检测玩家有没有挥拍
    private float currentAngle; //检测当前角度

    private void Awake()
    {
        //默认rd，确保为Kinematic的同时保持不变
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void OnEnable()
    {
        //开启
        if (moveAction != null) moveAction.action.Enable();

        if (swingAction != null)
        {
            swingAction.action.Enable();
            swingAction.action.performed += OnSwing;
        }
    }

    private void OnDisable()
    {
        //关闭
        if (moveAction != null) moveAction.action.Disable();

        if (swingAction != null)
        {
            swingAction.action.performed -= OnSwing;
            swingAction.action.Disable();
        }
    }

    private void Start()
    {
        //角度归零同时检查边界
        currentAngle = restAngle;
        ApplyAngle(restAngle);
        ComputeBounds();
    }

    private void ComputeBounds()
    {
        //球拍活动范围
        var cam = Camera.main;
        float halfH = cam.orthographicSize;
        float halfW = halfH * cam.aspect;

        //找到网球拍的SpriteR

        float halfWPad = 0.5f, halfHPad = 0.5f;
        var sr = (paddleVisual ? paddleVisual.GetComponent<SpriteRenderer>() : GetComponent<SpriteRenderer>());
        if (sr != null)
        {
            halfWPad = sr.bounds.extents.x;
            halfHPad = sr.bounds.extents.y;
        }
        
        //范围控制
        minX = cam.transform.position.x - halfW + halfWPad + padding;
        maxX = cam.transform.position.x + halfW - halfWPad - padding;
        minY = cam.transform.position.y - halfH + halfHPad + padding;
        maxY = cam.transform.position.y + halfH - halfHPad - padding;
    }

    private void FixedUpdate()
    {
        //移动输入
        Vector2 move = moveAction != null ? moveAction.action.ReadValue<Vector2>() : Vector2.zero;

        if (move.sqrMagnitude > 1f) move = move.normalized;

        Vector2 pos = rb.position + move * speed * Time.fixedDeltaTime;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);

        rb.MovePosition(pos);
    }

    private void OnSwing(InputAction.CallbackContext ctx)
    {
        //检测会拍，如果没有这重启尝试
        if (!isSwinging) StartCoroutine(SwingPaddle());
    }

    private System.Collections.IEnumerator SwingPaddle()
    {
        isSwinging = true;

        float swingStart = restAngle + (isRightSide ?  swingAngle : -swingAngle);
        while (!Mathf.Approximately(currentAngle, swingStart))
        {
            //开始会拍
            currentAngle = Mathf.MoveTowards(currentAngle, swingStart, swingSpeed * Time.deltaTime);
            ApplyAngle(currentAngle);
            yield return null;
        }

        float swingEnd = restAngle + (isRightSide ? -swingAngle :  swingAngle);
        while (!Mathf.Approximately(currentAngle, swingEnd))
        {
            //挥拍结束为止
            currentAngle = Mathf.MoveTowards(currentAngle, swingEnd, swingSpeed * Time.deltaTime);
            ApplyAngle(currentAngle);
            yield return null;
        }

        while (!Mathf.Approximately(currentAngle, restAngle))
        {
            //回归初始角度
            currentAngle = Mathf.MoveTowards(currentAngle, restAngle, swingSpeed * Time.deltaTime);
            ApplyAngle(currentAngle);
            yield return null;
        }

        isSwinging = false;
    }

    private void ApplyAngle(float angle)
    {
        //设置角度限制
        if (paddleVisual != null)
            paddleVisual.localRotation = Quaternion.Euler(0, 0, angle);
        else
            transform.rotation = Quaternion.Euler(0, 0, angle);
    }
}