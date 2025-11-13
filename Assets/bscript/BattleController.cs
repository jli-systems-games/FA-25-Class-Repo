using UnityEngine;

[RequireComponent(typeof(Unit))]
public class BattleController : MonoBehaviour
{
    //private Unit unit;

    //private void Start()
    //{
    //    unit = GetComponent<Unit>();
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    Unit other = collision.GetComponent<Unit>();

    //    // Only deal damage if it’s an opposing side
    //    if (other != null && other.isEnemy != unit.isEnemy)
    //    {
    //        other.TakeDamage(unit.damage);

    //        // Optional: Knockback
    //        Vector2 dir = (other.transform.position - transform.position).normalized;
    //        other.transform.position += (Vector3)dir * 0.2f; // small pushback
    //    }
    //}
}
