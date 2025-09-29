using UnityEngine;

public enum KissCamState { Moving, Stopping }

public class KissCamController : MonoBehaviour
{
    public float fixedZ = 108.5f;
    public Vector2 xRange = new Vector2(52f, 98f);
    public Vector2 yRange = new Vector2(2f, 14f);

    public Vector2 checkpoint = new Vector2(77.6f, 4.89f);
    public float stopDuration = 3f;
    public float reachTolerance = 0.2f;

    public float moveSpeed = 7f;
    public bool randomPatrol = true;

    public UIController ui;
    public SFXVFXManager sfx;

    KissCamState state = KissCamState.Moving;
    float stopTimer = 0f;
    Vector3 target;

    void Start()
    {
        var p = transform.position;
        transform.position = new Vector3(
            Mathf.Clamp(p.x, xRange.x, xRange.y),
            Mathf.Clamp(p.y, yRange.x, yRange.y),
            fixedZ);
        PickNewTarget();
    }

    void Update()
    {
        if (state == KissCamState.Moving) MoveUpdate();
        else StopUpdate();
    }

    void MoveUpdate()
    {
        Vector2 now2 = new Vector2(transform.position.x, transform.position.y);
        if (Vector2.Distance(now2, checkpoint) <= reachTolerance)
        {
            state = KissCamState.Stopping;
            stopTimer = stopDuration;
            ui?.SetWarningFrame(true);
            sfx?.OnKissCamArriveCheckpoint();
            return;
        }

        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target) < 0.05f) PickNewTarget();
    }

    void StopUpdate()
    {
        stopTimer -= Time.deltaTime;
        if (stopTimer <= 0f)
        {
            state = KissCamState.Moving;
            ui?.SetWarningFrame(false);
            sfx?.OnKissCamLeaveCheckpoint();
            PickNewTarget();
        }
    }

    void PickNewTarget()
    {
        target = new Vector3(
            Random.Range(xRange.x, xRange.y),
            Random.Range(yRange.x, yRange.y),
            fixedZ);
    }

    public KissCamState GetState() => state;

    public bool IsAtCheckpoint()
    {
        Vector2 now2 = new Vector2(transform.position.x, transform.position.y);
        return Vector2.Distance(now2, checkpoint) <= reachTolerance;
    }
}
