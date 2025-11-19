using UnityEngine;

public class CarHealth : MonoBehaviour
{
    public float hp = 100f;
    public System.Action<CarHealth> onDeath;
    public void Damage(float d) { if (hp <= 0) return; hp -= d; if (hp <= 0) { onDeath?.Invoke(this); Destroy(gameObject); } }
}
