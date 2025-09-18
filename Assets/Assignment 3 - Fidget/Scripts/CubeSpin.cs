using Unity.VisualScripting;
using UnityEngine;

public class CubeSpin : MonoBehaviour
{
    private float randomSpeed;
    private int randomDir;

    private void Start()
    {
        randomSpeed = Random.Range(50, 100);

        randomDir = Random.Range(0, 2);
    }
    void Update()
    {
        if (randomDir == 0)
        {
            transform.Rotate(randomSpeed * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.Rotate(-randomSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
