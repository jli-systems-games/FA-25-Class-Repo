using UnityEngine;

public class SqueezeWalls : MonoBehaviour
{
    public Transform leftWall, rightWall;
    public float speed = 0.5f;
    public float minGap = 12f;
    void Update()
    {
        float gap = Vector3.Distance(leftWall.position, rightWall.position);
        if (gap <= minGap) return;
        Vector3 dir = (rightWall.position - leftWall.position).normalized;
        leftWall.position += dir * speed * Time.deltaTime;
        rightWall.position -= dir * speed * Time.deltaTime;
    }
}
