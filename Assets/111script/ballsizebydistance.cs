using UnityEngine;
using TMPro;

public class ballsizebydistance : MonoBehaviour
{
    public Transform a;
    public Transform b;
    public Transform ball;
    public TextMeshPro tmp;
    public Camera user;

    public float nearSize = 5f;
    public float farSize = 0.5f;
    public float maxDistance = 10f;

  
    public float fadeOutDistance = 2f;

    public float floatSpeed = 2f;
    public float floatRange = 0.1f;
    public float fadeSpeed = 1f;

    private float recordSize = 0f;
    private float baseY;
    private Color tmpColor;
    private bool fading = false;
    private bool fadedOut = false;

    void Start()
    {
        if (tmp)
        {
            baseY = tmp.transform.localPosition.y;
            tmpColor = tmp.color;
        }

        if (!user)
        {
            user = Camera.main;
        }
    }

    void Update()
    {
        if (!a || !b || !ball) return;

        float d = Vector3.Distance(a.position, b.position);
        float t = Mathf.Clamp01(d / maxDistance);
        float s = Mathf.Lerp(nearSize, farSize, t);

        if (s > recordSize)
            recordSize = s;

        ball.localScale = Vector3.one * recordSize;

        if (!tmp) return;


        tmp.transform.LookAt(user.transform);
        tmp.transform.Rotate(0, 180f, 0);
        Vector3 p = tmp.transform.localPosition;
        p.y = baseY + Mathf.Sin(Time.time * floatSpeed) * floatRange;
        tmp.transform.localPosition = p;

        if (!fadedOut && d <= fadeOutDistance)
            fading = true;

        if (fading && !fadedOut)
        {
            Color c = tmp.color;
            c.a = Mathf.MoveTowards(c.a, 0f, fadeSpeed * Time.deltaTime);
            tmp.color = c;

            if (c.a <= 0.01f)
            {
                fadedOut = true;
                tmp.color = new Color(c.r, c.g, c.b, 0f);
            }
        }
    }
}
