using UnityEngine;
using System.Collections.Generic;

public class TrackSwitch2D_Pos : MonoBehaviour
{
    [Header("fill")]
    public Train2DController_Pos train;
    public List<Vector3> leftPathPositions;
    public List<Vector3> rightPathPositions;

    [Header("chyange")]
    public float triggerDistance = 3.8f;

    [Header("ifleft")]
    public bool usingLeft = true;

    void Update()
    {
        if (!train || !train.isAlive) return;

        bool wantSwitch = Input.GetMouseButtonDown(0)
               || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began);

        if (!wantSwitch) return;

        float dist = Vector2.Distance(train.transform.position, transform.position);
        if (dist > triggerDistance) return;

        usingLeft = !usingLeft;
        train.SetNewPathPositions(usingLeft ? leftPathPositions : rightPathPositions, 0);

        transform.localScale = Vector3.one * (usingLeft ? 1.1f : 0.9f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, triggerDistance);
    }
}
