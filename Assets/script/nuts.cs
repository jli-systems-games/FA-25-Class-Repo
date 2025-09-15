using UnityEngine;

public class nuts : MonoBehaviour
{
    public Transform leftWalnut;
    public Transform rightWalnut;
    public float rotationSpeed = 5f;
    public float frictionSpin = 15f;
    public float inertiaDamp = 2f;

    public AudioSource frictionAudio;   // 循环摩擦声
    public AudioSource clickAudio;      // 咔嗒声

    private Vector2 lastMousePos;
    private Vector3 inertia;
    private float spinSpeed;            // 当前旋转速度
    private float clickTimer;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            lastMousePos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(0))
        {
            Vector2 delta = (Vector2)Input.mousePosition - lastMousePos;
            lastMousePos = Input.mousePosition;

            Vector3 rot = new Vector3(-delta.y, delta.x, 0) * rotationSpeed * Time.deltaTime;
            transform.Rotate(rot, Space.World);

            inertia = rot * 30f;
            spinSpeed = delta.magnitude / Time.deltaTime; // 速度值
        }
        else
        {
            if (inertia.magnitude > 0.01f)
            {
                transform.Rotate(inertia * Time.deltaTime, Space.World);
                spinSpeed = inertia.magnitude;
                inertia = Vector3.Lerp(inertia, Vector3.zero, Time.deltaTime * inertiaDamp);
            }
            else
            {
                spinSpeed = 0f;
            }
        }

        // 模拟两个核桃相反微转
        leftWalnut.Rotate(Vector3.up * frictionSpin * Time.deltaTime, Space.Self);
        rightWalnut.Rotate(Vector3.up * -frictionSpin * Time.deltaTime, Space.Self);

        HandleAudio();
    }

    void HandleAudio()
    {
        // 控制摩擦音
        if (spinSpeed > 1f)
        {
            if (!frictionAudio.isPlaying) frictionAudio.Play();
            frictionAudio.volume = Mathf.Clamp01(spinSpeed / 500f); // 转快声音更大
        }
        else
        {
            if (frictionAudio.isPlaying) frictionAudio.Stop();
        }

        // 控制咔嗒声
        if (spinSpeed > 300f)
        {
            clickTimer -= Time.deltaTime;
            if (clickTimer <= 0f)
            {
                clickAudio.PlayOneShot(clickAudio.clip);
                clickTimer = Random.Range(0.2f, 0.6f); // 随机间隔
            }
        }
    }
}
