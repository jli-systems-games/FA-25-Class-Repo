using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class WeaponsControl : MonoBehaviour
{
    public bool isGreatAxe;
    public bool isSawBladeSingle;
    public bool isSawBladeMultiple;
    public bool isNeedleTrap;
    public bool isSawBladeGround;

    public float movementSpeed;
    public float rotateSpeed;
    public float waitTime;

    private bool isPaused = false;
    private bool goingUp = true;

    void Update()
    {
        if (isGreatAxe)
        {
            GreatAxeMovement();
        }
        else if (isSawBladeSingle || isSawBladeMultiple)
        {
            SawBladeMovement();
        }
        else if (isNeedleTrap)
        {
            NeedleTrapMovement();
        }
        else if (isSawBladeGround)
        {
            SawBladeGroundMovement();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene("Game Over Scene");
        }
    }

    void NeedleTrapMovement()
    {
        float maxY = -2.6f;
        float minY = -4.3f;

        if (!isPaused)
        {
            float targetY = goingUp ? maxY : minY;
            transform.position = Vector3.MoveTowards(transform.position,
                new Vector3(transform.position.x, targetY, transform.position.z),
                movementSpeed * Time.deltaTime);

            if (Mathf.Abs(transform.position.y - targetY) < 0.01f)
            {
                StartCoroutine(PauseAndSwitch(waitTime));
            }
        }
    }

    private IEnumerator PauseAndSwitch(float delay)
    {
        isPaused = true;
        yield return new WaitForSeconds(delay);
        goingUp = !goingUp;
        isPaused = false;
    }

    void GreatAxeMovement()
    {
        float angle = Mathf.PingPong(Time.time * movementSpeed, 20 * 2) - 20;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    private void SawBladeGroundMovement()
    {
        transform.Rotate(Vector3.forward, rotateSpeed * Time.deltaTime, Space.World);

        float xPos = Mathf.PingPong(Time.time * movementSpeed, 1.3f * 2) - 1.3f;
        transform.localPosition = new Vector3(xPos, transform.localPosition.y, transform.localPosition.z);
    }

    private void SawBladeMovement()
    {
        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.World);

        if (isSawBladeSingle)
        {
            float yPos = Mathf.PingPong(Time.time * movementSpeed, 1.5f * 2) - 1.5f;
            transform.localPosition = new Vector3(transform.localPosition.x, yPos, transform.localPosition.z);
        }
        
    }
}
