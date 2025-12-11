using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ArrowRotation : MonoBehaviour
{
    public Transform arrow;

    public float fixedX = 90f;
    public float fixedY = 0f;

    void Update()
    {

        Vector3 direction = Data.arrowDestination.position - arrow.position;
        direction.y = 0f; 

        if (direction.sqrMagnitude < 0.001f)
            return;

        float angle = Mathf.Atan2(direction.z, direction.x) * Mathf.Rad2Deg;

        arrow.rotation = Quaternion.Euler(fixedX, fixedY, angle);

        arrow.gameObject.SetActive(Data.arrowEnabled);
    }
}
