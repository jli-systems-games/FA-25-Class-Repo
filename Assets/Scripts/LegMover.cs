using UnityEngine;
using System.Collections;

/// referencing this tutorial: "How To Make A 2D Ik-Driven Spider" by ThatOneUnityDev
/// https://www.youtube.com/watch?v=9-7owO16syA
/// 
/// original code:
        //public Transform limbSolverTarget;
        //public float moveDistance;
        //public LayerMask groundLayer;

        //private void Update()
        //{ 
        //    CheckGround();

        //    if (Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance)
        //    {
        //        limbSolverTarget.position = transform.position;
        //    } 
        //}

        //public void CheckGround()
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, Vector2.down, 5, groundLayer);

        //    if (hit.collider != null)
        //    {
        //        Vector3 point = hit.point; point.y += 0.1f; transform.position = point;
        //    }
        //}

public class LegMover : MonoBehaviour
{
    public Transform limbSolverTarget;
    public float moveDistance = 0.5f;
    public LayerMask groundLayer;

    public bool isGroupA = true;

    private float stepCooldown = 0.05f;
    private float stepSpeed = 5f;
    private float stepHeight = 0.25f;

    private Vector3 desiredPosition;
    private static bool groupATurn = true;
    private static float lastStepTime;

    private void Update()
    {
        CheckGround();

        if (NeedsStep() && Time.time - lastStepTime > stepCooldown)
        {
            if ((groupATurn && isGroupA) || (!groupATurn && !isGroupA))
            {
                Step();
                groupATurn = !groupATurn;
                lastStepTime = Time.time;
            }
        }
    }

    public bool NeedsStep()
    {
        return Vector2.Distance(limbSolverTarget.position, transform.position) > moveDistance;
    }

    public void Step()
    {
        desiredPosition = transform.position;
        StopAllCoroutines();
        StartCoroutine(MoveTarget());
    }

    private IEnumerator MoveTarget()
    {
        Vector3 start = limbSolverTarget.position;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * stepSpeed;

            Vector3 mid = Vector3.Lerp(start, desiredPosition, t);
            mid.y += Mathf.Sin(t * Mathf.PI) * stepHeight;

            limbSolverTarget.position = mid;
            yield return null;
        }
    }

    public void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 5, groundLayer);
        if (hit.collider != null)
        {
            Vector3 point = hit.point;
            point.y += 0.1f;
            transform.position = point;
        }
    }
}
