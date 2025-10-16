using UnityEngine;

public class TailChange : MonoBehaviour
{
    public GameObject truncate;
    public GameObject rounded;
    public GameObject forked;
    public GameObject lunate;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        truncate.SetActive(true);
        rounded.SetActive(false);
        forked.SetActive(false);
        lunate.SetActive(false);

        Data.isTruncate = true;
        Data.isRounded = false;
        Data.isLunate = false;
        Data.isForked = false;

        Data.groundMovementSpeed = 2f;
        Data.movementSpeed = 0.5f;

        Data.healthDecreaseAmount = 0.005f;
    }

    public void OnTruncate()
    {
        truncate.SetActive(true);
        rounded.SetActive(false);
        forked.SetActive(false);
        lunate.SetActive(false);

        Data.isTruncate = true;
        Data.isRounded = false;
        Data.isLunate = false;
        Data.isForked = false;

        Data.groundMovementSpeed = 2f;
        Data.movementSpeed = 0.5f;

        Data.healthDecreaseAmount = 0.005f;
    }

    public void OnRounded()
    {
        truncate.SetActive(false);
        rounded.SetActive(true);
        forked.SetActive(false);
        lunate.SetActive(false);

        Data.isTruncate = false;
        Data.isRounded = true;
        Data.isLunate = false;
        Data.isForked = false;

        Data.groundMovementSpeed = 1f;
        Data.movementSpeed = 0.2f;

        Data.healthDecreaseAmount = 0.005f;
    }

    public void OnForked()
    {
        truncate.SetActive(false);
        rounded.SetActive(false);
        forked.SetActive(true);
        lunate.SetActive(false);

        Data.isTruncate = false;
        Data.isRounded = false;
        Data.isLunate = false;
        Data.isForked = true;

        Data.groundMovementSpeed = 2f;
        Data.movementSpeed = 0.5f;

        Data.healthDecreaseAmount = 0.001f;
    }

    public void OnLunate()
    {
        truncate.SetActive(false);
        rounded.SetActive(false);
        forked.SetActive(false);
        lunate.SetActive(true);

        Data.isTruncate = false;
        Data.isRounded = false;
        Data.isLunate = true;
        Data.isForked = false;

        Data.groundMovementSpeed = 5f;
        Data.movementSpeed = 2f;

        Data.healthDecreaseAmount = 0.005f;
    }
}
