using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public UnitStats stats;          
    public bool isEnemy;
    public AudioSource attackSound;

    private float currentHealth;
    private Slider healthSlider;
    private Rigidbody2D rb;

    [Header("Pushback Settings")]
    public float pushForce = 250f;  

    void Start()
    {
        currentHealth = stats.maxHealth;
        rb = GetComponent<Rigidbody2D>();
        healthSlider = GetComponentInChildren<Slider>();

        if (healthSlider != null)
        {
            healthSlider.maxValue = stats.maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;

        if (healthSlider != null)
            healthSlider.value = currentHealth;

        if (currentHealth <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Unit other = collision.GetComponent<Unit>();
        if (other != null && other.isEnemy != isEnemy)
        {
            //sound
            attackSound.Play();
            // deal damage
            other.TakeDamage(stats.damage);

           
            //TakeDamage(other.stats.damage);

            // pushback
            Vector2 pushDir = (other.transform.position - transform.position).normalized;

            float randomPush = Random.Range(pushForce * 0.8f, pushForce * 1.2f);

            if (other.rb != null)
                other.rb.AddForce(pushDir * randomPush);

            if (rb != null)
                rb.AddForce(-pushDir * randomPush);
        }
    }
}
