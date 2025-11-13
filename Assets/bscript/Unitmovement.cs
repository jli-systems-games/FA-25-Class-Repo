using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    private Unit unit;
    private float moveSpeed;

    void Start()
    {
        unit = GetComponent<Unit>();
        moveSpeed = unit.stats.moveSpeed;
    }

    void Update()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm == null || !gm.battleStarted) return;

        Unit target = FindClosestEnemy();
        if (target != null)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.transform.position,
                moveSpeed * Time.deltaTime
            );
        }
    }

    Unit FindClosestEnemy()
    {
        Unit[] allUnits = FindObjectsOfType<Unit>();
        Unit closest = null;
        float minDist = Mathf.Infinity;

        foreach (Unit u in allUnits)
        {
            if (u.isEnemy != unit.isEnemy)
            {
                float dist = Vector2.Distance(transform.position, u.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = u;
                }
            }
        }
        return closest;
    }
}
