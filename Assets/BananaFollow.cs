using UnityEngine;

public class BananaFollow : MonoBehaviour
{
    public Camera cam;

    public PottyZone currentTarget;

    float awayTimer = 0f;
    bool hasBeenFar = false;
    float stableTimer = 0f;
    Vector3 lastPos;

    void Update()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = transform.position.z;
        transform.position = mousePos;

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);

        if (dist > 5f)
        {
            hasBeenFar = true;
        }
        else
        {
            if (hasBeenFar && dist < 1.5f)
            {
                AchievementManager.Instance.Unlock("Comeback Kid");
                hasBeenFar = false;
            }
        }

        float moveDist = Vector3.Distance(transform.position, lastPos);

        if (moveDist < 0.01f)
        {
            stableTimer += Time.deltaTime;
            if (stableTimer == 3f)
            {
                AchievementManager.Instance.Unlock("Smooth Operator");
                stableTimer = 0f;
            }
        }
        else
        {
            stableTimer = 0f;
        }

        lastPos = transform.position;
    }
}
