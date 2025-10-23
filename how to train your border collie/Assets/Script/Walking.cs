using UnityEngine;

public class BorderCollieUIWalk : MonoBehaviour
{
    public float speed = 100f;         
    public float moveDistance = 300f;   
    private RectTransform rect;
    private Vector2 startPos;
    private bool movingRight = true;
    private bool isWalking = true;      

    void Start()
    {
        rect = GetComponent<RectTransform>();
        startPos = rect.anchoredPosition;
    }

    void Update()
    {
        if (!isWalking) return;

        Vector2 pos = rect.anchoredPosition;

        if (movingRight)
        {
            pos.x += speed * Time.deltaTime;
            if (pos.x >= startPos.x + moveDistance)
            {
                movingRight = false;
                Flip();
            }
        }
        else
        {
            pos.x -= speed * Time.deltaTime;
            if (pos.x <= startPos.x - moveDistance)
            {
                movingRight = true;
                Flip();
            }
        }

        rect.anchoredPosition = pos;
    }

    void Flip()
    {
        Vector3 scale = rect.localScale;
        scale.x *= -1;
        rect.localScale = scale;
    }


    public void StopWalking()
    {
        isWalking = false;
    }

    public void StartWalking()
    {
        isWalking = true;
    }
}