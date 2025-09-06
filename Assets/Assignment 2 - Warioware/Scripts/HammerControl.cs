using UnityEngine;

public class HammerControl : MonoBehaviour
{
    private Animator malletAnimator;
    
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
        }
    }
}
