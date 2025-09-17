using UnityEngine;

public class WalnutSpinnerKey : MonoBehaviour
{
    public Transform leftWalnut;
    public Transform rightWalnut;
    public float baseSpeed = 50f;
    public float accel = 100f;
    public float frictionSpin = 15f;
    public AudioSource f;
    public Animator anim;
    public GameObject objectToActivate; 

    private float currentSpeed = 0f;
    [Range(0.01f, 1f)]
    public float rewindSpeed = 0.2f; 

    private bool activated = false; 

    void Update()
    {
        float targetSpeed = 0f;
        bool keyHeld = false;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            targetSpeed = baseSpeed;
            keyHeld = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            targetSpeed = -baseSpeed;
            keyHeld = true;
        }

        if (Input.GetKey(KeyCode.UpArrow) && keyHeld)
        {
            if (targetSpeed > 0)
                targetSpeed += accel;
            else if (targetSpeed < 0)
                targetSpeed -= accel;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow) && keyHeld)
            f.Play();
            //  rotation
            currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 5f);
        transform.Rotate(0, 0, currentSpeed * Time.deltaTime, Space.Self);

        leftWalnut.Rotate(Vector3.up * frictionSpin * Time.deltaTime, Space.Self);
        rightWalnut.Rotate(Vector3.up * -frictionSpin * Time.deltaTime, Space.Self);

       
        if (anim != null)
        {
            AnimatorStateInfo state = anim.GetCurrentAnimatorStateInfo(0);

            if (keyHeld)
            {
                anim.speed = 1f; 
            }
            else
            {
                anim.speed = 0f; 
                float t = state.normalizedTime;
                t -= rewindSpeed * Time.deltaTime; 
                if (t < 0f) t = 0f;
                anim.Play(state.shortNameHash, 0, t);
            }

            // Check if animation finished
            if (!keyHeld && state.normalizedTime >= 1f && !activated)
            {
                if (objectToActivate != null)
                    objectToActivate.SetActive(true);
                activated = true;
            }
        }
    }
}
