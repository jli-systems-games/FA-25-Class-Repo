using UnityEngine;

public class TracePath : MonoBehaviour
{
    public Transform[] tracePoints;
    private LineRenderer lr;

    void Start()
    {
        lr = GetComponent<LineRenderer>();
        lr.positionCount = tracePoints.Length;

        for (int i = 0; i < tracePoints.Length; i++)
        {
            lr.SetPosition(i, tracePoints[i].position);
        }
    }
}
