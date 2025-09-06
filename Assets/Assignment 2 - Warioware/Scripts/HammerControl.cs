using UnityEngine;

public class HammerControl : MonoBehaviour
{
    private Animator malletAnimator;
    public bool mouseClicked;
    [Space(10)]

    public float nailDownAmount = 0.05f;
    private float shortNailFinalPosition = 0.0038f;
    private float longNailFinalPosition = -0.031f;

    void Start()
    {
        malletAnimator = GetComponent<Animator>();
    }
    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, 1)); //Code from https://discussions.unity.com/t/3d-object-follow-mouse/792952
        transform.position = new Vector3(worldPos.x, 0.3064f, worldPos.z);

        if (Input.GetMouseButtonDown(0))
        {
            malletAnimator.SetTrigger("HitTrigger");
            mouseClicked = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (mouseClicked)
        {
            Vector3 newPos = other.transform.position;
            newPos.y -= nailDownAmount;
            if (other.gameObject.CompareTag("Short Nail"))
            {
                newPos.y = Mathf.Max(newPos.y, shortNailFinalPosition);
            }
            else if (other.gameObject.CompareTag("Long Nail"))
            {
                newPos.y = Mathf.Max(newPos.y, longNailFinalPosition);
            }

            other.transform.position = newPos;

            mouseClicked = false;
        }
    }
}
