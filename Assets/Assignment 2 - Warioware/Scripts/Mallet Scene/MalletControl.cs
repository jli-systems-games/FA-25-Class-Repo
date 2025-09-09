using UnityEngine;

public class MalletControl : MonoBehaviour
{
    private Animator malletAnimator;
    public bool mouseClicked;
    [Space(10)]

    public float nailDownAmount = 0.05f;
    private float nailFinalPosition = -0.031f;

    void Start()
    {
        malletAnimator = GetComponent<Animator>();
    }

    void Update()
    {
        //Make the mallet move according to mouse position
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
            //Make nails go down when mallet hits it
            Vector3 newPos = other.transform.position;
            newPos.y -= nailDownAmount;
            newPos.y = Mathf.Max(newPos.y, nailFinalPosition); //Clamp so that the nail ends up at the final position and not go more down

            other.transform.position = newPos;

            mouseClicked = false;
        }
    }
}
