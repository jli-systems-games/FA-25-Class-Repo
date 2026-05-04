using UnityEngine;

public class Translator : MonoBehaviour
{
    [Header("平移轴")]
    public bool moveX = true;
    public bool moveY = false;
    public bool moveZ = false;

    [Header("参数")]
    public float speed = 2f;
    public float distance = 3f;

    [Header("设置")]
    public Space space = Space.Self;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        Vector3 axis = new Vector3(
            moveX ? 1f : 0f,
            moveY ? 1f : 0f,
        moveZ ? 1f : 0f
        );

        float offset = Mathf.Sin(Time.time * speed) * distance;
        transform.position = startPos + axis * offset;
    }
}